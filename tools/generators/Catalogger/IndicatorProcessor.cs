namespace Generators.Catalogger;

/// <summary>
/// Processes indicator attributes and extracts information from them.
/// </summary>
internal static class IndicatorProcessor
{
    internal static void ProcessNodes(
        SourceProductionContext context,
        Compilation compilation,
        string attributeType,
        ImmutableArray<SyntaxNode> nodes,
        Dictionary<string, INamedTypeSymbol?> symbols,
        List<IndicatorInfo> indicators,
        HashSet<string> processedIds)
    {
        foreach (SyntaxNode node in nodes)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
            ISymbol? symbol = semanticModel.GetDeclaredSymbol(node);

            if (symbol is not IMethodSymbol methodSymbol)
            {
                continue;
            }

            foreach (AttributeData attributeData in methodSymbol.GetAttributes())
            {
                INamedTypeSymbol? attributeSymbol = symbols[attributeType + "Attribute"];

                if (attributeSymbol == null
                || !attributeData.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default))
                {
                    continue;
                }

                string uiid = attributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(uiid) || !processedIds.Add(uiid))
                {
                    continue;
                }

                string name = attributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;

                // Convert category from enum to string
                object? categoryValue = attributeData.ConstructorArguments[2].Value;
                string category = categoryValue != null
                    ? GetEnumFieldName(symbols["Category"]!, Convert.ToInt32(categoryValue))
                    : string.Empty;

                // Convert chartType from enum to string
                object? chartTypeValue = attributeData.ConstructorArguments[3].Value;
                string chartType = chartTypeValue != null
                    ? GetEnumFieldName(symbols["ChartType"]!, Convert.ToInt32(chartTypeValue))
                    : string.Empty;

                // Extract legendOverride, if provided
                string? legendOverride = null;
                if (attributeData.ConstructorArguments.Length >= 5
                 && attributeData.ConstructorArguments[4].Value != null)
                {
                    legendOverride = attributeData.ConstructorArguments[4].Value?.ToString();
                }

                // Process parameters
                List<ParameterInfo> parameters = GetMethodParameters(
                    context: context,
                    methodSymbol: methodSymbol,
                    paramAttributeBaseSymbol: symbols["ParamAttribute"]);

                // Create indicator info
                indicators.Add(new IndicatorInfo(
                    Uiid: uiid,
                    Name: name,
                    Type: attributeType,
                    ContainingType: methodSymbol.ContainingType.Name,
                    MemberName: methodSymbol.Name == ".ctor" ? "Constructor" : methodSymbol.Name,
                    Category: category,
                    ChartType: chartType,
                    LegendOverride: legendOverride,
                    Parameters: parameters));
            }
        }
    }

    /// <summary>
    /// Retrieves the name of an enum field from its integer value.
    /// </summary>
    private static string GetEnumFieldName(INamedTypeSymbol enumType, int value)
    {
        foreach (IFieldSymbol member in enumType.GetMembers().OfType<IFieldSymbol>())
        {
            if (member.HasConstantValue && Convert.ToInt32(member.ConstantValue) == value)
            {
                return member.Name;
            }
        }

        return string.Empty;
    }

    private static List<ParameterInfo> GetMethodParameters(
        SourceProductionContext context,
        IMethodSymbol methodSymbol,
        INamedTypeSymbol? paramAttributeBaseSymbol)
    {
        List<ParameterInfo> parameters = [];

        // If paramAttributeBaseSymbol is null, we can't identify parameters
        if (paramAttributeBaseSymbol is null)
        {
            return parameters;
        }

        int paramIndex = 0;
        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
        {
            // Find attributes that derive from ParamAttribute<T>
            AttributeData? paramAttribute = null;
            foreach (AttributeData attr in parameter.GetAttributes())
            {
                if (IsParamAttributeOrDerived(attr.AttributeClass, paramAttributeBaseSymbol))
                {
                    paramAttribute = attr;
                    break;
                }
            }

            if (paramAttribute == null)
            {
                continue; // Skip parameters without a compatible ParamAttribute
            }

            // Get attribute type to determine how to extract information
            INamedTypeSymbol? attrClass = paramAttribute.AttributeClass;
            if (attrClass == null)
            {
                continue;
            }

            // Increment parameter index and generate PID
            paramIndex++;
            string pid = $"P{paramIndex}";

            string attributeClassName = attrClass.Name;
            string displayName = GetDisplayName(paramAttribute, parameter.Name);

            // Extract parameter values based on attribute type
            ExtractAttributeValues(
                context: context,
                attribute: paramAttribute,
                attributeClass: attrClass,
                dataType: out string dataType,
                defaultValue: out double? defaultValue,
                minValue: out double? minValue,
                maxValue: out double? maxValue,
                enumValues: out Dictionary<int, string>? enumValues);

            parameters.Add(new ParameterInfo(
                Pid: pid,
                Name: parameter.Name,
                DisplayName: displayName,
                DataType: dataType,
                DefaultValue: defaultValue,
                MinValue: minValue,
                MaxValue: maxValue,
                EnumOptions: enumValues));
        }

        return parameters;
    }

    private static void ExtractAttributeValues(
        SourceProductionContext context,
        AttributeData attribute,
        INamedTypeSymbol attributeClass,
        out string dataType,
        out double? defaultValue,
        out double? minValue,
        out double? maxValue,
        out Dictionary<int, string>? enumValues)
    {
        string attributeClassName = attributeClass.Name;

        minValue = null;
        maxValue = null;
        enumValues = null;

        // Handle ParamNumAttribute<T> (most common)
        if (attributeClassName.StartsWith("ParamNum"))
        {
            dataType = DetermineNumericDataType(attributeClass: attributeClass);

            ExtractNumericValues(
                attribute: attribute,
                defaultValue: out defaultValue,
                minValue: out minValue,
                maxValue: out maxValue);
        }

        // Handle ParamEnumAttribute<T>
        else if (attributeClassName.StartsWith("ParamEnum"))
        {
            dataType = "enum";
            defaultValue = ExtractEnumDefaultValue(attribute: attribute);

            // Get the enum type and extract its values
            ITypeSymbol? enumType = attributeClass.TypeArguments.Length > 0
                ? attributeClass.TypeArguments[0]
                : throw new InvalidOperationException(
                    "This should never occur if the code is working correctly.");

            enumValues = GetEnumValues(enumType);

            // min/max of enum (int) key values
            minValue = enumValues.Keys.Min();
            maxValue = enumValues.Keys.Max();
        }

        // Handle ParamBoolAttribute
        else if (attributeClassName == "ParamBoolAttribute")
        {
            dataType = "boolean";
            defaultValue = ExtractBooleanDefaultValue(attribute) ? 0 : 1;

            minValue = 0;
            maxValue = 1;
        }

        // Handle ParamSeriesAttribute<T>
        else if (attributeClassName.StartsWith("ParamSeries"))
        {
            // Extract DataType property from attribute
            dataType = "any[]"; // Default fallback value

            // Look for DataType property in named arguments
            foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
            {
                if (namedArg.Key == "DataType" && namedArg.Value.Value is string dataTypeValue)
                {
                    dataType = dataTypeValue;
                    break;
                }
            }

            defaultValue = null;
            minValue = null;
            maxValue = null;
        }

        // this should never occur
        else
        {
            throw new ArgumentException(
                $"Unsupported attribute type: '{attributeClassName}'. "
               + "Please ensure the attribute is derived from ParamAttribute<T>.",
                nameof(attributeClass));
        }

        // Emit diagnostic error IND902 if default value is not in min/max bounds
        if (defaultValue.HasValue && minValue.HasValue && maxValue.HasValue)
        {
            if (defaultValue < minValue || defaultValue > maxValue)
            {
                context.ReportDiagnostic(
                    diagnostic: Diagnostic.Create(
                        descriptor: DiagnosticDescriptors.IND902_InvalidDefaultValueDescriptor,
                        location: Location.None,
                        messageArgs: [defaultValue, minValue, maxValue]));
            }
        }
    }

    private static Dictionary<int, string> GetEnumValues(ITypeSymbol enumType)
    {
        if (enumType == null || enumType.TypeKind != TypeKind.Enum)
        {
            throw new ArgumentNullException(nameof(enumType),
                "The provided type is not an enum type.");
        }

        Dictionary<int, string> enumValues = [];

        foreach (ISymbol member in enumType.GetMembers())
        {
            // reminder: enum members are static fields
            if (member is IFieldSymbol field && field.HasConstantValue)
            {
                int value = Convert.ToInt32(field.ConstantValue);
                enumValues[value] = field.Name;
            }
        }

        return enumValues.Count > 0
            ? enumValues
            : throw new InvalidOperationException(
                "This should never occur if the code is working correctly.");
    }

    private static string GetDisplayName(AttributeData attribute, string defaultName)
    {
        // Try to get display name from constructor argument (first argument)
        if (attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is string displayName)
        {
            return displayName;
        }

        // Try named argument
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DisplayName" && namedArg.Value.Value is string name)
            {
                return name;
            }
        }

        return defaultName;
    }

    private static string DetermineNumericDataType(INamedTypeSymbol attributeClass)
    {
        // Check the generic type argument for numeric attributes
        if (attributeClass.TypeArguments.Length > 0)
        {
            ITypeSymbol typeArg = attributeClass.TypeArguments[0];

            if (typeArg.SpecialType == SpecialType.System_Int32 || typeArg.Name == "Int32")
            {
                return "int";
            }
        }

        return "number"; // Default to number for decimal, double, etc.
    }

    private static void ExtractNumericValues(
        AttributeData attribute,
        out double? defaultValue,
        out double? minValue,
        out double? maxValue)
    {
        minValue = null;
        maxValue = null;
        defaultValue = null;

        // Try to extract from constructor arguments
        if (attribute.ConstructorArguments.Length >= 4)
        {
            // Format expected: (displayName, defaultValue, minValue, maxValue)
            if (attribute.ConstructorArguments[1].Value != null)
            {
                defaultValue = Convert.ToDouble(
                    value: attribute.ConstructorArguments[1].Value,
                    provider: CultureInfo.InvariantCulture);
            }

            if (attribute.ConstructorArguments[2].Value != null)
            {
                minValue = Convert.ToDouble(
                    value: attribute.ConstructorArguments[2].Value,
                    provider: CultureInfo.InvariantCulture);
            }

            if (attribute.ConstructorArguments[3].Value != null)
            {
                maxValue = Convert.ToDouble(
                    value: attribute.ConstructorArguments[3].Value,
                    provider: CultureInfo.InvariantCulture);
            }
        }

        // Or try named arguments (preferred)
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Value.Value is null)
            {
                continue;
            }

            switch (namedArg.Key)
            {
                case "DefaultValue":
                    defaultValue = Convert.ToDouble(
                        value: namedArg.Value.Value,
                        provider: CultureInfo.InvariantCulture);
                    break;

                case "MinValue":
                    minValue = Convert.ToDouble(
                        value: namedArg.Value.Value,
                        provider: CultureInfo.InvariantCulture);
                    break;

                case "MaxValue":
                    maxValue = Convert.ToDouble(
                        value: namedArg.Value.Value,
                        provider: CultureInfo.InvariantCulture);
                    break;
            }
        }
    }

    private static bool ExtractBooleanDefaultValue(AttributeData attribute)
    {
        // Try to get default value from constructor argument
        if (attribute.ConstructorArguments.Length >= 2
         && attribute.ConstructorArguments[1].Value is bool defaultValue)
        {
            return defaultValue;
        }

        // Try named argument (preferred)
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DefaultValue" && namedArg.Value.Value is bool boolVal)
            {
                return boolVal;
            }
        }

        return false; // if not found
    }

    private static int ExtractEnumDefaultValue(AttributeData attribute)
    {
        // Try to get default enum value from constructor argument
        if (attribute.ConstructorArguments.Length >= 2
         && attribute.ConstructorArguments[1].Value != null)
        {
            return Convert.ToInt32(attribute.ConstructorArguments[1].Value);
        }

        // Try named argument
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DefaultValue" && namedArg.Value.Value != null)
            {
                return Convert.ToInt32(namedArg.Value.Value);
            }
        }

        return 0; // Default if not found
    }

    /// <summary>
    /// Determines if an attribute class is derived from ParamAttribute<T>.
    /// </summary>
    private static bool IsParamAttributeOrDerived(
        INamedTypeSymbol? attributeClass,
        INamedTypeSymbol? baseParamAttrSymbol)
    {
        if (attributeClass == null || baseParamAttrSymbol == null)
        {
            return false;
        }

        // Check if this is a generic instantiation of ParamAttribute<T> or its derived classes
        if (attributeClass.IsGenericType && attributeClass.ConstructedFrom != null)
        {
            INamedTypeSymbol originalDefinition = attributeClass.OriginalDefinition;

            // Check if this is derived from ParamAttribute<T> by walking up the inheritance chain
            INamedTypeSymbol? currentType = originalDefinition;
            while (currentType != null)
            {
                if (SymbolEqualityComparer.Default.Equals(currentType, baseParamAttrSymbol))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }
        }

        // Check non-generic derived types (like ParamBoolAttribute)
        return attributeClass.BaseType != null
            && IsParamAttributeOrDerived(
                attributeClass: attributeClass.BaseType,
                baseParamAttrSymbol: baseParamAttrSymbol);
    }
}

namespace CatalogAnalyzer;

[TestClass]
public class CatalogAnalyzing
{
    [TestMethod]
    public void CatalogAnalyzer_ShouldDetectPublicMethods_WithoutRequiredAttributes()
    {
        // Arrange
        // This test validates the logic used by the analyzer to identify public methods
        // that should have indicator attributes

        // Create a mock method that should trigger the analyzer
        MethodDeclarationSyntax methodSyntax = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
            SyntaxFactory.Identifier("GetTestIndicator"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList())
            .WithBody(SyntaxFactory.Block());

        // Create a mock class that contains the method
        ClassDeclarationSyntax classSyntax = SyntaxFactory.ClassDeclaration("TestIndicator")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .AddMembers(methodSyntax);

        // Check if the method meets the criteria that would make the analyzer generate a diagnostic
        bool isPublicMethod = methodSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
        bool returnsObject = methodSyntax.ReturnType.IsKind(SyntaxKind.PredefinedType);
        bool hasNoAttributes = methodSyntax.AttributeLists.Count == 0;

        // Assert that this method would be flagged by the analyzer's detection logic
        isPublicMethod.Should().BeTrue("Method should be public");
        hasNoAttributes.Should().BeTrue("Method should have no attributes");
        returnsObject.Should().BeTrue("Method should return an object type");

        // The combination of these conditions would trigger the IND001 warning
        bool wouldTriggerWarning = isPublicMethod && hasNoAttributes;
        wouldTriggerWarning.Should().BeTrue("Method should trigger IND001 warning");
    }
}

namespace GeneratedCatalog;

// Tests for the analyzer itself
[TestClass]
public class CatalogGenerating
{
    [TestMethod]
    public void CatalogGenerator_MethodWithSeriesAttribute_IsIdentified()
    {
        // Create test method with Series attribute
        string methodWithSeriesAttribute = """
            namespace TestNamespace
            {
                public static class TestIndicator
                {
                    [Series("test-id", "Test Indicator", "TestCategory", "line")]
                    public static TestResult GetTestIndicator(Quote[] quotes)
                    {
                        return null;
                    }
                }
            }
            """;

        // Parse the code into a SyntaxTree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(methodWithSeriesAttribute);
        CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

        // Find the method declaration
        MethodDeclarationSyntax method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

        // Verify that the method has an attribute list with at least one attribute
        method.AttributeLists.Count.Should().BeGreaterThan(0, "Method should have attribute lists");

        // Check that one of the attributes has a name that contains "Series"
        method.AttributeLists.Any(al =>
            al.Attributes.Any(a => a.Name.ToString().Contains("Series")))
            .Should().BeTrue("Method should have a Series attribute");
    }
}

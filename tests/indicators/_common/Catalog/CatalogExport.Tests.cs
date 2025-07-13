using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Catalog;

[TestClass]
public class CatalogExport : TestBase
{
    // These fields are no longer used since we're skipping the export test
    // after removing the code generation system
    /*
    private readonly JsonSerializerOptions IndentedJsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // the generated catalog.json test exported file (below)
    private readonly string jsonPath = Path.Combine(Path.GetFullPath(Path.Combine(
        Directory.GetCurrentDirectory(), "..", "..", "..", "_common", "Catalog")), "catalog.json");
    */

    [TestMethod]
    public void ExportCatalogToJsonFile()
    {
        // Skip in CI/CD pipeline, only for local debugging
        if (Environment.GetEnvironmentVariable("CI") == "true")
        {
            return;
        }

        // SKIP: This test is not relevant after removing the automated catalog generation
        // Per Task 8.7: "Remove code generation system completely"
        // The catalog is now explicitly defined by indicator implementations, not generated
        Console.WriteLine("Test skipped: Catalog export is no longer applicable after removing code generation");
        return;
    }
}

using System.Reflection;

namespace Behavioral;

/// <summary>
/// Reads and invokes a listing's indicator method the way a catalog-driven consumer
/// does, without scanning the library assembly.
/// </summary>
[TestClass]
public class CatalogMethodOverloadsTests
{
    [TestMethod]
    public void EveryListingExposesItsMethodOverloads()
    {
        foreach (IndicatorListing listing in Catalog.Get())
        {
            IReadOnlyList<MethodInfo> overloads = listing.GetMethodOverloads();

            overloads.Should().NotBeEmpty($"{listing.Uiid} ({listing.Style}) names {listing.MethodName}");
            overloads.Should().OnlyContain(m => m.Name == listing.MethodName && m.IsStatic && m.IsPublic);
        }
    }

    [TestMethod]
    public void OverloadInvokesLikeTheDirectCall()
    {
        IReadOnlyList<Bar> bars = Data.GetDefault();
        IndicatorListing listing = Catalog.Get("EMA", Style.Series)!;

        MethodInfo method = listing.GetMethodOverloads()
            .Single(static m => m.GetParameters().Length == 2);

        object result = method.Invoke(null, [bars, 20]);

        result.Should().BeEquivalentTo(bars.ToEma(20));
    }

    [TestMethod]
    public void OverloadsCannotBeModifiedThroughTheResult()
    {
        IReadOnlyList<MethodInfo> overloads = Catalog.Get("EMA", Style.Series)!.GetMethodOverloads();

        overloads.Should().NotBeAssignableTo<MethodInfo[]>();
        overloads.Should().BeAssignableTo<ICollection<MethodInfo>>()
            .Which.IsReadOnly.Should().BeTrue();
    }

    [TestMethod]
    public void ListingWithoutMethodHasNoOverloads()
    {
        IndicatorListing listing = Catalog.Get("EMA", Style.Series)! with { MethodName = null };

        listing.GetMethodOverloads().Should().BeEmpty();
    }
}

# Guide and Pro Tips

## Quote

Historical quotes should be of consistent time frequency (e.g. per minute, hour, day, etc.).

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | long | Volume

There is also an `Index` property that is set internally, so do not set that value.  We set this to `public` visibility in case you want to use it in your own wrapper code.  See **Cleaning History** section below if you want to return the `Index` values.

## Cleaning History

If you intend to use the same composed `IEnumerable<Quote> history` in multiple calls and want to have the same `Index` values mapped into results, we recommend you pre-clean it to initialize those index values.  This will add the `Index` value and sort by `Date` provided; it will also perform basic checks for data quality.

You only need to do this if you want to use the `Index` value in your own wrapper software; otherwise, there is no need as `history` is cleaned on every call, internally.  If you pre-clean, the provided `history` will be used as-is without additional cleaning.  The original `Date` and composed `Index` is always returned in resultsets.

``` C#
// fetch historical quotes from your favorite feed, in Quote format
IEnumerable<Quote> history = GetHistoryFromFeed("SPY");

// preclean
history = Cleaners.PrepareHistory(history);
```

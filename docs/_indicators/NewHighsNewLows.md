---
title: New Highs New Lows(NHNL)
description: New Highs New Lows
permalink: /indicators/NewHighsNewLows/
image: /assets/charts/NewHighsNewLowsResult.png
type: price-trend
layout: indicator
---

# {{ page.title }}

The New Highs-New Lows indicator ("NH-NL") is a powerful trend indicator that displays the daily difference between the number of times the stock reached new 52-week highs and the number of times the stock reached new 52-week lows.

You can use this indicator to analyze a specific stock trend and verify the number of new highs a stock has reach in the last N days or you can analyze an entire sector or market.

When analyzing an individual stock, you can verify if a stock is reaching new highs while the sector or market is reaching new lows. this indicates that the stock is doing better than the market and also usually a stock that has been reaching more new highs in the last 3 to 6 months than the market tends to continue to do well in the next 3 to 6 months.

When analyzing an entire sector or market, It generally reaches its extreme lows slightly before a major market bottom. As the market then turns up from the major bottom, the indicator jumps up rapidly. During this period, many new stocks are making new highs because it's easy to make a new high when prices have been depressed for a long time.

As the cycle matures, a divergence often occurs as fewer and fewer stocks are making new highs (the indicator falls), yet the market indices continue to reach new highs. This is a classic bearish divergence that indicates that the current upward trend is weak and may reverse.

The NH-NL indicator oscillates around zero. If the indicator is positive, the bulls are in control. If it is negative, the bears are in control. You can trade the NH-NL indicator by buying and selling as the indicators passes through zero. This won't always keep you on the right side of the market, but it is helpful to confirm the current trend.

Consider a bull market, when the S&P 500 rose above its 50-day moving average, but only if the 10-day moving average of the NH-NL indicator was above zero. Consider a bear market, anytime the S&P 500 fell below its moving average.

Ignore buy signals unless the 10-day moving average of the NH-NL indicator was above zero, it will reduced the number of trades by 50% and increased profits by 9%.

The New Highs-New Lows is calculated by simply taking the difference between the number of stocks that made new 52-week highs and the number of stocks that made new 52-week lows. 
The Net provides an immediate score for internal strength or weakness in the market. There are more new highs when the indicator is positive, which favors the bulls. There are more new lows when the indicator is negative, which favors the bears. Chartists can analyze daily fluctuations or apply a moving average to create an oscillator that meanders above and below the zero line. Net New Highs can also be used like the AD Line by creating a High-Low Line based on cumulative Net New Highs.

Record High Percent is a breadth indicator based on new highs and new lows. The indicator is derived by dividing the number of new highs by the number of new highs plus new lows. This ratio shows new highs relative to the total (new highs plus new lows). Like all breadth indicators, Record High Percent is a measure of underlying strength or weakness in a particular index.

Readings below 50 (50%) indicate that there were more new lows than new highs. Readings above 50 (50%) indicate that there were more new highs than new lows. 0 indicates there were no new 52-week highs (0% new highs). 100 indicates there was at least 1 new high and no new lows (100% new highs). 50 indicates that new highs and new lows were equal and greater than zero (50% new highs and 50% new lows).

In general, a stock index is deemed strong (bullish) when Record High Percent is above 50, which means new highs are greater than new lows. Conversely, a stock index is deemed weak (bearish) when Record High Percent is below 50, which means new lows are greater than new highs. This indicator can move to its extremities and remain near its extremities when the underlying index is trending one way or the other. Readings consistently above 70 indicate a strong uptrend. Readings consistently below 30 indicate a strong downtrend. Record High Percent can also bounce between zero and one hundred during corrective periods or choppy periods.

[[Discuss] :speech_balloon:]({{site.github.repository_url}}/discussions/??? "Community discussion about this indicator")

![chart for {{page.title}}]({{site.baseurl}}{{page.image}})

```csharp
// usage for a single stock
IEnumerable<NewHighsNewLowsResult> results =
  quotes.GetNewHighsNewLows();

// usage for an entire sector stock
List<IEnumerable<NewHighsNewLowsResult>> sectorResults = new [] { stock1.GetNewHighsNewLows(), stock2.GetNewHighsNewLows(), stock3.GetNewHighsNewLows() ...  };
IEnumerable<NewHighsNewLowsResult> results =
  sectorResults.GetNewHighsNewLows();
```

## Parameters

**`tradingDays`** _`int`_ - Optional.  Number of trading days to be considered.  Must be greater than 1, if specified.

### Historical quotes requirements

You must have at least two historical quotes to cover the warmup periods; however, since this is a trendline, more is recommended.

`quotes` is a collection of generic `TQuote` historical price quotes.  It should have a consistent frequency (day, hour, minute, etc).  See [the Guide]({{site.baseurl}}/guide/#historical-quotes) for more information.

## Response

```csharp
IEnumerable<NewHighsNewLowsResult>
```

- This method returns a time series of all available indicator values for the `quotes` provided.
- It always returns the same number of elements as there are in the historical quotes.
- It does not return a single incremental indicator value.
- The first period New Highs New Lows will have `0` value since there's not enough data to calculate.

### ObvResult

**`Date`** _`DateTime`_ - Date from evaluated `TQuote`

**`NewHigh`** _`bool`_ - Whether it reach a new high on that date.

**`NewLow`** _`bool`_ - Whether it reach a new low on that date.

**`NewHighs`** _`double`_ - Number of stocks that reach new highd on that date.

**`NewLows`** _`double`_ - Number of stocks that reach new lows on that date.

**`LastNewHigh`** _`DateTime`_ - Date of when the lastest new highs happened.

**`LastNewLow`** _`DateTime`_ - Date of when the lastest new lows happened.

**`Net`** _`double`_ - The difference between new highs and new lows, it indicates the strength or weakness.

**`Cumulative`** _`double`_ - The cumulative value of the Net.

**`RecordHighPercent`** _`double`_ - Measure of underlying strength or weakness.

> :warning: **Warning**: absolute values in New Highs New Lows are somewhat meaningless. Use with caution.

### Utilities

- [.Find(lookupDate)]({{site.baseurl}}/utilities#find-indicator-result-by-date)
- [.RemoveWarmupPeriods(qty)]({{site.baseurl}}/utilities#remove-warmup-periods)

See [Utilities and helpers]({{site.baseurl}}/utilities#utilities-for-indicator-results) for more information.

## Chaining

You can interpret the NH-NL indicator as a divergence indicator. It will be useful to chain this indicator with the SMA indicator. You can plot a 10-day moving average of the NH-NL indicator to smooth the daily values.

Results can be further processed on `NewHighsNewLows` with additional chain-enabled indicators.

```csharp
// example
var results = quotes
    .GetNewHighsNewLows(..)
    .GetSma(10);
```

This indicator must be initially generated from `quotes` and then it **can** be combined to generated a collection of results to futher get secotr or market results as shown in the example above.

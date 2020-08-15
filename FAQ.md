<!-- markdownlint-disable MD026 -->
# Frequently asked questions

## Do you have any documentation?

Yes.  The documentation site is a GitHub Pages site here: [https://daveskender.github.io/Stock.Indicators](https://daveskender.github.io/Stock.Indicators).  It is automatically generated from the `README.md` files in this repository, so you can navigate from within repo `README.md` links too.

## Where can I get historical quote data?

There are many places to get stock market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, try [Alpha Vantage](https://www.alphavantage.co).

## How much historical quote data do I need?

Each indicator will need different amounts to calculate.  You can find guidance on the individual indicator documentation pages.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).  A `BadHistoryException` will be thrown if you do not provide enough history.

Note that some indicators, especially those that are derived from [Exponential Moving Average](/Indicators/Ema/README.md), use a smoothing technique where there is convergence over time.  While you can calculate these with the minimum amount of data, the precision to two decimal points often requires 250 or more preceding historical records.

For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of total historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results.

## More questions?

Contact us through the NuGet [Contact Owners](https://www.nuget.org/packages/Skender.Stock.Indicators) method or [submit an Issue](https://github.com/DaveSkender/Stock.Indicators/issues) with your question if it is publicly relevant.

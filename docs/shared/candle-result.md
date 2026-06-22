### `CandleResult`

| property    | type         | description |
| ----------- | ------------ | ----------- |
| `Timestamp` | _`DateTime`_ | Date from evaluated `TBar` |
| `Price`     | _`decimal`_  | Price of the most relevant OHLC candle element when a signal is present |
| `Match`     | _`Match`_    | Indicates a [matching signal type](#match) for this candlestick pattern |
| `Candle`    | _`CandleProperties`_ | Characteristics of the candle body and wicks |

#### `Match`

When a candlestick pattern is recognized, it produces a matching _**signal**_.  In some cases, an intrinsic _confirmation_ is also available after the _**signal**_.  In cases where previous bars were used to identify a pattern, they are indicated as the _**basis**_ for the _**signal**_.  This `enum` can also be referenced as an `int` value.  Documentation for each [candlestick pattern](/indicators/candlestick-patterns) will indicate whether confirmation and/or _**basis**_ information is produced.

| type                  |  int | description                         |
| --------------------- | ---: | ----------------------------------- |
| `Match.BullConfirmed` |  200 | Confirmation of a prior bull signal |
| `Match.BullSignal`    |  100 | Bullish signal                      |
| `Match.BullBasis`     |   10 | Bars supporting a bullish signal    |
| `Match.Neutral`       |    1 | Signal for non-directional patterns |
| `Match.None`          |    0 | No match                            |
| `Match.BearBasis`     |  -10 | Bars supporting a bearish signal    |
| `Match.BearSignal`    | -100 | Bearish signal                      |
| `Match.BearConfirmed` | -200 | Confirmation of a prior bear signal |

<!--@include: ../shared/candle-properties.md-->

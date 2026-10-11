### `CandleResult`

| property | description |
| ----- | ----------- |
| `Timestamp`<br>_`DateTime`_ | Date from evaluated `TBar` |
| `Price`<br>_`decimal`_ | Price of the most relevant OHLC candle element when a signal is present |
| `Match`<br>_`Match`_ | Indicates a [matching signal type](#match) for this candlestick pattern |
| `Candle`<br>_`CandleProperties`_ | Characteristics of the candle body and wicks |

#### `Match`

When a candlestick pattern is recognized, it produces a matching _**signal**_.  In some cases, an intrinsic _confirmation_ is also available after the _**signal**_.  In cases where previous bars were used to identify a pattern, they are indicated as the _**basis**_ for the _**signal**_.  This `enum` can also be referenced as an `int` value.  Documentation for each [candlestick pattern](/indicators/candlestick-patterns) will indicate whether confirmation and/or _**basis**_ information is produced.

| enum / value | description |
| ----- | ----------- |
| `Match.BullConfirmed`<br>200 | Confirmation of a prior bull signal |
| `Match.BullSignal`<br>100 | Bullish signal |
| `Match.BullBasis`<br>10 | Bars supporting a bullish signal |
| `Match.Neutral`<br>1 | Signal for non-directional patterns |
| `Match.None`<br>0 | No match |
| `Match.BearBasis`<br>-10 | Bars supporting a bearish signal |
| `Match.BearSignal`<br>-100 | Bearish signal |
| `Match.BearConfirmed`<br>-200 | Confirmation of a prior bear signal |

<!--@include: ../shared/candle-properties.md-->

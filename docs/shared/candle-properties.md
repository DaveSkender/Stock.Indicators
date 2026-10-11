#### `CandleProperties`

The `CandleProperties` record class extends the basic `Bar` type with calculated properties.

| property | description |
| ----- | ----------- |
| `Timestamp`<br>_`DateTime`_ | Close date |
| `Open`<br>_`decimal`_ | Open price |
| `High`<br>_`decimal`_ | High price |
| `Low`<br>_`decimal`_ | Low price |
| `Close`<br>_`decimal`_ | Close price |
| `Volume`<br>_`decimal`_ | Volume |
| `Size`<br>_`decimal?`_ | `High-Low` |
| `Body`<br>_`decimal?`_ | `\|Open-Close\|` |
| `UpperWick`<br>_`decimal?`_ | Upper wick size |
| `LowerWick`<br>_`decimal?`_ | Lower wick size |
| `BodyPct`<br>_`double?`_ | `Body/Size` |
| `UpperWickPct`<br>_`double?`_ | `UpperWick/Size` |
| `LowerWickPct`<br>_`double?`_ | `LowerWick/Size` |
| `IsBullish`<br>_`bool`_ | `Close>Open` direction |
| `IsBearish`<br>_`bool`_ | `Close<Open` direction |

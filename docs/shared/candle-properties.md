#### `CandleProperties`

The `CandleProperties` record class extends the basic `Bar` type with calculated properties.

| property       | type         | description            |
| -------------- | ------------ | ---------------------- |
| `Timestamp`    | _`DateTime`_ | Close date             |
| `Open`         | _`decimal`_  | Open price             |
| `High`         | _`decimal`_  | High price             |
| `Low`          | _`decimal`_  | Low price              |
| `Close`        | _`decimal`_  | Close price            |
| `Volume`       | _`decimal`_  | Volume                 |
| `Size`         | _`decimal`_  | `High-Low`             |
| `Body`         | _`decimal`_  | `\|Open-Close\|`       |
| `UpperWick`    | _`decimal`_  | Upper wick size        |
| `LowerWick`    | _`decimal`_  | Lower wick size        |
| `BodyPct`      | _`double`_   | `Body/Size`            |
| `UpperWickPct` | _`double`_   | `UpperWick/Size`       |
| `LowerWickPct` | _`double`_   | `LowerWick/Size`       |
| `IsBullish`    | _`bool`_     | `Close>Open` direction |
| `IsBearish`    | _`bool`_     | `Close<Open` direction |

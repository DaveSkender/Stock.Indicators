#### CandleProperties

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume
| `Size` | decimal | `High-Low`
| `Body` | decimal | `|Open-Close|`
| `UpperWick` | decimal | Upper wick size
| `LowerWick` | decimal | Lower wick size
| `BodyPct` | double | `Body/Size`
| `UpperWickPct` | double | `UpperWick/Size`
| `LowerWickPct` | double | `LowerWick/Size`
| `IsBullish` | bool | `Close>Open` direction
| `IsBearish` | bool | `Close<Open` direction

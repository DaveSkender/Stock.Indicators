### CandleResult

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Price` | decimal | Indicates the candle `Close` price when a signal is present
| `Signal` | Signal | Generated signal `enum`.  See [Signal](#signal) below
| `Candle` | Candle | Candle properties.  See [Candle](#candle) below

#### Signal

| type | int | description
|-- |--: |--
| `Signal.BullConfirmed` | 200 | Confirmation of a prior bull signal, if available; see notes above
| `Signal.BullSignal` | 100 | Matching bullish pattern
| `Signal.None` | 0 | No match
| `Signal.BearSignal` | -100 | Matching bearish pattern
| `Signal.BearConfirmed` | -200 | Confirmation of a prior bear signal, if available; see notes above

#### Candle

| name | type | notes
| -- |-- |--
| `Date` | DateTime | Date
| `Open` | decimal | Open price
| `High` | decimal | High price
| `Low` | decimal | Low price
| `Close` | decimal | Close price
| `Volume` | decimal | Volume
| `Size` | decimal | `High-Low`
| `Body` | decimal | `|Open-Close|` ($)
| `UpperWick` | decimal | Upper wick size ($)
| `LowerWick` | decimal | Lower wick size ($)
| `BodyPct` | double | `Body/Size`
| `UpperWickPct` | double | `UpperWick/Size`
| `LowerWickPct` | double | `Lowerwick/Size`
| `IsBullish` | bool | `Close>Open` direction
| `IsBearish` | bool | `Close<Open` direction

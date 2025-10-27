# Phase 12: Indicators to Optimize (1.3x-2x slowdown)

## StreamHub Indicators

| Indicator | Series (ns) | Stream (ns) | Ratio | Priority |
|-----------|-------------|-------------|-------|----------|
| BollingerBands | 50,093 | 93,438 | 1.87x | High |
| Adx | 23,930 | 44,351 | 1.85x | High |
| Renko | 23,018 | 39,460 | 1.71x | Medium |
| Epma | 42,662 | 61,902 | 1.45x | Medium |
| Donchian | 105,687 | 151,217 | 1.43x | Medium |
| Hma | 74,679 | 97,967 | 1.31x | Low |

**Total**: 6 StreamHub indicators

## BufferList Indicators

| Indicator | Series (ns) | Buffer (ns) | Ratio | Priority |
|-----------|-------------|-------------|-------|----------|
| Smma | 2,866 | 5,596 | 1.95x | High |
| Sma | 11,109 | 20,591 | 1.85x | High |
| Awesome | 20,162 | 36,920 | 1.83x | High |
| Cci | 21,615 | 38,834 | 1.80x | High |
| Roc | 4,226 | 7,377 | 1.75x | Medium |
| StochRsi | 31,449 | 54,452 | 1.73x | Medium |
| Smi | 14,502 | 24,550 | 1.69x | Medium |
| Tema | 3,374 | 5,544 | 1.64x | Medium |
| Tr | 12,006 | 19,520 | 1.63x | Medium |
| Macd | 6,480 | 10,017 | 1.55x | Medium |
| Ema | 2,749 | 4,154 | 1.51x | Medium |
| Trix | 3,761 | 5,642 | 1.50x | Low |
| RocWb | 11,824 | 16,987 | 1.44x | Low |
| T3 | 4,375 | 6,103 | 1.39x | Low |
| Vortex | 16,803 | 23,163 | 1.38x | Low |
| RollingPivots | 141,874 | 194,203 | 1.37x | Low |
| Beta | 66,837 | 89,585 | 1.34x | Low |
| Chandelier | 27,156 | 36,082 | 1.33x | Low |
| WilliamsR | 27,026 | 35,835 | 1.33x | Low |
| ZigZag | 44,648 | 58,979 | 1.32x | Low |
| Tsi | 7,394 | 9,633 | 1.30x | Low |

**Total**: 21 BufferList indicators

## Optimization Patterns

### Pattern 1: EMA-Family BufferList (High Priority)
- Smma, Ema, Tema, T3, Trix, Macd
- Already optimized StreamHub versions
- BufferList may have unnecessary allocations

### Pattern 2: Simple Moving Averages (High Priority)
- Sma, Smma
- Use circular buffers, track running sums

### Pattern 3: Volume/Price Combinations (Medium Priority)
- Awesome, Cci, Smi
- May have redundant calculations

### Pattern 4: Rate of Change (Medium Priority)
- Roc, RocWb
- Simple lookback operations

### Pattern 5: Complex Multi-Component (Low Priority)
- Beta, Vortex, RollingPivots, ZigZag, Tsi
- May require case-by-case analysis

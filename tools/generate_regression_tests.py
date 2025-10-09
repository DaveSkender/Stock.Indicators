#!/usr/bin/env python3
"""
Generate regression test files for all indicators based on baseline files.
"""

import os
import re
from pathlib import Path

# Mapping of baseline names to folder locations and indicator details
INDICATORS = {
    # Format: 'baseline-name': ('FolderName', 'ResultType', 'MethodCall', 'has_buffer', 'has_stream')
    'adl': ('Adl', 'AdlResult', 'ToAdl()', False, True),
    'adx': ('Adx', 'AdxResult', 'ToAdx(14)', False, True),
    'alligator': ('Alligator', 'AlligatorResult', 'ToAlligator()', False, True),
    'alma': ('Alma', 'AlmaResult', 'ToAlma(10, 0.85, 6)', False, True),
    'aroon': ('Aroon', 'AroonResult', 'ToAroon(25)', False, False),
    'atr': ('Atr', 'AtrResult', 'ToAtr(14)', False, False),
    'atr-stop': ('AtrStop', 'AtrStopResult', 'ToAtrStop(21, 3)', False, False),
    'awesome': ('Awesome', 'AwesomeResult', 'ToAwesome(5, 34)', False, False),
    'bb': ('BollingerBands', 'BollingerBandsResult', 'ToBollingerBands(20, 2)', False, False),
    'bop': ('Bop', 'BopResult', 'ToBop()', False, False),
    'cci': ('Cci', 'CciResult', 'ToCci(20)', False, False),
    'chaikin-osc': ('ChaikinOsc', 'ChaikinOscResult', 'ToChaikinOsc(3, 10)', False, False),
    'chexit': ('Chandelier', 'ChandelierResult', 'ToChandelier(22, 3)', False, False),
    'chop': ('Chop', 'ChopResult', 'ToChop(14)', False, False),
    'cmf': ('Cmf', 'CmfResult', 'ToCmf(20)', False, False),
    'cmo': ('Cmo', 'CmoResult', 'ToCmo(14)', False, False),
    'crsi': ('ConnorsRsi', 'ConnorsRsiResult', 'ToConnorsRsi(3, 2, 100)', False, False),
    'dema': ('Dema', 'DemaResult', 'ToDema(20)', False, False),
    'doji': ('Doji', 'CandleResult', 'ToDoji(0.1)', False, False),
    'donchian': ('Donchian', 'DonchianResult', 'ToDonchian(20)', False, False),
    'dpo': ('Dpo', 'DpoResult', 'ToDpo(20)', False, False),
    'dynamic': ('Dynamic', 'DynamicResult', 'ToDynamic(14)', False, False),
    'elder-ray': ('ElderRay', 'ElderRayResult', 'ToElderRay(13)', False, False),
    'ema': ('Ema', 'EmaResult', 'ToEma(20)', False, True),
    'epma': ('Epma', 'EpmaResult', 'ToEpma(20)', False, False),
    'fcb': ('Fcb', 'FcbResult', 'ToFcb(2)', False, False),
    'fisher': ('FisherTransform', 'FisherTransformResult', 'ToFisherTransform(10)', False, False),
    'force': ('ForceIndex', 'ForceIndexResult', 'ToForceIndex(13)', False, False),
    'fractal': ('Fractal', 'FractalResult', 'ToFractal(2)', False, False),
    'gator': ('Gator', 'GatorResult', 'ToGator()', False, False),
    'heikinashi': ('HeikinAshi', 'HeikinAshiResult', 'ToHeikinAshi()', False, False),
    'hma': ('Hma', 'HmaResult', 'ToHma(20)', False, False),
    'htl': ('HtTrendline', 'HtlResult', 'ToHtTrendline()', False, False),
    'hurst': ('Hurst', 'HurstResult', 'ToHurst(20)', False, False),
    'ichimoku': ('Ichimoku', 'IchimokuResult', 'ToIchimoku(9, 26, 52)', False, False),
    'kama': ('Kama', 'KamaResult', 'ToKama(10, 2, 30)', False, False),
    'keltner': ('Keltner', 'KeltnerResult', 'ToKeltner(20, 2, 10)', False, False),
    'kvo': ('Kvo', 'KvoResult', 'ToKvo(34, 55, 13)', False, False),
    'ma-env': ('MaEnvelopes', 'MaEnvelopeResult', 'ToMaEnvelopes(20, 2.5)', False, False),
    'macd': ('Macd', 'MacdResult', 'ToMacd(12, 26, 9)', False, False),
    'mama': ('Mama', 'MamaResult', 'ToMama(0.5, 0.05)', False, False),
    'marubozu': ('Marubozu', 'CandleResult', 'ToMarubozu(0.95)', False, False),
    'mfi': ('Mfi', 'MfiResult', 'ToMfi(14)', False, False),
    'obv': ('Obv', 'ObvResult', 'ToObv()', False, False),
    'pmo': ('Pmo', 'PmoResult', 'ToPmo(35, 20, 10)', False, False),
    'psar': ('ParabolicSar', 'ParabolicSarResult', 'ToParabolicSar(0.02, 0.2)', False, False),
    'pvo': ('Pvo', 'PvoResult', 'ToPvo(12, 26, 9)', False, False),
    'renko-atr': ('RenkoAtr', 'RenkoResult', 'ToRenko(14, EndType.Close)', False, False),
    'roc': ('Roc', 'RocResult', 'ToRoc(20)', False, False),
    'roc-wb': ('RocWb', 'RocWbResult', 'ToRocWb(12, 3, 6)', False, False),
    'rolling-pivots': ('RollingPivots', 'RollingPivotsResult', 'ToRollingPivots(11, 9)', False, False),
    'rsi': ('Rsi', 'RsiResult', 'ToRsi(14)', False, False),
    'slope': ('Slope', 'SlopeResult', 'ToSlope(20)', False, False),
    'sma': ('Sma', 'SmaResult', 'ToSma(20)', False, False),
    'sma-analysis': ('SmaAnalysis', 'SmaAnalysisResult', 'ToSmaAnalysis(20)', False, False),
    'smi': ('Smi', 'SmiResult', 'ToSmi(14, 20, 5, 3)', False, False),
    'smma': ('Smma', 'SmmaResult', 'ToSmma(20)', False, False),
    'starc': ('StarcBands', 'StarcBandsResult', 'ToStarcBands(20, 2, 10)', False, False),
    'stc': ('Stc', 'StcResult', 'ToStc(10, 23, 50)', False, False),
    'stdev': ('StdDev', 'StdDevResult', 'ToStdDev(20)', False, False),
    'stdev-channels': ('StdDevChannels', 'StdDevChannelsResult', 'ToStdDevChannels(20)', False, False),
    'stoch': ('Stoch', 'StochResult', 'ToStoch(14, 3, 3)', False, True),
    'stoch-rsi': ('StochRsi', 'StochRsiResult', 'ToStochRsi(14, 14, 3, 3)', False, True),
    'supertrend': ('SuperTrend', 'SuperTrendResult', 'ToSuperTrend(10, 3)', False, False),
    't3': ('T3', 'T3Result', 'ToT3(5, 0.7)', False, False),
    'tema': ('Tema', 'TemaResult', 'ToTema(20)', False, False),
    'tr': ('Tr', 'TrResult', 'ToTr()', False, False),
    'trix': ('Trix', 'TrixResult', 'ToTrix(20)', False, False),
    'tsi': ('Tsi', 'TsiResult', 'ToTsi(25, 13, 7)', False, False),
    'ulcer': ('UlcerIndex', 'UlcerIndexResult', 'ToUlcerIndex(14)', False, False),
    'uo': ('Ultimate', 'UltimateResult', 'ToUltimate(7, 14, 28)', False, False),
    'vol-stop': ('VolatilityStop', 'VolatilityStopResult', 'ToVolatilityStop(14, 3)', False, False),
    'vortex': ('Vortex', 'VortexResult', 'ToVortex(14)', False, False),
    'vwma': ('Vwma', 'VwmaResult', 'ToVwma(20)', False, False),
    'willr': ('WilliamsR', 'WilliamsResult', 'ToWilliamsR(14)', False, False),
    'wma': ('Wma', 'WmaResult', 'ToWma(20)', False, False),
}

def get_folder_prefix(folder_name):
    """Determine which folder prefix (a-d, e-k, m-r, s-z) based on folder name."""
    first_char = folder_name[0].upper()
    if 'A' <= first_char <= 'D':
        return 'a-d'
    elif 'E' <= first_char <= 'K':
        return 'e-k'
    elif 'M' <= first_char <= 'R':
        return 'm-r'
    elif 'S' <= first_char <= 'Z':
        return 's-z'
    else:
        return 'a-d'  # default

def generate_regression_test(baseline_name, folder_name, result_type, method_call, has_buffer, has_stream):
    """Generate regression test file content."""

    # Convert folder name to class name (e.g., ATR-STOP -> AtrStop)
    class_name = ''.join(word.capitalize() for word in folder_name.replace('-', ' ').split())

    # Get the method name without To prefix for List and Hub methods
    method_base = method_call.split('(')[0][2:]  # Remove 'To' prefix
    params = method_call.split('(', 1)[1].rstrip(')')
    params_part = f", {params}" if params else ""

    buffer_method = f"To{method_base}List({params})" if params else f"To{method_base}List()"
    stream_method = f"To{method_base}Hub({params})" if params else f"To{method_base}Hub()"

    # Create test content
    content = f"""using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class {class_name}Tests : RegressionTestBase<{result_type}>
{{
    public {class_name}Tests() : base("{baseline_name}.standard.json") {{ }}

    [TestMethod]
    public override void Series() => Quotes.{method_call}.AssertEquals(Expected);
"""

    if has_buffer:
        content += f"""
    [TestMethod]
    public override void Buffer() => Quotes.{buffer_method}.AssertEquals(Expected);
"""
    else:
        content += """
    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");
"""

    if has_stream:
        content += f"""
    [TestMethod]
    public override void Stream() => quoteHub.{stream_method}.Results.AssertEquals(Expected);
"""
    else:
        content += """
    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
"""

    content += "}\n"

    return content

def main():
    """Generate all regression test files."""
    base_path = Path("tests/indicators")

    for baseline_name, (folder_name, result_type, method_call, has_buffer, has_stream) in INDICATORS.items():
        folder_prefix = get_folder_prefix(folder_name)
        indicator_path = base_path / folder_prefix / folder_name

        if not indicator_path.exists():
            print(f"Warning: Folder not found: {indicator_path}")
            continue

        # Generate test file
        test_content = generate_regression_test(
            baseline_name, folder_name, result_type, method_call, has_buffer, has_stream
        )

        # Write to file
        test_file = indicator_path / f"{folder_name}.Regression.Tests.cs"
        test_file.write_text(test_content, encoding='utf-8')
        print(f"Created: {test_file}")

if __name__ == "__main__":
    main()

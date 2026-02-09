#!/usr/bin/env python3
"""
Analyze performance baselines to identify StreamHub and BufferList implementations
that are significantly slower than their Series equivalents.
"""

import json
from pathlib import Path
from typing import Dict, List, Tuple
from dataclasses import dataclass

@dataclass
class BenchmarkResult:
    name: str
    mean: float  # nanoseconds per operation

@dataclass
class ComparisonResult:
    indicator: str
    series_mean: float
    stream_mean: float
    buffer_mean: float
    stream_ratio: float  # stream/series
    buffer_ratio: float  # buffer/series

def parse_benchmark_file(filepath: Path) -> Dict[str, float]:
    """Extract benchmark means from a JSON file."""
    with open(filepath, 'r') as f:
        data = json.load(f)

    results = {}
    for benchmark in data.get('Benchmarks', []):
        method = benchmark.get('Method', '')
        stats = benchmark.get('Statistics', {})
        mean = stats.get('Mean', 0)

        # Extract indicator name from method
        # Methods are like "ToAdl", "AdlHub", "AdlList"
        if method:
            # Normalize method names
            if method.startswith('To'):
                indicator_name = method[2:]  # Remove "To" prefix
            elif method.endswith('Hub'):
                indicator_name = method[:-3]  # Remove "Hub" suffix
            elif method.endswith('List'):
                indicator_name = method[:-4]  # Remove "List" suffix
            elif method.endswith('Batch'):
                indicator_name = method[:-5]  # Remove "Batch" suffix
            elif method.endswith('Series'):
                indicator_name = method[:-6]  # Remove "Series" suffix
            elif method.endswith('Stream'):
                indicator_name = method[:-6]  # Remove "Stream" suffix
            elif method.endswith('Buffer'):
                indicator_name = method[:-6]  # Remove "Buffer" suffix
            else:
                indicator_name = method

            results[indicator_name] = mean

    return results

def main():
    baselines_dir = Path(__file__).parent

    # Parse all baseline files
    series_file = baselines_dir / 'Performance.SeriesIndicators-report-full.json'
    stream_file = baselines_dir / 'Performance.StreamIndicators-report-full.json'
    buffer_file = baselines_dir / 'Performance.BufferIndicators-report-full.json'

    print("Parsing baseline files...")
    series_results = parse_benchmark_file(series_file)
    stream_results = parse_benchmark_file(stream_file)
    buffer_results = parse_benchmark_file(buffer_file)

    # Find common indicators and compare
    all_indicators = set(series_results.keys()) | set(stream_results.keys()) | set(buffer_results.keys())

    comparisons: List[ComparisonResult] = []

    for indicator in sorted(all_indicators):
        series_mean = series_results.get(indicator, 0)
        stream_mean = stream_results.get(indicator, 0)
        buffer_mean = buffer_results.get(indicator, 0)

        if series_mean > 0:  # Only compare if series exists
            stream_ratio = stream_mean / series_mean if stream_mean > 0 else 0
            buffer_ratio = buffer_mean / series_mean if buffer_mean > 0 else 0

            comparisons.append(ComparisonResult(
                indicator=indicator,
                series_mean=series_mean,
                stream_mean=stream_mean,
                buffer_mean=buffer_mean,
                stream_ratio=stream_ratio,
                buffer_ratio=buffer_ratio
            ))

    # Output analysis
    print("\n" + "="*100)
    print("PERFORMANCE ANALYSIS: StreamHub & BufferList vs Series")
    print("="*100)

    # Identify problematic implementations
    threshold = 1.3  # 30% slower threshold
    critical_threshold = 2.0  # 100% slower (2x)

    print(f"\n🔍 Indicators where StreamHub is >{int((threshold-1)*100)}% slower than Series:")
    print("-"*100)
    print(f"{'Indicator':<20} {'Series (ns)':<15} {'Stream (ns)':<15} {'Ratio':>8} {'Status':<20}")
    print("-"*100)

    stream_issues = []
    for comp in comparisons:
        if comp.stream_ratio > threshold and comp.stream_mean > 0:
            status = "🔴 CRITICAL" if comp.stream_ratio >= critical_threshold else "⚠️  REVIEW"
            print(f"{comp.indicator:<20} {comp.series_mean:>13,.0f}  {comp.stream_mean:>13,.0f}  {comp.stream_ratio:>7.2f}x {status:<20}")
            stream_issues.append(comp)

    if not stream_issues:
        print("✅ No significant StreamHub performance issues found")

    print(f"\n🔍 Indicators where BufferList is >{int((threshold-1)*100)}% slower than Series:")
    print("-"*100)
    print(f"{'Indicator':<20} {'Series (ns)':<15} {'Buffer (ns)':<15} {'Ratio':>8} {'Status':<20}")
    print("-"*100)

    buffer_issues = []
    for comp in comparisons:
        if comp.buffer_ratio > threshold and comp.buffer_mean > 0:
            status = "🔴 CRITICAL" if comp.buffer_ratio >= critical_threshold else "⚠️  REVIEW"
            print(f"{comp.indicator:<20} {comp.series_mean:>13,.0f}  {comp.buffer_mean:>13,.0f}  {comp.buffer_ratio:>7.2f}x {status:<20}")
            buffer_issues.append(comp)

    if not buffer_issues:
        print("✅ No significant BufferList performance issues found")

    # Summary statistics
    print("\n" + "="*100)
    print("SUMMARY STATISTICS")
    print("="*100)

    if stream_issues:
        print(f"\nStreamHub Issues Found: {len(stream_issues)}")
        avg_stream_ratio = sum(c.stream_ratio for c in stream_issues) / len(stream_issues)
        max_stream = max(stream_issues, key=lambda c: c.stream_ratio)
        print(f"  Average slowdown: {avg_stream_ratio:.2f}x")
        print(f"  Worst case: {max_stream.indicator} at {max_stream.stream_ratio:.2f}x slower")

    if buffer_issues:
        print(f"\nBufferList Issues Found: {len(buffer_issues)}")
        avg_buffer_ratio = sum(c.buffer_ratio for c in buffer_issues) / len(buffer_issues)
        max_buffer = max(buffer_issues, key=lambda c: c.buffer_ratio)
        print(f"  Average slowdown: {avg_buffer_ratio:.2f}x")
        print(f"  Worst case: {max_buffer.indicator} at {max_buffer.buffer_ratio:.2f}x slower")

    # Detailed recommendations
    if stream_issues or buffer_issues:
        print("\n" + "="*100)
        print("RECOMMENDATIONS")
        print("="*100)
        print("\nIndicators to investigate for potential O(n²) or inefficient implementations:\n")

        critical_stream = [c for c in stream_issues if c.stream_ratio >= critical_threshold]
        critical_buffer = [c for c in buffer_issues if c.buffer_ratio >= critical_threshold]

        if critical_stream:
            print("🔴 CRITICAL StreamHub implementations (≥2x slower):")
            for comp in sorted(critical_stream, key=lambda c: c.stream_ratio, reverse=True):
                print(f"   - {comp.indicator}: {comp.stream_ratio:.2f}x slower ({comp.stream_mean:,.0f} ns vs {comp.series_mean:,.0f} ns)")

        if critical_buffer:
            print("\n🔴 CRITICAL BufferList implementations (≥2x slower):")
            for comp in sorted(critical_buffer, key=lambda c: c.buffer_ratio, reverse=True):
                print(f"   - {comp.indicator}: {comp.buffer_ratio:.2f}x slower ({comp.buffer_mean:,.0f} ns vs {comp.series_mean:,.0f} ns)")

        moderate_stream = [c for c in stream_issues if threshold <= c.stream_ratio < critical_threshold]
        moderate_buffer = [c for c in buffer_issues if threshold <= c.buffer_ratio < critical_threshold]

        if moderate_stream:
            print("\n⚠️  StreamHub implementations to review (1.3x-2x slower):")
            for comp in sorted(moderate_stream, key=lambda c: c.stream_ratio, reverse=True):
                print(f"   - {comp.indicator}: {comp.stream_ratio:.2f}x slower")

        if moderate_buffer:
            print("\n⚠️  BufferList implementations to review (1.3x-2x slower):")
            for comp in sorted(moderate_buffer, key=lambda c: c.buffer_ratio, reverse=True):
                print(f"   - {comp.indicator}: {comp.buffer_ratio:.2f}x slower")

        print("\nPotential causes of slowdowns:")
        print("  • O(n²) complexity in loops (nested iterations)")
        print("  • Unnecessary allocations or collection copies in hot paths")
        print("  • Inefficient lookback/window operations")
        print("  • Missing span optimizations or LINQ usage in tight loops")
        print("  • Redundant calculations not cached properly")
    else:
        print("\n✅ All StreamHub and BufferList implementations perform within acceptable range!")

    print("\n" + "="*100)

if __name__ == '__main__':
    main()

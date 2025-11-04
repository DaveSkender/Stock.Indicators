#!/usr/bin/env python3
"""
Validate StreamHub test interface compliance.

Checks that all StreamHub test classes implement the correct test interfaces
based on their corresponding hub provider base class.

Interface Selection Rules:
- ChainProvider<IReusable, T>: ITestChainObserver + ITestChainProvider
- ChainProvider<IQuote, T>: ITestQuoteObserver + ITestChainProvider
- PairsProvider<T>: ITestPairsObserver (only)
- StreamHub<IQuote, T>: ITestQuoteObserver (only)

Note: ITestChainObserver inherits ITestQuoteObserver, so implementing both is redundant.
"""

import os
import re
from pathlib import Path
from typing import List, Tuple, Optional

def find_hub_provider_type(hub_file: Path) -> Optional[str]:
    """Determine the provider base class used by the hub."""
    if not hub_file.exists():
        return None
    
    content = hub_file.read_text()
    
    # Check for PairsProvider
    if re.search(r':\s*PairsProvider<', content):
        return "PairsProvider"
    
    # Check for ChainProvider<IReusable, T>
    if re.search(r':\s*ChainProvider<IReusable,', content):
        return "ChainProvider<IReusable>"
    
    # Check for ChainProvider<IQuote, T>
    if re.search(r':\s*ChainProvider<IQuote,', content):
        return "ChainProvider<IQuote>"
    
    # Check for QuoteProvider
    if re.search(r':\s*QuoteProvider<', content):
        return "QuoteProvider"
    
    # Check for StreamHub<IQuote, T> (direct inheritance, not via provider)
    if re.search(r':\s*StreamHub<IQuote,', content):
        return "StreamHub<IQuote>"
    
    # Check for StreamHub<IReusable, T>
    if re.search(r':\s*StreamHub<IReusable,', content):
        return "StreamHub<IReusable>"
    
    return None

def find_test_interfaces(test_file: Path) -> List[str]:
    """Find which test interfaces the test class implements."""
    if not test_file.exists():
        return []
    
    content = test_file.read_text()
    
    # Find class declaration with interfaces
    class_match = re.search(r'public\s+class\s+\w+\s*:\s*StreamHubTestBase(?:,\s*(.+?))?[\s\{]', content)
    if not class_match:
        return []
    
    interfaces_str = class_match.group(1)
    if not interfaces_str:
        return []
    
    # Parse interfaces
    interfaces = []
    for iface in interfaces_str.split(','):
        iface = iface.strip()
        if iface.startswith('ITest'):
            interfaces.append(iface)
    
    return interfaces

def get_expected_interfaces(provider_type: str) -> List[str]:
    """Get the expected test interfaces for a given provider type."""
    if provider_type == "PairsProvider":
        return ["ITestPairsObserver"]
    elif provider_type == "ChainProvider<IReusable>":
        return ["ITestChainObserver", "ITestChainProvider"]
    elif provider_type == "ChainProvider<IQuote>":
        return ["ITestQuoteObserver", "ITestChainProvider"]
    elif provider_type == "QuoteProvider":
        return ["ITestQuoteObserver", "ITestChainProvider"]
    elif provider_type == "StreamHub<IQuote>":
        return ["ITestQuoteObserver"]
    elif provider_type == "StreamHub<IReusable>":
        return ["ITestQuoteObserver"]  # Can still observe quotes
    else:
        return []

def validate_test_file(test_file: Path, src_root: Path) -> Tuple[bool, str, List[str], List[str]]:
    """
    Validate a single test file.
    
    Returns: (is_compliant, provider_type, expected_interfaces, actual_interfaces)
    """
    # Find corresponding hub file
    test_rel = test_file.relative_to(test_file.parent.parent.parent.parent)
    hub_name = test_file.stem.replace('.StreamHub.Tests', '')
    
    # Search for hub file in src tree
    hub_candidates = list(src_root.rglob(f"{hub_name}/{hub_name}.StreamHub.cs"))
    if not hub_candidates:
        return True, "Hub not found", [], []
    
    hub_file = hub_candidates[0]
    provider_type = find_hub_provider_type(hub_file)
    
    if not provider_type:
        return True, "Provider type unknown", [], []
    
    expected = get_expected_interfaces(provider_type)
    actual = find_test_interfaces(test_file)
    
    # Check compliance
    is_compliant = set(expected) == set(actual)
    
    return is_compliant, provider_type, expected, actual

def main():
    """Main validation function."""
    repo_root = Path(__file__).parent.parent.parent
    tests_root = repo_root / "tests" / "indicators"
    src_root = repo_root / "src"
    
    # Find all StreamHub test files
    test_files = list(tests_root.rglob("*.StreamHub.Tests.cs"))
    test_files.sort()
    
    print(f"Found {len(test_files)} StreamHub test files\n")
    
    compliant = []
    non_compliant = []
    unknown = []
    
    for test_file in test_files:
        indicator_name = test_file.stem.replace('.StreamHub.Tests', '')
        is_compliant, provider_type, expected, actual = validate_test_file(test_file, src_root)
        
        if provider_type in ["Hub not found", "Provider type unknown"]:
            unknown.append((indicator_name, provider_type))
        elif is_compliant:
            compliant.append((indicator_name, provider_type, expected))
        else:
            non_compliant.append((indicator_name, provider_type, expected, actual))
    
    # Print results
    print("=" * 80)
    print("VALIDATION RESULTS")
    print("=" * 80)
    print()
    
    print(f"✓ Compliant: {len(compliant)}/{len(test_files)}")
    print(f"✗ Non-compliant: {len(non_compliant)}/{len(test_files)}")
    print(f"? Unknown: {len(unknown)}/{len(test_files)}")
    print()
    
    if non_compliant:
        print("=" * 80)
        print("NON-COMPLIANT TESTS")
        print("=" * 80)
        print()
        
        for indicator, provider_type, expected, actual in non_compliant:
            print(f"✗ {indicator}")
            print(f"  Provider: {provider_type}")
            print(f"  Expected: {', '.join(expected)}")
            print(f"  Actual:   {', '.join(actual) if actual else '(none)'}")
            
            missing = set(expected) - set(actual)
            extra = set(actual) - set(expected)
            
            if missing:
                print(f"  Missing:  {', '.join(missing)}")
            if extra:
                print(f"  Extra:    {', '.join(extra)}")
            print()
    
    if unknown:
        print("=" * 80)
        print("UNKNOWN/SKIPPED")
        print("=" * 80)
        print()
        
        for indicator, reason in unknown:
            print(f"? {indicator}: {reason}")
        print()
    
    print("=" * 80)
    print("SUMMARY")
    print("=" * 80)
    print()
    print(f"Total tests: {len(test_files)}")
    print(f"Compliant: {len(compliant)} ({len(compliant)*100//len(test_files)}%)")
    print(f"Non-compliant: {len(non_compliant)}")
    print(f"Unknown: {len(unknown)}")
    
    # Exit with error code if non-compliant tests found
    return 0 if len(non_compliant) == 0 else 1

if __name__ == "__main__":
    exit(main())

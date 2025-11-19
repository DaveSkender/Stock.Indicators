#!/usr/bin/env python3
import yaml
from pathlib import Path
import re

# Load categories
with open('_data/categories.yml', 'r') as f:
    categories = yaml.safe_load(f)

# Load aliases
with open('_data/aliases.yml', 'r') as f:
    aliases = yaml.safe_load(f)

# Read all indicator files and extract metadata
indicators_by_type = {}
indicator_files = Path('indicators').glob('*.md')

for indicator_file in indicator_files:
    with open(indicator_file, 'r', encoding='utf-8') as f:
        content = f.read()
        
    # Extract frontmatter
    parts = content.split('---', 2)
    if len(parts) >= 3:
        frontmatter = yaml.safe_load(parts[1])
        
        indicator_type = frontmatter.get('type', 'other')
        title = frontmatter.get('title', indicator_file.stem)
        
        if indicator_type not in indicators_by_type:
            indicators_by_type[indicator_type] = []
        
        indicators_by_type[indicator_type].append({
            'title': title,
            'file': indicator_file.stem,
        })

# Add aliases to the list
for alias in aliases:
    alias_type = alias.get('type', 'other')
    if alias_type not in indicators_by_type:
        indicators_by_type[alias_type] = []
    indicators_by_type[alias_type].append({
        'title': alias.get('title', ''),
        'file': alias.get('permalink', '').strip('/').split('/')[-1] if 'permalink' in alias else '',
    })

# Generate the indicators overview page
output = """---
title: Indicators and Overlays
description: The Stock Indicators for .NET library contains financial market technical analysis methods to view price patterns or to develop your own trading strategies in Microsoft .NET programming languages and developer platforms.
---

# Indicators and Overlays

The Stock Indicators for .NET library contains financial market technical analysis methods to view price patterns or to develop your own trading strategies in Microsoft .NET programming languages and developer platforms.

Categories include price trends, price channels, oscillators, stop and reverse, candlestick patterns, volume and momentum, moving averages, price transforms, price characteristics, and many classic numerical methods.

"""

# Add table of contents
output += "## Categories\n\n"
for category in categories:
    cat_type = category['type']
    cat_name = category['name']
    output += f"- [{cat_name}](#{cat_type})\n"
    
    if 'subcategories' in category:
        for subcat in category['subcategories']:
            subcat_type = subcat['type']
            subcat_name = subcat['name']
            output += f"  - [{subcat_name}](#{subcat_type})\n"

output += "\n"

# Add each category
for category in categories:
    cat_type = category['type']
    cat_name = category['name']
    
    output += f"## {cat_name} {{#{cat_type}}}\n\n"
    
    if cat_type in indicators_by_type:
        items = sorted(indicators_by_type[cat_type], key=lambda x: x['title'])
        for item in items:
            if item['file']:
                output += f"- [{item['title']}](/indicators/{item['file']})\n"
    
    output += "\n"
    
    # Add subcategories
    if 'subcategories' in category:
        for subcat in category['subcategories']:
            subcat_type = subcat['type']
            subcat_name = subcat['name']
            
            output += f"### {subcat_name} {{#{subcat_type}}}\n\n"
            
            if subcat_type in indicators_by_type:
                items = sorted(indicators_by_type[subcat_type], key=lambda x: x['title'])
                for item in items:
                    if item['file']:
                        output += f"- [{item['title']}](/indicators/{item['file']})\n"
            
            output += "\n"

# Write the file
with open('indicators.md', 'w', encoding='utf-8') as f:
    f.write(output)

print("Generated indicators.md overview page")

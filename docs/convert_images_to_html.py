#!/usr/bin/env python3
import re
from pathlib import Path

def fix_images(filepath):
    """Convert markdown images to HTML img tags"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    original = content
    
    # Replace ![alt](/assets/...) with <img src="/assets/..." alt="alt" />
    content = re.sub(
        r'!\[([^\]]*)\]\((\/assets\/[^)]+)\)',
        r'<img src="\2" alt="\1" />',
        content
    )
    
    if content != original:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        return True
    
    return False

# Fix all indicator files
indicator_files = Path('indicators').glob('*.md')
count = 0
for indicator_file in indicator_files:
    if fix_images(indicator_file):
        count += 1

print(f"Converted images in {count} indicator files")

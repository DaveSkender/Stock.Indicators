#!/usr/bin/env python3
import re
from pathlib import Path

def fix_indicator_file(filepath):
    """Remove image frontmatter and keep only markdown images"""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Remove image field from frontmatter
    content = re.sub(r'^image:.*$', '', content, flags=re.MULTILINE)
    
    # Clean up double newlines in frontmatter
    parts = content.split('---', 2)
    if len(parts) >= 3:
        frontmatter = parts[1]
        # Remove empty lines
        frontmatter_lines = [line for line in frontmatter.split('\n') if line.strip()]
        new_frontmatter = '\n'.join(frontmatter_lines)
        content = f"---\n{new_frontmatter}\n---\n{parts[2]}"
    
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

# Fix all indicator files
indicator_files = Path('indicators').glob('*.md')
count = 0
for indicator_file in indicator_files:
    fix_indicator_file(indicator_file)
    count += 1

print(f"Fixed {count} indicator files")

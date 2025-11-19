#!/usr/bin/env python3
import re
import os
from pathlib import Path

def convert_jekyll_to_vitepress(content, filepath=""):
    """Convert Jekyll syntax to Vitepress/Markdown"""
    
    # Remove Jekyll variable references
    content = re.sub(r'\{\{\s*page\.title\s*\}\}', '', content)
    content = re.sub(r'\{\{\s*site\.baseurl\s*\}\}', '', content)
    content = re.sub(r'\{\{\s*site\.github\.repository_url\s*\}\}', 'https://github.com/DaveSkender/Stock.Indicators', content)
    
    # Fix anchor links - remove #content
    content = re.sub(r'(href="[^"]*?)#content', r'\1', content)
    
    # Remove Jekyll liquid tags
    content = re.sub(r'\{%.*?%\}', '', content, flags=re.DOTALL)
    
    # Remove Jekyll comments
    content = re.sub(r'\{#.*?#\}', '', content, flags=re.DOTALL)
    
    # Fix relative links - remove baseurl references
    content = content.replace('{{site.baseurl}}/', '/')
    
    return content

def convert_frontmatter(frontmatter, filepath):
    """Convert Jekyll frontmatter to Vitepress format"""
    lines = []
    skip_keys = ['layout', 'lazy-images', 'relative_path', 'permalink']
    
    for line in frontmatter.split('\n'):
        # Skip Jekyll-specific keys
        skip = False
        for key in skip_keys:
            if line.strip().startswith(key + ':'):
                skip = True
                break
        if not skip and line.strip():
            lines.append(line)
    
    return '\n'.join(lines)

def migrate_file(source_path, dest_path):
    """Migrate a single markdown file"""
    with open(source_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Split frontmatter and content
    parts = content.split('---', 2)
    if len(parts) >= 3:
        frontmatter = parts[1]
        body = parts[2]
        
        # Convert both parts
        new_frontmatter = convert_frontmatter(frontmatter, source_path)
        new_body = convert_jekyll_to_vitepress(body, source_path)
        
        # Reassemble
        new_content = f"---\n{new_frontmatter}\n---\n{new_body}"
    else:
        new_content = convert_jekyll_to_vitepress(content, source_path)
    
    # Write to destination
    dest_dir = os.path.dirname(dest_path)
    if dest_dir:
        os.makedirs(dest_dir, exist_ok=True)
    with open(dest_path, 'w', encoding='utf-8') as f:
        f.write(new_content)
    
    print(f"Migrated: {source_path} -> {dest_path}")

# Migrate main pages
pages_map = {
    'pages/guide.md': 'guide.md',
    'pages/utilities.md': 'utilities.md', 
    'pages/performance.md': 'performance.md',
    'contributing.md': 'contributing.md',
}

for source, dest in pages_map.items():
    if os.path.exists(source):
        migrate_file(source, dest)

print("\nMain pages migrated successfully!")

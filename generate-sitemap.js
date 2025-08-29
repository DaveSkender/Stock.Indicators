#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

// Configuration
const baseUrl = 'https://dotnet.stockindicators.dev';
const docsDir = path.join(__dirname, 'public', 'docs');
const outputPath = path.join(__dirname, 'dist', 'static', 'browser', 'sitemap.xml');

// Get all markdown files
const markdownFiles = fs.readdirSync(docsDir)
  .filter(file => file.endsWith('.md'))
  .map(file => file.replace('.md', ''));

// Generate sitemap XML
const urls = markdownFiles.map(file => {
  const url = file === 'index' ? `${baseUrl}/` : `${baseUrl}/docs/${file}`;
  return `  <url>
    <loc>${url}</loc>
    <lastmod>${new Date().toISOString().split('T')[0]}</lastmod>
    <changefreq>monthly</changefreq>
    <priority>${file === 'index' ? '1.0' : '0.8'}</priority>
  </url>`;
}).join('\n');

const sitemap = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
${urls}
</urlset>`;

// Ensure output directory exists
const outputDir = path.dirname(outputPath);
if (!fs.existsSync(outputDir)) {
  fs.mkdirSync(outputDir, { recursive: true });
}

// Write sitemap
fs.writeFileSync(outputPath, sitemap);
console.log(`Sitemap generated at ${outputPath}`);
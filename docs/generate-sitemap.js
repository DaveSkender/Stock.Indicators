const fs = require('fs');
const path = require('path');

const pagesDir = path.join(__dirname, 'pages');
const indicatorsDir = path.join(__dirname, '_indicators');
const outputDir = path.join(__dirname, 'dist');
const baseUrl = 'https://stockindicators.dev';

function getMarkdownFiles(dir, recursive = true) {
  if (!fs.existsSync(dir)) {
    return [];
  }

  const entries = fs.readdirSync(dir, { withFileTypes: true });
  const files = [];

  for (const entry of entries) {
    if (entry.isDirectory() && recursive) {
      files.push(...getMarkdownFiles(path.join(dir, entry.name), recursive));
    } else if (entry.isFile() && entry.name.endsWith('.md')) {
      files.push(path.join(dir, entry.name));
    }
  }

  return files;
}

function generateSitemap() {
  const urls = [];

  // Add main site URL
  urls.push(`  <url>
    <loc>${baseUrl}</loc>
    <changefreq>weekly</changefreq>
    <priority>1.0</priority>
  </url>`);

  // Add root level pages (excluding home.md since that's the root)
  const contentDirectories = fs.existsSync(pagesDir) ? [pagesDir] : [__dirname];

  contentDirectories.forEach(dir => {
    const recursive = dir === pagesDir;
    const markdownFiles = getMarkdownFiles(dir, recursive);

    markdownFiles.forEach(filePath => {
      const content = fs.readFileSync(filePath, 'utf8');
      const permalinkMatch = content.match(/^permalink:\s*(.+)$/m);

      if (!permalinkMatch) {
        return;
      }

      const permalink = permalinkMatch[1].trim();

      if (permalink === '/' || permalink.length === 0) {
        return;
      }

      const normalized = permalink.endsWith('/') ? permalink : `${permalink}/`;
      const href = normalized.startsWith('/') ? normalized : `/${normalized}`;
      urls.push(`  <url>
    <loc>${baseUrl}${href}</loc>
    <changefreq>weekly</changefreq>
    <priority>0.8</priority>
  </url>`);
    });
  });

  // Add indicator pages
  if (fs.existsSync(indicatorsDir)) {
    const indicatorFiles = fs.readdirSync(indicatorsDir)
      .filter(file => file.endsWith('.md'))
      .map(file => file.replace('.md', ''));

    indicatorFiles.forEach(indicator => {
      urls.push(`  <url>
    <loc>${baseUrl}/indicators/${indicator}/</loc>
    <changefreq>monthly</changefreq>
    <priority>0.7</priority>
  </url>`);
    });
  }

  const sitemap = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
${urls.join('\n')}
</urlset>`;

  if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
  }

  fs.writeFileSync(path.join(outputDir, 'sitemap.xml'), sitemap);
  console.log(`Sitemap generated successfully with ${urls.length} URLs`);
}

generateSitemap();

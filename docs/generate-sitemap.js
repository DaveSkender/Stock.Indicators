const fs = require('fs');
const path = require('path');

const docsDir = path.join(__dirname, 'public', 'docs');
const outputDir = path.join(__dirname, 'dist', 'static');
const baseUrl = 'https://stockindicators.dev';

function generateSitemap() {
  if (!fs.existsSync(docsDir)) {
    console.log('Docs directory not found, skipping sitemap generation');
    return;
  }

  const files = fs.readdirSync(docsDir)
    .filter(file => file.endsWith('.md'))
    .map(file => file.replace('.md', ''));

  const urls = files.map(slug => {
    return `  <url>
    <loc>${baseUrl}/docs/${slug}</loc>
    <changefreq>weekly</changefreq>
    <priority>0.8</priority>
  </url>`;
  }).join('\n');

  const sitemap = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
  <url>
    <loc>${baseUrl}</loc>
    <changefreq>weekly</changefreq>
    <priority>1.0</priority>
  </url>
${urls}
</urlset>`;

  if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
  }

  fs.writeFileSync(path.join(outputDir, 'sitemap.xml'), sitemap);
  console.log('Sitemap generated successfully');
}

generateSitemap();
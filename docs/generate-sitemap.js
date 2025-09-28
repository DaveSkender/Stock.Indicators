const fs = require('fs');
const path = require('path');

const docsDir = path.join(__dirname, 'public', 'docs');
const indicatorsDir = path.join(__dirname, 'public', '_indicators');
const outputDir = path.join(__dirname, 'dist', 'static');
const baseUrl = 'https://stockindicators.dev';

function generateSitemap() {
  const urls = [];

  // Add main site URL
  urls.push(`  <url>
    <loc>${baseUrl}</loc>
    <changefreq>weekly</changefreq>
    <priority>1.0</priority>
  </url>`);

  // Add docs pages
  if (fs.existsSync(docsDir)) {
    const docsFiles = fs.readdirSync(docsDir)
      .filter(file => file.endsWith('.md'))
      .map(file => file.replace('.md', ''));

    docsFiles.forEach(slug => {
      urls.push(`  <url>
    <loc>${baseUrl}/docs/${slug}</loc>
    <changefreq>weekly</changefreq>
    <priority>0.8</priority>
  </url>`);
    });
  }

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
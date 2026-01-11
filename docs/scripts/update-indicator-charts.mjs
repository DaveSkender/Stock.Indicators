#!/usr/bin/env node

/**
 * Updates indicator markdown files to use the IndicatorChart component
 * instead of static image references.
 */

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const indicatorsDir = path.join(__dirname, '..', 'indicators');
const dataDir = path.join(__dirname, '..', '.vitepress', 'public', 'data');

// Get list of available data files
const dataFiles = fs.readdirSync(dataDir)
  .filter(f => f.endsWith('.json'))
  .map(f => path.basename(f, '.json'));

console.log(`Found ${dataFiles.length} data files`);

// Get list of indicator markdown files
const mdFiles = fs.readdirSync(indicatorsDir)
  .filter(f => f.endsWith('.md'));

console.log(`Found ${mdFiles.length} indicator markdown files`);

let updatedCount = 0;
let skippedCount = 0;

for (const mdFile of mdFiles) {
  const indicatorName = path.basename(mdFile, '.md');
  const mdPath = path.join(indicatorsDir, mdFile);
  
  // Check if we have data for this indicator
  if (!dataFiles.includes(indicatorName)) {
    console.log(`Skipping ${indicatorName} - no data file`);
    skippedCount++;
    continue;
  }
  
  let content = fs.readFileSync(mdPath, 'utf-8');
  
  // Check if already updated
  if (content.includes('IndicatorChart')) {
    console.log(`Skipping ${indicatorName} - already updated`);
    skippedCount++;
    continue;
  }
  
  // Pattern 1: <img src="/assets/charts/Indicator.png" alt="..." />
  // Pattern 1: HTML img tags
  const imgPattern = /<img\s+src="\/assets\/charts\/[^"]+\.png"\s+alt="[^"]*"\s*\/?>/gi;
  
  // Pattern 2: Markdown image syntax
  const mdImgPattern = /!\[[^\]]*\]\(\/assets\/charts\/[^)]+\.png\)/gi;
  
  let updated = false;
  const replacement = `<ClientOnly>\n  <IndicatorChart src="/data/${indicatorName}.json" :height="360" />\n</ClientOnly>`;
  
  // Check and replace HTML img tags
  if (imgPattern.test(content)) {
    imgPattern.lastIndex = 0; // Reset after test()
    content = content.replace(imgPattern, replacement);
    updated = true;
  }
  
  // Check and replace markdown images
  if (mdImgPattern.test(content)) {
    mdImgPattern.lastIndex = 0; // Reset after test()
    content = content.replace(mdImgPattern, replacement);
    updated = true;
  }
  
  if (updated) {
    fs.writeFileSync(mdPath, content, 'utf-8');
    console.log(`Updated ${indicatorName}`);
    updatedCount++;
  } else {
    console.log(`No image found in ${indicatorName}`);
    skippedCount++;
  }
}

console.log(`\nSummary: Updated ${updatedCount} files, skipped ${skippedCount} files`);

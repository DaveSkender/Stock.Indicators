// Script to generate alphabetically sorted indicator sidebar
import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const indicatorsDir = path.resolve(__dirname, '../../indicators')

// Read all indicator markdown files
const files = fs.readdirSync(indicatorsDir).filter(f => f.endsWith('.md'))

// Extract title and create sidebar items
const items = files.map(file => {
  const content = fs.readFileSync(path.join(indicatorsDir, file), 'utf-8')
  const titleMatch = content.match(/^title:\s*(.+)$/m)
  const title = titleMatch ? titleMatch[1].trim() : file.replace('.md', '')
  const link = `/indicators/${file.replace('.md', '')}`
  
  return { title, link, file }
})

// Sort alphabetically by title
items.sort((a, b) => a.title.localeCompare(b.title))

// Generate sidebar config
const sidebarItems = items.map(item => ({
  text: item.title,
  link: item.link
}))

console.log(JSON.stringify(sidebarItems, null, 2))

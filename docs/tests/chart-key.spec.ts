import { expect, test } from '@playwright/test'
import { readFileSync } from 'fs'
import { dirname, join } from 'path'
import { fileURLToPath } from 'url'

import { CHART_INDICATORS } from '../.vitepress/theme/chart-indicators'
import { indicatorPages } from './chart-pages'

// Every multi-line chart page carries a ChartKey, and the key's name rules hold
// for the snapshot rows that exercise them.

const __dirname = dirname(fileURLToPath(import.meta.url))
const SNAPSHOT = join(__dirname, '../.vitepress/public/data/chart-api/indicators.json')
const PAGES = join(__dirname, '../indicators')

interface Listing {
  uiid: string
  results: unknown[]
}

const listings = JSON.parse(readFileSync(SNAPSHOT, 'utf8')) as Listing[]

test('every multi-line chart page renders a ChartKey', () => {
  const missing = indicatorPages().filter(({ page, indicator }) => {
    const uiid = CHART_INDICATORS[indicator]?.uiid?.toLowerCase()
    const listing = listings.find((l) => l.uiid.toLowerCase() === uiid)
    if (!listing || listing.results.length < 2) return false
    return !new RegExp(`<ChartKey\\s+indicator="${indicator}"`).test(readFileSync(join(PAGES, `${page}.md`), 'utf8'))
  })
  expect(missing.map((m) => `${m.page}:${m.indicator}`)).toEqual([])
})

const NAMES: Array<{ page: string; names: string[] }> = [
  { page: 'fcb', names: ['Upper Band', 'Lower Band'] },
  { page: 'vortex', names: ['VI+', 'VI-'] },
]

for (const { page, names } of NAMES) {
  test(`${page} key names its lines`, async ({ page: browser }) => {
    await browser.goto(`/indicators/${page}`)
    const items = browser.locator('.chart-key li')
    await expect(items).toHaveCount(names.length)
    expect((await items.allTextContents()).map((t) => t.trim())).toEqual(names)
  })
}

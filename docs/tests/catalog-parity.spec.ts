import { expect, test } from '@playwright/test'
import { readdirSync, readFileSync } from 'fs'
import { dirname, join } from 'path'
import { fileURLToPath } from 'url'

// Drift check between each indicator page and the library source it documents:
// parameter names and defaults against the `To*` series signatures, parameter
// minimums against the `.Catalog.cs` listings, and result tables against the
// result records. Reads source files only, so it needs no browser.

const __dirname = dirname(fileURLToPath(import.meta.url))
const ROOT = join(__dirname, '../..')
const PAGES = join(ROOT, 'docs/indicators')

function walk(dir: string, suffix: string): string[] {
  return readdirSync(dir, { withFileTypes: true }).flatMap((e) => {
    const full = join(dir, e.name)
    if (e.isDirectory()) return walk(full, suffix)
    return full.endsWith(suffix) ? [full] : []
  })
}

function topLevelSplit(text: string): string[] {
  const parts: string[] = []
  let depth = 0
  let cur = ''
  for (const ch of text) {
    if ('<(['.includes(ch)) depth++
    if ('>)]'.includes(ch)) depth--
    if (ch === ',' && depth === 0) {
      parts.push(cur)
      cur = ''
    } else cur += ch
  }
  if (cur.trim()) parts.push(cur)
  return parts.map((p) => p.trim())
}

const stripComments = (s: string): string => s.replace(/\/\*[\s\S]*?\*\//g, '').replace(/\/\/.*$/gm, '')

// ---- Series signatures -----------------------------------------------------

interface Param { type: string; name: string; def: string | null }
const methods = new Map<string, Param[][]>()
for (const file of walk(join(ROOT, 'src/Indicators'), '.Series.cs')) {
  const src = stripComments(readFileSync(file, 'utf8'))
  const re = /public static [^\n(=]*?\b(To\w+)(?:<[^>(]*>)?\s*\(\s*this\s+([^,)]+?)\s+(\w+)\s*(?:,([^)]*))?\)/g
  for (const m of src.matchAll(re)) {
    const params = topLevelSplit(m[4] ?? '').flatMap((p) => {
      const pm = /^(.+?)\s+(\w+)\s*(?:=\s*(.+))?$/s.exec(p)
      return pm ? [{ type: pm[1].trim(), name: pm[2], def: pm[3]?.trim() ?? null }] : []
    })
    params.unshift({ type: m[2].trim(), name: m[3], def: null })
    methods.set(m[1], [...(methods.get(m[1]) ?? []), params])
  }
}

// ---- Catalog minimums ------------------------------------------------------

const minimums = new Map<string, Map<string, number>>()
for (const file of walk(join(ROOT, 'src/Indicators'), '.Catalog.cs')) {
  const src = stripComments(readFileSync(file, 'utf8'))
  const blocks = src.split(/internal static readonly IndicatorListing /).slice(1)
  const common = new Map<string, Map<string, number>>()
  for (const block of blocks) {
    const ident = block.split('=')[0].trim()
    const mins = new Map<string, number>()
    for (const m of block.matchAll(/\.Add(?:Enum)?Parameter<\w+>\(\s*"(\w+)"([^;]*?)(?=\n\s*\.(?:Add|With|Build)|\n\s*;|$)/gs)) {
      const min = /minimum:\s*(-?[\d.]+)/.exec(m[2])
      if (min) mins.set(m[1], Number(min[1]))
    }
    if (block.includes('new CatalogListingBuilder()')) {
      common.set(ident, mins)
      continue
    }
    const builder = /new CatalogListingBuilder\((\w+)\)/.exec(block)
    const method = /WithMethodName\((?:typeof\(\w+\),\s*)?"(\w+)"\)/.exec(block)
    const style = /WithStyle\(Style\.(\w+)\)/.exec(block)
    if (builder && method && style?.[1] === 'Series') minimums.set(method[1], common.get(builder[1]) ?? new Map())
  }
}

if (minimums.size === 0) throw new Error('no Series listings parsed from src/Indicators/**/*.Catalog.cs')

// ---- Result records --------------------------------------------------------

const records = new Map<string, Map<string, string>>()
for (const file of walk(join(ROOT, 'src'), '.cs')) {
  if (!/Result\.cs$/.test(file)) continue
  const src = stripComments(readFileSync(file, 'utf8'))
  for (const m of src.matchAll(/public (?:sealed )?record (?:class )?(\w+Result)\b/g)) {
    const props = new Map<string, string>()
    const rest = src.slice(m.index! + m[0].length)
    const ctor = /^\s*\(([\s\S]*?)\)\s*(?::|\{|;)/.exec(rest)
    if (ctor) {
      for (const p of topLevelSplit(ctor[1])) {
        const pm = /^(?:\[[^\]]*\]\s*)*([\w<>?.,\[\] ]+?)\s+(\w+)\s*(?:=[\s\S]*)?$/.exec(p)
        if (pm) props.set(pm[2], pm[1].trim())
      }
    } else {
      const open = rest.indexOf('{')
      let depth = 0
      let end = open
      for (let i = open; i < rest.length; i++) {
        if (rest[i] === '{') depth++
        if (rest[i] === '}' && --depth === 0) {
          end = i
          break
        }
      }
      for (const pm of rest.slice(open, end).matchAll(/public ([\w<>?]+) (\w+)\s*\{\s*get;/g)) props.set(pm[2], pm[1])
    }
    records.set(m[1], props)
  }
}

// ---- Page tables -----------------------------------------------------------

interface Row { name: string; type: string; text: string }

function tableAfter(lines: string[], from: number): Row[] {
  const rows: Row[] = []
  let started = false
  for (const line of lines.slice(from + 1)) {
    if (/^#{1,4} /.test(line)) break
    if (!line.startsWith('|')) {
      if (started) break
      continue
    }
    started = true
    const cells = line.split(/(?<!\\)\|/).slice(1, -1).map((c) => c.trim())
    const m = /^`([^`]+)`(?:\s+or\s+`[^`]+`)?\s*(?:<br>)?\s*_?`?([^`_]*)`?_?/.exec(cells[0] ?? '')
    if (m && !/^-+$/.test(m[1])) rows.push({ name: m[1], type: m[2].trim(), text: cells.slice(1).join(' ') })
  }
  return rows
}

function normalizeDefault(value: string): string {
  let v = value.trim().replace(/^"|"$/g, '')
  v = v.replace(/^[\w.]+\.(\w+)$/, (whole, last: string) => (/^\d/.test(whole) ? whole : last))
  const num = /^(-?\d+(?:\.\d+)?)[fdm]?$/i.exec(v)
  return num ? String(Number(num[1])) : v.toLowerCase()
}

// Parameters of an overload that the page documents in prose rather than in
// the table.
const IGNORED_PARAMS = new Set<string>([
  'ToFractal.leftSpan', 'ToFractal.rightSpan', // documented in the overload paragraph below the table
  'ToGator.alligator', // chained overload over Alligator results; the page documents the price-bar form
  'ToRenko.brickSize', // overload for decimal brick size; the table documents the ATR-based form
])

// Documented minimums that differ from the catalog today, as `documented/catalog`;
// each is a catalog fix tracked in #2320. The test fails when an entry stops
// drifting or either side moves, so the change that aligns a minimum also edits
// this map.
const KNOWN_MINIMUM_DRIFT = new Map<string, string>([
  ['ToAlligator.lipsPeriods', '1/2'],
  ['ToAtrStop.lookbackPeriods', '2/1'],
  ['ToChop.lookbackPeriods', '2/1'],
  ['ToDema.lookbackPeriods', '1/2'],
  ['ToDpo.lookbackPeriods', '1/2'],
  ['ToEma.lookbackPeriods', '1/2'],
  ['ToFractal.windowSpan', '2/1'],
  ['ToIchimoku.kijunPeriods', '1/2'],
  ['ToKama.erPeriods', '1/2'],
  ['ToMaEnvelopes.lookbackPeriods', '2/1'],
  ['ToMacd.signalPeriods', '0/1'],
  ['ToMfi.lookbackPeriods', '2/1'],
  ['ToPmo.timePeriods', '2/1'],
  ['ToPvo.signalPeriods', '0/1'],
  ['ToStarcBands.atrPeriods', '2/1'],
  ['ToStarcBands.smaPeriods', '2/1'],
  ['ToStc.cyclePeriods', '0/1'],
  ['ToStdDev.lookbackPeriods', '2/1'],
  ['ToStdDevChannels.lookbackPeriods', '2/1'],
  ['ToSuperTrend.lookbackPeriods', '2/1'],
  ['ToTema.lookbackPeriods', '1/2'],
  ['ToTsi.signalPeriods', '0/1'],
  ['ToVolatilityStop.lookbackPeriods', '2/1'],
])

function minimumDrift(method: string, row: Row, type: string) {
  const catalog = minimums.get(method)?.get(row.name)
  const gt = /(greater than (?:or equal to )?|at least )`?(-?\d+)`?(?!\d|\.\d)/.exec(row.text)
  if (!gt || catalog === undefined || type !== 'int') return null
  const inclusive = gt[1].includes('or equal to') || gt[1].startsWith('at least')
  const documented = Number(gt[2]) + (inclusive ? 0 : 1)
  return documented === catalog ? null : { key: `${method}.${row.name}`, documented, catalog }
}

// First Series method a page's code samples call.
function seriesCall(markdown: string): string | undefined {
  return /\.\s*(To[A-Z]\w*)\(/.exec(markdown)?.[1]
}

// Overview pages that list indicators and document no single method.
const OVERVIEW_PAGES = new Set<string>([
  'candlestick-patterns.md', 'moving-averages.md', 'numerical-analysis.md', 'oscillators.md', 'price-channels.md',
  'price-characteristics.md', 'price-transforms.md', 'price-trends.md', 'stop-and-reverse.md', 'volume-based.md',
])

const pages = readdirSync(PAGES).filter((f) => f.endsWith('.md')).sort()

for (const file of pages) {
  const markdown = readFileSync(join(PAGES, file), 'utf8')
  const method = seriesCall(markdown)
  const overloads = method ? methods.get(method) : undefined
  if (!method || !overloads) continue

  test(`${file} matches the source`, () => {
    const lines = markdown.split('\n')
    const problems: string[] = []

    const start = lines.findIndex((l) => /^##\s+Parameters\s*$/.test(l))
    const docParams = start < 0 ? [] : tableAfter(lines, start)
    const docNames = docParams.map((p) => p.name)
    const known = new Map<string, Param>()
    for (const overload of overloads) for (const p of overload) known.set(p.name, known.get(p.name) ?? p)
    const subject = overloads[0][0].name
    const sig = [...known.values()].filter((p) => !(p.name === subject && ['bars', 'quotes', 'source'].includes(p.name)))

    for (const p of sig) if (!docNames.includes(p.name) && !IGNORED_PARAMS.has(`${method}.${p.name}`)) problems.push(`${method}: parameter ${p.name} is not documented`)
    for (const n of docNames) if (!known.has(n)) problems.push(`${method}: documented parameter ${n} is not in the signature`)

    for (const row of docParams) {
      const p = known.get(row.name)
      if (!p) continue
      if (row.type && p.type.replace('?', '') !== row.type.replace('?', '')) {
        problems.push(`${method}.${row.name}: docs type ${row.type}, signature ${p.type}`)
      }
      const stated = /Default is `?(-?\d+(?:\.\d+)?|[A-Za-z_][\w.]*)`?/.exec(row.text)
      if (stated && p.def !== null && p.def !== 'null' && normalizeDefault(stated[1]) !== normalizeDefault(p.def)) {
        problems.push(`${method}.${row.name}: docs default ${stated[1]}, signature ${p.def}`)
      }
      if (!stated && p.def !== null && p.def !== 'null' && p.def !== 'default') {
        problems.push(`${method}.${row.name}: signature default ${p.def} is not stated`)
      }
      const drift = minimumDrift(method, row, p.type)
      if (drift && KNOWN_MINIMUM_DRIFT.get(drift.key) !== `${drift.documented}/${drift.catalog}`) {
        problems.push(`${drift.key}: docs minimum ${drift.documented}, catalog ${drift.catalog}`)
      }
    }

    lines.forEach((line, i) => {
      const heading = /^###\s+`(\w+Result)`/.exec(line)
      if (!heading) return
      const record = records.get(heading[1])
      if (!record) {
        problems.push(`${heading[1]}: no matching record found in src`)
        return
      }
      const rows = tableAfter(lines, i)
      const names = rows.map((r) => r.name)
      for (const n of record.keys()) if (!names.includes(n)) problems.push(`${heading[1]}: property ${n} is not documented`)
      for (const r of rows) {
        const declared = record.get(r.name)
        if (declared === undefined) {
          problems.push(`${heading[1]}: documented property ${r.name} is not in the record`)
        } else if (r.type && r.type !== declared) {
          problems.push(`${heading[1]}.${r.name}: docs type ${r.type}, record ${declared}`)
        }
      }
    })

    expect(problems, problems.join('\n')).toEqual([])
  })
}

test('every known minimum drift still drifts', () => {
  const observed = new Map<string, string>()
  for (const file of pages) {
    const markdown = readFileSync(join(PAGES, file), 'utf8')
    const method = seriesCall(markdown)
    const overloads = method ? methods.get(method) : undefined
    if (!method || !overloads) continue
    const lines = markdown.split('\n')
    const start = lines.findIndex((l) => /^##\s+Parameters\s*$/.test(l))
    const types = new Map(overloads.flat().map((p) => [p.name, p.type]))
    for (const row of start < 0 ? [] : tableAfter(lines, start)) {
      const drift = minimumDrift(method, row, types.get(row.name) ?? '')
      if (drift) observed.set(drift.key, `${drift.documented}/${drift.catalog}`)
    }
  }
  const stale = [...KNOWN_MINIMUM_DRIFT]
    .filter(([key, pair]) => observed.get(key) !== pair)
    .map(([key, pair]) => `${key} (pinned ${pair}, observed ${observed.get(key) ?? 'none'})`)
  expect(stale, `update KNOWN_MINIMUM_DRIFT: ${stale.join(', ')}`).toEqual([])
})

test('every indicator page names a Series method the source defines', () => {
  const uncovered = pages.filter((file) => {
    const method = seriesCall(readFileSync(join(PAGES, file), 'utf8'))
    return OVERVIEW_PAGES.has(file) ? false : !method || !methods.has(method)
  })
  expect(uncovered, `no checked Series method: ${uncovered.join(', ')}`).toEqual([])
})

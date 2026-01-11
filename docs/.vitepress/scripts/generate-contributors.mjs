import { writeFileSync } from 'fs'
import { resolve, dirname } from 'path'
import { fileURLToPath } from 'url'
import { Octokit } from '@octokit/rest'

const __dirname = dirname(fileURLToPath(import.meta.url))

async function generateContributors() {
  try {
    console.log('Fetching contributors from GitHub API...')

    const octokit = new Octokit({
      auth: process.env.GITHUB_TOKEN
    })

    const { data } = await octokit.repos.listContributors({
      owner: 'DaveSkender',
      repo: 'Stock.Indicators',
      per_page: 100
    })

    const contributors = data.map(contributor => ({
      login: contributor.login || 'unknown',
      avatar: `${contributor.avatar_url}&s=75`,
      url: contributor.html_url || '#',
      contributions: contributor.contributions || 0
    }))

    const outputPath = resolve(__dirname, '../public/contributors.json')
    writeFileSync(outputPath, JSON.stringify(contributors, null, 2))

    console.log(`✓ Generated contributors data: ${contributors.length} contributors`)
    console.log(`  Output: ${outputPath}`)
  } catch (error) {
    console.error('Error fetching contributors:', error.message)
    process.exit(1)
  }
}

generateContributors()

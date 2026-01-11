import { Octokit } from '@octokit/rest'

export interface Contributor {
  login: string
  avatar: string
  url: string
  contributions: number
}

declare const data: Contributor[]
export { data }

export default {
  async load(): Promise<Contributor[]> {
    try {
      const token = process.env.GITHUB_TOKEN
      if (!token) {
        console.warn('GITHUB_TOKEN not found. Contributors list will not be populated. Set GITHUB_TOKEN environment variable for local development.')
      }

      const octokit = new Octokit({
        auth: token
      })

      const { data } = await octokit.repos.listContributors({
        owner: 'DaveSkender',
        repo: 'Stock.Indicators',
        per_page: 100
      })

      // Filter out bot accounts (usernames containing '[bot]')
      return data
        .filter(contributor => !contributor.login?.includes('[bot]'))
        .map(contributor => ({
          login: contributor.login || 'unknown',
          avatar: `${contributor.avatar_url}&s=75`,
          url: contributor.html_url || '#',
          contributions: contributor.contributions || 0
        }))
    } catch (error) {
      console.error('Error fetching contributors:', error)
      return []
    }
  }
}

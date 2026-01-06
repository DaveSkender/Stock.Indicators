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
      const octokit = new Octokit({
        auth: process.env.GITHUB_TOKEN
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

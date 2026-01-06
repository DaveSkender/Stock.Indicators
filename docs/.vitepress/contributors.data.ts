import { Octokit } from '@octokit/rest'

export interface Contributor {
  avatar: string
  name: string
  title?: string
  links?: Array<{ icon: string; link: string }>
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

      return data.map(contributor => ({
        avatar: `${contributor.avatar_url}&s=128`,
        name: contributor.login || 'unknown',
        title: `${contributor.contributions || 0} contributions`,
        links: [
          { icon: 'github', link: contributor.html_url || '#' }
        ]
      }))
    } catch (error) {
      console.error('Error fetching contributors:', error)
      return []
    }
  }
}

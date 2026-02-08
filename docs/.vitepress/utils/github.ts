import { execSync } from 'child_process'

/**
 * Resolves GitHub token from environment or GitHub CLI
 *
 * Token resolution order:
 * 1. GITHUB_TOKEN environment variable (explicit override)
 * 2. GitHub CLI (`gh auth token`) if available
 * 3. null (graceful degradation)
 *
 * @param options - Configuration options
 * @param options.verbose - Log debug information during resolution
 * @returns GitHub token or null if unavailable
 */
export function getGitHubToken(options: { verbose?: boolean } = {}): string | null {
  const { verbose = false } = options

  // Check environment variable first
  if (process.env.GITHUB_TOKEN) {
    if (verbose) {
      console.log('[GitHub Token] Using GITHUB_TOKEN environment variable')
    }
    return process.env.GITHUB_TOKEN
  }

  // Attempt to use GitHub CLI
  try {
    const token = execSync('gh auth token', {
      encoding: 'utf8',
      stdio: ['pipe', 'pipe', 'ignore'], // Suppress stderr
      timeout: 5000 // 5 second timeout
    }).trim()

    if (token) {
      if (verbose) {
        console.log('[GitHub Token] Using token from GitHub CLI (gh auth token)')
      }
      return token
    }
  } catch (error) {
    // gh CLI not installed or not authenticated - silent failure
    if (verbose) {
      console.log('[GitHub Token] GitHub CLI not available or not authenticated')
    }
  }

  // No token available
  if (verbose) {
    console.log('[GitHub Token] No token found. Contributors list will be empty.')
    console.log('[GitHub Token] Tip: Install and authenticate with GitHub CLI: gh auth login')
  }

  return null
}

# Renovate Setup Guide

This repository uses [Renovate](https://docs.renovatebot.com/) to automatically keep dependencies up to date.

## Overview

Renovate is configured to:
- Run **weekly** (every weekend)
- Create PRs for dependency updates
- Group related packages together (e.g., Microsoft packages, React ecosystem)
- Automatically assign **@copilot** for review
- Run full build and test validation on every PR

## Setup Options

### Option 1: Renovate GitHub App (Recommended)

1. Install the [Renovate GitHub App](https://github.com/apps/renovate) on this repository
2. The app will read the configuration from `.github/renovate.json`
3. PRs will be created automatically on the configured schedule

**Advantages:**
- No token management required
- Official GitHub integration
- Better rate limits

### Option 2: Self-Hosted Renovate

If you prefer to run Renovate as a GitHub Action:

1. Create a Personal Access Token (PAT) with `repo` and `workflow` scopes
2. Add the token as a repository secret named `RENOVATE_TOKEN`
3. The workflow in `.github/workflows/renovate.yml` will run weekly

## How It Works

1. **Renovate runs weekly** and checks for dependency updates
2. **PRs are created** for available updates (grouped by category)
3. **Workflow triggers** (`.github/workflows/renovate-pr.yml`) on PR creation:
   - Validates the PR is from Renovate
   - Assigns @copilot as a reviewer
   - Runs full build and test suite
   - Comments on PR with test results
   - Adds "ready-to-merge" label if tests pass
4. **Manual review** by @copilot or maintainers
5. **Merge** when approved

## Configuration

The Renovate configuration (`.github/renovate.json`) includes:

### Package Grouping
- **Microsoft & Aspire packages**: All Microsoft.*, Aspire.*, System.* packages
- **Testing packages**: xunit, coverlet, Moq, Testcontainers
- **GitHub Actions**: All action updates grouped together
- **React ecosystem**: React and testing-library packages

### Security
- Vulnerability alerts enabled with "security" label
- Security updates are prioritized

### Rate Limiting
- Maximum 5 concurrent PRs
- No hourly limit (controlled by schedule instead)

## Acceptance Criteria

Each Renovate PR must:
- ✅ Build successfully (`dotnet build`)
- ✅ Pass all tests (`dotnet test`)
- ✅ Be reviewed by @copilot
- ✅ Have the "ready-to-merge" label (added automatically when tests pass)

## Manual Testing

To test Renovate locally:

```bash
# Install Renovate CLI
npm install -g renovate

# Run Renovate in dry-run mode
renovate --platform=github --token=YOUR_TOKEN --dry-run=full TomSB1423/NetWorth
```

## Troubleshooting

### Renovate isn't creating PRs
- Check if the GitHub App is installed (Option 1)
- Verify `RENOVATE_TOKEN` secret is set (Option 2)
- Check the schedule in `.github/renovate.json`

### PRs are failing validation
- Check the workflow run logs in Actions tab
- Ensure all tests pass locally: `dotnet test Networth.sln`
- Review the test results artifact uploaded by the workflow

### Too many PRs at once
- Adjust `prConcurrentLimit` in `.github/renovate.json`
- Consider grouping more packages together

## Further Reading

- [Renovate Documentation](https://docs.renovatebot.com/)
- [Configuration Options](https://docs.renovatebot.com/configuration-options/)
- [Package Rules](https://docs.renovatebot.com/configuration-options/#packagerules)

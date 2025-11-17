# CI/CD Setup Guide

This document describes the GitHub Actions workflows configured for the SmartCafe Menu Service.

## 📋 Overview

The project includes 3 main workflows:

1. **CI Workflow** - Main build, test, and Docker image creation
2. **Code Coverage Workflow** - Test coverage reporting and enforcement
3. **PR Validation Workflow** - PR quality checks and security scanning

## 🔄 Workflows

### 1. CI Workflow (`.github/workflows/ci.yml`)

**Triggers:**
- Push to `main` branch
- Pull requests to `main` branch

**Jobs:**

#### Build and Test
- Runs on: Ubuntu Latest
- Services: PostgreSQL 16
- Steps:
  1. Checkout code
  2. Setup .NET 10
  3. Restore dependencies
  4. Build solution (Release configuration)
  5. Run unit tests with trx logger
  6. Run integration tests with environment variables
  7. Publish test results

#### Code Quality
- Runs on: Ubuntu Latest
- Steps:
  1. Checkout code
  2. Setup .NET 10
  3. Restore dependencies
  4. Check code formatting (`dotnet format --verify-no-changes`)
  5. Run security scan

#### Build Docker
- Runs on: Ubuntu Latest
- Condition: Only on push to `main` branch
- Steps:
  1. Checkout code
  2. Setup Docker Buildx
  3. Build Docker image with caching
  4. Tag with commit SHA

### 2. Code Coverage Workflow (`.github/workflows/coverage.yml`)

**Triggers:**
- Push to `main` branch
- Pull requests to `main` branch

**Features:**
- PostgreSQL service container for integration tests
- XPlat Code Coverage collector
- ReportGenerator for HTML/Cobertura reports
- Coverage threshold enforcement (70% minimum)
- PR comment with coverage summary
- Coverage report artifact upload

**Coverage Threshold:**
```bash
Minimum: 70%
Action: Workflow fails if below threshold
```

### 3. PR Validation Workflow (`.github/workflows/pr-validation.yml`)

**Triggers:**
- Pull request opened
- Pull request synchronized (new commits)
- Pull request reopened

**Validation Jobs:**

#### PR Title Validation
Enforces conventional commit format:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style/formatting
- `refactor`: Code refactoring
- `perf`: Performance improvement
- `test`: Test updates
- `build`: Build system changes
- `ci`: CI configuration changes
- `chore`: Maintenance tasks
- `revert`: Revert previous commit

#### Merge Conflict Detection
- Automatically labels PRs with merge conflicts
- Label: `merge-conflict`

#### PR Size Labeling
Automatically labels PRs by size:
- `size/xs`: 0-10 lines
- `size/s`: 11-100 lines
- `size/m`: 101-500 lines
- `size/l`: 501-1000 lines
- `size/xl`: 1000+ lines

#### Security Scanning
- **Vulnerable Packages**: Checks for known vulnerabilities in NuGet packages
- **Secret Detection**: Scans for leaked secrets using TruffleHog

## 🔐 Secrets and Environment Variables

### Required Secrets

No secrets are required for basic CI/CD workflows. The workflows use:
- PostgreSQL service container (ephemeral)
- Mock Azure services for tests

### Optional Secrets (for production deployments)

```yaml
# Azure Container Registry (if pushing Docker images)
ACR_LOGIN_SERVER: your-registry.azurecr.io
ACR_USERNAME: service-principal-id
ACR_PASSWORD: service-principal-secret

# Code Coverage reporting (if using external service)
CODECOV_TOKEN: your-codecov-token
```

## 🛡️ Branch Protection Rules

Recommended settings for `main` branch:

```yaml
General:
  - Require pull request reviews before merging
  - Required approvals: 1
  - Dismiss stale pull request approvals when new commits are pushed
  - Require review from Code Owners

Status Checks:
  - Require status checks to pass before merging
  - Required checks:
    - Build and Test
    - Code Quality
    - Test Coverage
    - PR Validation / validate
    - PR Validation / security-scan
  - Require branches to be up to date before merging

Additional Rules:
  - Require conversation resolution before merging
  - Require linear history
  - Do not allow bypassing the above settings
  - Allow force pushes: Disabled
  - Allow deletions: Disabled
```

## 📊 Status Badges

Add these to your README.md:

```markdown
[![CI](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/ci.yml/badge.svg)](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/ci.yml)
[![Code Coverage](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/coverage.yml/badge.svg)](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/coverage.yml)
[![PR Validation](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/pr-validation.yml/badge.svg)](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/pr-validation.yml)
```

## 🧪 Running Workflows Locally

### Using act

```bash
# Install act
# Windows (Chocolatey)
choco install act-cli

# macOS (Homebrew)
brew install act

# Linux
curl https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash

# Run CI workflow
act -j build-and-test

# Run with pull_request event
act pull_request -j build-and-test

# Run specific workflow
act -W .github/workflows/ci.yml
```

### Manual Testing (Local)

```bash
# Run all checks that CI will run

# 1. Build in Release mode
dotnet build SmartCafe.Menu.sln --configuration Release

# 2. Run tests
dotnet test --configuration Release

# 3. Check code formatting
dotnet format SmartCafe.Menu.sln --verify-no-changes

# 4. Check for vulnerabilities
dotnet list package --vulnerable --include-transitive

# 5. Run coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Dependabot Configuration

Dependabot is configured to:
- Check for NuGet package updates weekly (Mondays, 6:00 AM)
- Check for GitHub Actions updates weekly
- Check for Docker base image updates weekly
- Group related packages (Microsoft, Azure, EF Core, Testing)
- Auto-assign PRs to `petro-konopelko`
- Label PRs with `dependencies` and package ecosystem

## 📝 Pull Request Template

The PR template (`PULL_REQUEST_TEMPLATE.md`) includes:
- Change type classification
- Related issues linking
- Testing checklist
- Code quality checklist
- Coverage reporting
- Deployment notes

## 🐛 Issue Templates

Two issue templates are available:

1. **Bug Report** (`bug_report.md`)
   - Bug description
   - Reproduction steps
   - Environment details
   - Expected vs actual behavior

2. **Feature Request** (`feature_request.md`)
   - Feature description
   - Problem statement
   - Use cases
   - Implementation considerations

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
- [Dependabot Configuration](https://docs.github.com/en/code-security/dependabot)
- [Code Owners](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-code-owners)

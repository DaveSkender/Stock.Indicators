# Security Policy

## Supported Versions

All current and recent versions of Stock Indicators for .NET are supported with security updates. We maintain security patches for:

| Version | Supported          |
| ------- | ------------------ |
| 2.x.x   | :white_check_mark: |
| 1.x.x   | :x: (EOL)          |

## Security Features

### Automated Security Scanning

This repository is protected by several automated security measures:

- **GitHub Advanced Security** - Code scanning and secret scanning
- **Dependabot** - Automated security updates for dependencies
- **Codacy** - Additional code quality and security analysis

### Dependency Management

- Dependencies are automatically monitored for known vulnerabilities
- Security updates are prioritized and applied promptly
- We maintain minimal dependencies to reduce attack surface

## Reporting a Vulnerability

### Private Reporting (Preferred)

For security vulnerabilities, please use GitHub's private reporting feature:

1. Go to the [Security Advisories](https://github.com/DaveSkender/Stock.Indicators/security/advisories) page
2. Click "Report a vulnerability"
3. Provide detailed information about the vulnerability

### Alternative Contact

You can also report security issues via email to [support@facioquo.com](mailto:support@facioquo.com).

### What to Include

When reporting a security vulnerability, please include:

- **Description** of the vulnerability
- **Steps to reproduce** the issue
- **Potential impact** and severity assessment
- **Suggested fix** (if you have one)
- **Your contact information** for follow-up

## Response Process

1. **Acknowledgment** - We'll acknowledge your report within 48 hours
2. **Investigation** - We'll investigate and assess the vulnerability
3. **Fix Development** - If confirmed, we'll develop a fix privately
4. **Release** - Security patches are released as soon as possible
5. **Disclosure** - Public disclosure follows responsible disclosure practices

## Security Best Practices for Users

### Package Verification

- Always verify package integrity when installing from NuGet
- Use package signature verification when available
- Monitor your dependencies for security updates

### Data Handling

- This library processes financial data but doesn't store or transmit it
- Ensure your quote data sources are secure and trusted
- Be mindful of sensitive data in logs or error reports

### Environment Security

- Keep your .NET runtime updated
- Use secure coding practices in applications using this library
- Regularly update dependencies in your projects

## Contact

For general security questions or concerns:

- **Email**: [support@facioquo.com](mailto:support@facioquo.com)
- **GitHub Discussions**: [Security discussions](https://github.com/DaveSkender/Stock.Indicators/discussions/categories/security)

We appreciate your help in keeping Stock Indicators for .NET secure! 🔒

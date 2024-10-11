# StormVault

StormVault is a secure, CLI-based password manager built with .NET. It allows users to safely store, retrieve, and manage passwords within encrypted vaults.

## Getting Started

### Prerequisites

- .NET SDK 8.0 or later

### Installation

1. Clone the repository:
```bash
   git clone https://github.com/tobman4/StormVault.git
   cd StormVault
```

2. Build the project:
```bash
  dotnet pack src/StormVault.CLI -o bin
```

3. Install
```bash
  dotnet tool install --global --add-source bin/ StormVault.CLI
```

4. Use
```bash
  storm --help
```

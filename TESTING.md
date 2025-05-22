# WanderlustJournal Test Documentation

This document provides information on how to run the tests for the WanderlustJournal project.

## Table of Contents

1. [Unit Tests](#unit-tests)
2. [End-to-End (E2E) Tests](#end-to-end-e2e-tests)
3. [CI Pipeline Integration](#ci-pipeline-integration)

## Unit Tests

The unit tests are implemented using xUnit and can be found in the `WanderlustJournal.Tests` project.

### Running Unit Tests

To run the unit tests, execute the following command from the solution directory:

```bash
dotnet test WanderlustJournal.Tests
```

To run a specific test class:

```bash
dotnet test WanderlustJournal.Tests --filter "FullyQualifiedName~JournalEntryServiceTests"
```

### Unit Test Coverage

The unit tests cover the following components:

- **Services**
  - `JournalEntryService`: CRUD operations, search functionality
  - `GeocodingService`: Location geocoding with error handling
- **Data**
  - `DatabaseMigrationHelper`: Database creation and migration
- **Pages**
  - Index Page Model
  - Journal CRUD Page Models

## End-to-End (E2E) Tests

The E2E tests are implemented using Microsoft Playwright and MSTest. They can be found in the `WanderlustJournal.E2ETests` project.

### Prerequisites

1. Install the Playwright browsers by running:

```bash
dotnet build WanderlustJournal.E2ETests
cd WanderlustJournal.E2ETests
pwsh bin/Debug/net8.0/playwright.ps1 install
```

Or on Linux/macOS:

```bash
dotnet build WanderlustJournal.E2ETests
cd WanderlustJournal.E2ETests
./bin/Debug/net8.0/playwright.sh install
```

### Running the Application for E2E Tests

Before running the E2E tests, make sure the WanderlustJournal application is running on `https://localhost:5001`:

```bash
cd WanderlustJournal.5
dotnet run
```

### Running E2E Tests

With the application running, in a separate terminal, execute:

```bash
dotnet test WanderlustJournal.E2ETests
```

To run a specific test class:

```bash
dotnet test WanderlustJournal.E2ETests --filter "FullyQualifiedName~NavigationTests"
```

### E2E Test Coverage

The E2E tests verify the following user workflows:

- **Navigation**
  - Home to Journal List
  - Home to Create Journal
  - Journal List to Create Journal
- **Journal Entry Operations**
  - Creating a new journal entry
  - Searching for journal entries
- **Form Validation**
  - Required field validation
  - Successful form submission

## CI Pipeline Integration

Both test projects can be integrated into a CI pipeline using GitHub Actions, Azure DevOps, or another CI system.

Here's an example GitHub Actions workflow:

```yaml
name: Test

on:
  push:
    branches: [ main, master, develop ]
  pull_request:
    branches: [ main, master, develop ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Run unit tests
      run: dotnet test WanderlustJournal.Tests --no-build
      
    - name: Install Playwright browsers
      run: |
        dotnet build WanderlustJournal.E2ETests
        cd WanderlustJournal.E2ETests
        ./bin/Debug/net8.0/playwright.sh install --with-deps chromium
        
    - name: Start WanderlustJournal app in background
      run: |
        cd WanderlustJournal.5
        dotnet run &
        sleep 10 # Wait for app to start
        
    - name: Run E2E tests
      run: dotnet test WanderlustJournal.E2ETests
```

Note: Adjust the .NET version in the CI configuration as needed.
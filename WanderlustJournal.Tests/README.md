# Wanderlust Journal Playwright Tests

This directory contains end-to-end tests for the Wanderlust Journal application using Playwright.

## Prerequisites

- .NET 8.0 SDK
- Playwright browsers (installed automatically during the first test run)

## Running the Tests

1. Make sure the Wanderlust Journal application is built:

```bash
cd WanderlustJournal.5
dotnet build
```

2. Run the tests:

```bash
cd WanderlustJournal.Tests
dotnet test
```

The tests will automatically:
- Start the application on the default port (http://localhost:5026)
- Install Playwright browsers if needed
- Run all the tests
- Stop the application when the tests are complete

## Test Structure

- `HomePageTests.cs` - Tests for the home page
- `JournalEntryTests.cs` - Tests for journal entry functionality (create, view, search)
- `NavigationTests.cs` - Tests for navigation between pages
- `PlaywrightFixture.cs` - Test setup and teardown (starts/stops the application)

## Adding New Tests

1. Create a new class that inherits from `PageTest`
2. Add test methods with the `[TestMethod]` attribute
3. Use the Playwright API to interact with the page

Example:

```csharp
[TestMethod]
public async Task MyNewTest()
{
    await Page.GotoAsync("http://localhost:5026");
    await Page.ClickAsync("text=My Button");
    await Expect(Page.Locator(".result")).ToHaveTextAsync("Success");
}
```

## Debugging Tests

To debug tests, you can set `Headless = false` in the test to see the browser:

```csharp
public override BrowserNewContextOptions ContextOptions()
{
    return new BrowserNewContextOptions
    {
        Headless = false
    };
}
```

## CI/CD Integration

These tests can be integrated into a CI/CD pipeline by running:

```bash
dotnet test WanderlustJournal.Tests
```
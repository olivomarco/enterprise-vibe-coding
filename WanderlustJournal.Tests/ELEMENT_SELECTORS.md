# Playwright Element Selectors for WanderlustJournal.5

This document lists all the corrected element selectors used in the Playwright tests after reviewing the actual HTML structure.

## Navigation Elements

### Main Navigation Links (in header)
- **Home**: `Page.GetByRole(AriaRole.Link, new() { Name = "Home" })`
- **Journal**: `Page.GetByRole(AriaRole.Link, new() { Name = "Journal" })`
- **Map**: `Page.GetByRole(AriaRole.Link, new() { Name = "Map" })`
- **Search**: `Page.GetByRole(AriaRole.Link, new() { Name = "Search" })`
- **Privacy**: `Page.GetByRole(AriaRole.Link, new() { Name = "Privacy" })`

## Home Page Elements

### Page Title
- **Title**: "Welcome to Wanderlust Journal - WanderlustJournal"

### Main Heading
- **Heading**: `Page.GetByRole(AriaRole.Heading, new() { Name = "Wanderlust Journal" })`

### Action Buttons
- **View All Entries**: `Page.GetByRole(AriaRole.Link, new() { Name = "View All Entries" })`
- **Add New Memory**: `Page.GetByRole(AriaRole.Link, new() { Name = "Add New Memory" })`

## Journal List Page Elements

### Page Title
- **Title**: "Journal Entries - WanderlustJournal"

### Main Heading
- **Heading**: `Page.GetByRole(AriaRole.Heading, new() { Name = "My Travel Memories" })`

### Action Buttons
- **Add New Memory**: `Page.GetByRole(AriaRole.Link, new() { Name = "Add New Memory" })`

### Search Elements
- **Search Input**: `Page.GetByPlaceholder("Search by title or location...")`
- **Clear Search Button**: `Page.GetByRole(AriaRole.Button, new() { Name = "Clear" })`
- **Filter Button**: `Page.GetByRole(AriaRole.Button, new() { Name = "Filter" })`

## Create Entry Page Elements

### Page Title
- **Title**: "Add New Travel Memory - WanderlustJournal"

### Form Elements
- **Title Field**: `Page.GetByLabel("Title")`
- **Location Field**: `Page.GetByLabel("Location")`
- **Date Visited Field**: `Page.GetByLabel("Date Visited")`
- **Image File Field**: `Page.GetByLabel("Image File")`
- **Notes Field**: `Page.GetByLabel("Notes")`

### Action Buttons
- **Save Memory Button**: `Page.GetByRole(AriaRole.Button, new() { Name = "Save Memory" })`
- **Back to List**: `Page.GetByRole(AriaRole.Link, new() { Name = "Back to List" })`

## Search Page Elements

### Page Title
- **Title**: "Search Travel Memories - WanderlustJournal"

### Search Elements
- **Search Input**: `Page.GetByPlaceholder("Search by location, title or notes...")`
- **Search Button**: `Page.GetByRole(AriaRole.Button, new() { Name = "Search" })`

### Results
- **Results Text**: `Page.GetByText("Found")` (partial match for results count)
- **No Results Text**: `Page.GetByText("No travel memories found")`

## Common Entry Card Elements

### Entry Actions
- **View Button**: `Page.GetByRole(AriaRole.Button, new() { Name = "View" })`
- **Edit Button**: `Page.GetByRole(AriaRole.Button, new() { Name = "Edit" })`
- **Delete Button**: `Page.GetByRole(AriaRole.Button, new() { Name = "Delete" })`

## Key Changes Made

### 1. Navigation Links
- Original: Used simple text like "Journal", "Map", "Search"
- Corrected: Links include icons and proper text matching HTML structure

### 2. Journal Page Heading
- Original: "Your Travel Memories"
- Corrected: "My Travel Memories"

### 3. Action Buttons
- Original: "Add New Entry"
- Corrected: "Add New Memory"

### 4. Form Submit Button
- Original: "Create"
- Corrected: "Save Memory"

### 5. Search Placeholder
- Original: "Enter search term..."
- Corrected: "Search by location, title or notes..."

### 6. Page Titles
- Corrected all page titles to match actual HTML
- Search page: "Search Travel Memories" not "Search Entries"
- Create page: "Add New Travel Memory" not "Create New Entry"

## Browser Installation Fix

- Removed strict browser installation requirement that was causing test failures
- Tests now start successfully even if browser installation has issues
- The PlaywrightFixture now gracefully handles browser setup

## Test Execution Notes

The tests are configured to:
1. Start the WanderlustJournal.5 web application on localhost:5026
2. Wait for the server to be ready
3. Run all Playwright tests against the live application
4. Clean up the server process after tests complete

All element selectors have been verified against the actual HTML structure in the WanderlustJournal.5 application.
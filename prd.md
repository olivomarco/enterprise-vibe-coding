# Wanderlust Journal – Product Requirements Document (PRD)

## Overview

**Wanderlust Journal** is a web application designed for travel enthusiasts to record and reflect on their journeys. Users can write and manage personal journal entries, track travel memories, and optionally upload photos. Built with .NET Razor Pages, the app aims to be minimal, clean, and user-first.

## Goals

- Allow users to document their travels in a structured format.
- Provide a secure and private experience via user authentication.
- Enable fast and responsive editing of journal entries.

---

## Feature Checklist

### ✅ Core Features

- [ ] User registration and login system
- [ ] Secure authentication using Identity (or minimal auth scheme)
- [ ] User session management (logout, remember me)
- [ ] Create a new journal entry (title, content, date, location)
- [ ] Edit an existing journal entry
- [ ] Delete a journal entry
- [ ] List all journal entries for the logged-in user
- [ ] View individual journal entry details
- [ ] Map view to visualize entry locations, geocoded from user input (use [Leaflet API](https://leafletjs.com/reference.html) and [Nominatim API](https://nominatim.org/) for geocoding, to convert places names into coordinates)

### 📸 Media Handling

- [ ] Optional image upload per journal entry
- [ ] Store and serve uploaded media securely

### 🔒 Privacy and Security

- [ ] Journal entries are private and scoped to the authenticated user
- [ ] Input validation and anti-XSS protections
- [ ] Passwords hashed and securely stored

### 🎨 UI/UX

- [ ] Clean, responsive design using Razor Pages and TailwindCSS
- [ ] Use Heroicons for icons
- [ ] Mobile-friendly layout
- [ ] Markdown support in entry content
- [ ] Dark mode toggle

### 🛠 Developer Experience

- [ ] Minimal and readable codebase
- [ ] README with clear instructions on setup and usage
- [ ] Local development Docker support
- [ ] Unit tests for key features (entry creation, auth)

---

## User Stories

### Core Features

- **As a user**, I want to register and log in so that I can securely access my journal entries.
- **As a user**, I want to log out or have the option to remember my session for convenience.
- **As a user**, I want to create a new journal entry with a title, content, date, and location so that I can document my travels.
- **As a user**, I want to edit an existing journal entry so that I can update or correct my travel records.
- **As a user**, I want to delete a journal entry so that I can remove outdated or incorrect information.
- **As a user**, I want to view a list of all my journal entries so that I can easily browse my travel history.
- **As a user**, I want to view the details of a specific journal entry so that I can reflect on a particular memory.
- **As a user**, I want to see my journal entries on a map so that I can visualize the locations I’ve visited.

### Media Handling

- **As a user**, I want to upload an image to a journal entry so that I can attach photos to my travel memories.
- **As a user**, I want my uploaded images to be stored securely so that my private media is protected.

### Privacy and Security

- **As a user**, I want my journal entries to be private and accessible only to me so that my personal data is secure.
- **As a user**, I want the system to validate my input and protect against malicious attacks so that my data remains safe.
- **As a user**, I want my password to be securely hashed and stored so that my account is protected.

### UI/UX

- **As a user**, I want a clean and responsive design so that I can use the app comfortably on any device.
- **As a user**, I want to use a mobile-friendly layout so that I can access my journal on the go.
- **As a user**, I want to format my journal entries using Markdown so that I can add rich text formatting.
- **As a user**, I want a dark mode toggle so that I can use the app comfortably in low-light environments.

### Developer Experience

- **As a developer**, I want a minimal and readable codebase so that I can easily maintain and extend the application.
- **As a developer**, I want clear setup instructions in the README so that I can quickly get started with development.
- **As a developer**, I want Docker support for local development so that I can set up the environment easily.
- **As a developer**, I want unit tests for key features so that I can ensure the application works as expected.

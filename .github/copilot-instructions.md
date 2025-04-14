# Technical Stack

## .NET 8 Razor Pages (Server-side rendered)

- Use Razor Pages for building server-side rendered web applications to ensure fast initial load times and SEO-friendly content.
- Follow the MVVM (Model-View-ViewModel) pattern to separate concerns and improve maintainability.
- Leverage Tag Helpers and View Components for reusable UI components.
- Enable runtime compilation during development for faster iteration (`Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`).
- Use dependency injection for services and repositories to keep the codebase modular and testable.

## SQLite or PostgreSQL (configurable for local/dev/prod)

- Use SQLite for local development to simplify setup and reduce dependencies.
- Configure PostgreSQL for production environments to leverage its scalability and advanced features.
- Use Entity Framework Core for database access and migrations.
  - Follow a code-first approach for schema management.
  - Use `DbContext` pooling for better performance in high-traffic scenarios.
- Store sensitive connection strings in environment variables or a secure secrets manager.
- Implement database health checks using `IHealthCheck` to monitor connectivity.

## TailwindCSS for styling (optional enhancement)

- Use TailwindCSS for utility-first styling to maintain a consistent design system.
- Configure `tailwind.config.js` to define custom themes, colors, and breakpoints.
- Use PurgeCSS to remove unused styles in production builds for optimal performance.
- Follow a mobile-first design approach to ensure responsiveness.
- Use Tailwind's `@apply` directive for reusable component styles when necessary.

## ASP.NET Identity (or custom auth)

- Use ASP.NET Identity for secure user authentication and authorization.
  - Enable password hashing using the latest algorithms (e.g., PBKDF2, bcrypt).
  - Configure Identity options for stricter password policies and account lockout thresholds.
- For custom authentication, implement middleware to handle token-based or session-based authentication.
- Use `ClaimsPrincipal` for role-based access control (RBAC).
- Secure cookies with `HttpOnly`, `Secure`, and `SameSite` attributes.
- Implement multi-factor authentication (MFA) for enhanced security.

## General Best Practices

- Follow SOLID principles to ensure a clean and maintainable codebase.
- Use logging (use `Serilog`) to capture application events and errors.
- Document APIs and endpoints using tools like Swagger/OpenAPI.

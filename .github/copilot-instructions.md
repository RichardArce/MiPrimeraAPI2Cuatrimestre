# Copilot Instructions

## General Guidelines
- Follow best practices for exception handling and service architecture.
- Maintain a clean and organized code structure.

## Exception Handling
- Do not use try-catch in Repositories or Services. Handle exceptions in the API middleware.

## Architectural Patterns

### Business Logic Layer (BLL)
- Inject repositories through the constructor.
- Perform validations and transformations of DTOs without try-catch.
- Implement notifications and business logic.
- Use methods that return bool to indicate success or failure (failed validations return false).
- Return DTOs instead of entities.

### Data Access Layer (DAL)
- Use methods that return bool: return SaveChangesAsync() > 0 (without try-catch).
- For query methods, use AsNoTracking().
- For single-read methods, return a nullable type (Entity?).
- Use SaveChangesAsync() instead of SaveChanges.

## API Layer
- Ensure that the API layer is responsible for exception handling and not the underlying services or repositories.
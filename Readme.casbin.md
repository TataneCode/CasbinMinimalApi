# Casbin
Casbin is an authorization engine. It lets you define access control models such as RBAC, ABAC, etc. It does not handle authentication.

## Adapter
The adapter defines:
- The authorization model
- The application policies

## Enforcer
The enforcer uses the adapter configuration to validate access levels by username.

### Authorization
The enforcer provides methods to:
- Check permissions on a resource
- Retrieve the roles/policies assigned to a user

### Flexibility
Casbin’s strength is its dynamic customization:
- Add roles or permissions to users at runtime
- Add permissions on resources at runtime

## Implementation

### Configuration
Several approaches:
- Manually via an endpoint
- A one‑time hosted service
- Directly in application startup

Policies does not need to be initialized, but the model is required for the configuration.

Chosen approach: policies persisted in the database with an EF Core adapter.

### Usage
Likewise, multiple integration options:
- Via annotations/attributes
- Inside controllers or services
- In middleware

You can combine approaches pragmatically.
⚠ Casbin does not manage authentication. Protect controllers/endpoints separately.

## Official Documentation
- [Casbin RBAC Configuration](https://casbin.org/docs/rbac)
- [Casbin policy storage + adapter api](https://casbin.org/docs/policy-storage)

- [EFCore adapter for Casbin](https://github.com/casbin-net/EFCore-Adapter)

- [Policy editor](https://editor.casbin.org/)
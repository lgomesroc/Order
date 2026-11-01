# Swagger exposure

Swagger is disabled by default in the API's base configuration. Enable it explicitly only in environments where interactive API documentation is intended:

- Development: `Swagger:Enabled=true` is configured in `appsettings.Development.json`.
- Other environments: set the environment variable `Swagger__Enabled=true` only when public or private Swagger access is an intentional deployment decision.

For production deployments, keep the flag disabled unless the API documentation endpoint is deliberately exposed and protected by the deployment's access controls.

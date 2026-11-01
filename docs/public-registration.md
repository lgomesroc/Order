# Public user registration

The anonymous `POST /api/user` endpoint is controlled by `Registration:PublicEnabled`.

- The base configuration disables public registration.
- Development explicitly enables it for local setup and demonstrations.
- For a deployed demo that needs account creation, set `Registration__PublicEnabled=true` deliberately. Otherwise keep the endpoint disabled; existing users can continue to authenticate.

When disabled, the anonymous registration endpoint responds with HTTP 404 and does not invoke the user application.

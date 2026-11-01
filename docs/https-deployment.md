# HTTPS deployment

The API and Razor Pages only enable HTTPS redirection and HSTS when `HttpsRedirection:Enabled` is true.

## Local Docker Compose

The container environment uses HTTP internally and publishes the configured host ports. Keep the setting disabled for the local Compose environment.

## Production

When the application terminates TLS itself, set `HttpsRedirection__Enabled=true` for both applications and configure the HTTPS endpoint/certificate.

When TLS is terminated by a reverse proxy or ingress, configure HTTPS redirection and HSTS at that trusted edge. Only enable application-level redirection when forwarded headers and proxy trust are configured correctly.

The default is false so an HTTP-only container does not emit redirects to an HTTPS endpoint that is not listening.

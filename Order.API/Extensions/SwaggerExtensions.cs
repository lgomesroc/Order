using Microsoft.OpenApi.Models;

namespace Order.Extensions
{
    public static class SwaggerExtensions
    {
        public static void SwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ProgrameVC",
                    Description = "API order",
                    TermsOfService = new Uri("https://example.com/terms")
                });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "Bearer token",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                // Se SecurityRequirementsOperationFilter for uma classe comum, não é necessário registrar como serviço aqui.
                // c.OperationFilter<SecurityRequirementsOperationFilter>();

                var xmlApiPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(IStartup).Assembly.GetName().Name}.xml");

                c.IncludeXmlComments(xmlApiPath);
            });
        }
    }
}


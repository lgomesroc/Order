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
                    Title = "Order",
                    Description = "Order management API"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Order.Api.xml");
                if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
            });
        }
    }
}

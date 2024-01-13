using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Order.Domain.Interfaces.Repositories.DataConnector;
using System.Text;
using Order.Extensions;

var builder = WebApplication.CreateBuilder(args);
var authSettingsSection = builder.Configuration.GetSection("AuthSettings");
var authSettings = authSettingsSection.Get<AuthSettings>();
var app = builder.Build();
string connectionString = builder.Configuration.GetConnectionString("default");


builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Core));
builder.Services.Configure<AuthSettings>(authSettingsSection);
builder.Services.AddScoped<IDbConnector>(db => new SqlConnector(connectionString));
builder.Services.RegisterIoC();
builder.Services.SwaggerConfiguration();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddPolicyScheme("programevc", "Authorization Bearer or AccessToken", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Headers["Access-Token"].Any())
        {
            return "Access-Token";
        }

        return JwtBearerDefaults.AuthenticationScheme;
    };
}).AddJwtBearer(x =>
{
    SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret));

    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = "programevc",

        ValidateAudience = false,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,

        // Verify if token is valid
        ValidateLifetime = true,
        RequireExpirationTime = true,
    };
});

app.UseSwagger();
app.UseSwaggerUI(setup =>
{
    setup.RoutePrefix = "swagger";
    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Documentation");
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class Core
{
}

public class AuthSettings
{
    public string Secret { get; set; }
}
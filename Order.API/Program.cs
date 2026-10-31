using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Order.Application.Applications;
using Order.Application.Interfaces;
using Order.Application.Interfaces.Security;
using Order.Application.Mapper;
using Order.Application.Models;
using Order.Application.Security;
using Order.Domain.Common;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Interfaces.Services;
using Order.Domain.Services;
using Order.Extensions;
using Order.Infra.DataConnector;
using Order.Infra.Repositories;
using TimeProvider = Order.Domain.Common.TimeProvider;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("default")
    ?? throw new InvalidOperationException("Connection string 'default' was not configured.");
var authSettingsSection = builder.Configuration.GetSection("AuthSettings");
var authSettings = authSettingsSection.Get<AuthSettings>()
    ?? throw new InvalidOperationException("AuthSettings configuration was not found.");
if (string.IsNullOrWhiteSpace(authSettings.Secret))
    throw new InvalidOperationException("AuthSettings:Secret must be configured.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Core));
builder.Services.Configure<AuthSettings>(authSettingsSection);
builder.Services.SwaggerConfiguration();
builder.Services.AddScoped<IDbConnector>(_ => new SqlConnector(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITimeProvider, TimeProvider>();
builder.Services.AddScoped<IGenerators, Generators>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IClientApplication, ClientApplication>();
builder.Services.AddScoped<IOrderApplication, OrderApplication>();
builder.Services.AddScoped<IProductApplication, ProductApplication>();
builder.Services.AddScoped<IUserApplication, UserApplication>();
builder.Services.AddScoped<ITokenManager, TokenManager>();

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "programevc",
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateLifetime = true,
        RequireExpirationTime = true
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
});
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

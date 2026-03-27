using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// ── JWT Authentication ────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("authenticated", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("allow_all", policy => policy.RequireAssertion(_ => true));

// ── Rate Limiting ──────────────────────────────────────────────
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ── YARP ──────────────────────────────────────────────────────
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(context =>
    {
        // Chuyển tiếp claims của user xuống downstream services
        context.AddRequestTransform(async transformContext =>
        {
            var user = transformContext.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst("sub")?.Value
                          ?? user.FindFirst("id")?.Value;
                var email = user.FindFirst("email")?.Value;
                var role = user.FindFirst("role")?.Value;

                if (userId != null)
                    transformContext.ProxyRequest.Headers
                        .TryAddWithoutValidation("X-User-Id", userId);
                if (email != null)
                    transformContext.ProxyRequest.Headers
                        .TryAddWithoutValidation("X-User-Email", email);
                if (role != null)
                    transformContext.ProxyRequest.Headers
                        .TryAddWithoutValidation("X-User-Role", role);
            }
            await ValueTask.CompletedTask;
        });
    });

// ── CORS ───────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
                  "http://localhost:3000",  // React/Next.js dev
                  "https://yourdomain.com") // Production
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ── Health checks ──────────────────────────────────────────────
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseIpRateLimiting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();
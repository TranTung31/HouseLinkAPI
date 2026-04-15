using HouseLink.Identity.Domain.Interfaces;
using HouseLink.Identity.Infrastructure.Persistence;
using HouseLink.Identity.Infrastructure.Repositories;
using HouseLink.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HouseLink.Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── EF Core ───────────────────────────────────────────
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(
                    configuration.GetConnectionString("Default"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // ── ASP.NET Identity ──────────────────────────────────
            services.AddIdentity<AppIdentityUser, IdentityRole>(opt =>
            {
                // Password policy
                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;

                // Lockout policy
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.AllowedForNewUsers = true;

                // User policy
                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ── Services ──────────────────────────────────────────
            //services.AddScoped<IIdentityService, IdentityService>();
            //services.AddScoped<IJwtTokenService, JwtTokenService>();
            //services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<IKafkaProducerService, KafkaProducerService>();

            // Consumer Service - Đăng ký như hosted service
            services.AddHostedService<KafkaConsumerService>();

            return services;
        }
    }
}

using System.Net;
using System.Net.Mail;
using System.Text;

using Loqora.Application.Common.Interfaces;
using Loqora.Infrastructure.Data;
using Loqora.Infrastructure.Data.Interceptors;
using Loqora.Infrastructure.Identity;
using Loqora.Infrastructure.Services.Email;
using Loqora.Infrastructure.Settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Loqora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddIdentity()
            .AddDatabase(configuration)
            .AddJwt(configuration)
            .AddFluentEmail(configuration)
            .AddCaching();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<AppDbContext>(
        (sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
           .AddIdentityCore<AppUser>(options =>
           {
               options.Password.RequiredLength = Domain.Identity.IdentityConstants.PasswordMinLength;
               options.Password.RequireDigit = true;
               options.Password.RequireNonAlphanumeric = true;
               options.Password.RequireUppercase = true;
               options.Password.RequireLowercase = true;
               options.Password.RequiredUniqueChars = 4;

               options.SignIn.RequireConfirmedAccount = true;
               options.SignIn.RequireConfirmedEmail = true;

               options.Lockout.MaxFailedAccessAttempts = 5;
               options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
               options.Lockout.AllowedForNewUsers = true;
           })
           .AddRoles<AppRole>()
           .AddSignInManager()
           .AddEntityFrameworkStores<AppDbContext>()
           .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IIdentityLinkGenerator, IdentityLinkGenerator>();

        return services;
    }

    private static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret!)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        services.AddAuthorizationBuilder();

        services.AddScoped<ITokenProvider, TokenProvider>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
              options.DefaultEntryOptions = new HybridCacheEntryOptions
              {
                  Expiration = TimeSpan.FromMinutes(10), // L2, L3
                  LocalCacheExpiration = TimeSpan.FromSeconds(30), // L1
              });

        return services;
    }

    private static IServiceCollection AddFluentEmail(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

        var emailOptions = configuration.GetSection(EmailOptions.SectionName).Get<EmailOptions>()!;

        services.AddFluentEmail(emailOptions.FromEmail, emailOptions.FromName)
                .AddRazorRenderer()
                .AddSmtpSender(new SmtpClient(emailOptions.SmtpServer, emailOptions.SmtpPort)
                {
                    Host = emailOptions.SmtpServer,
                    Port = emailOptions.SmtpPort,
                    Credentials = new NetworkCredential(emailOptions.SmtpUsername, emailOptions.SmtpPassword),
                    EnableSsl = true
                });

        services.AddScoped<IEmailService, FlunetEmailService>();

        return services;
    }
}

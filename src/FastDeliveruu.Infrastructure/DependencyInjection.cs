using System.Text;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FastDeliveruu.Infrastructure.Services;
using FastDeliveruu.Infrastructure.Common;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Infrastructure.UnitOfWork;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Domain.Identity.CustomManagers;
using FastDeliveruu.Application.Common.Helpers;
using FastDeliveruu.Infrastructure.Seed.Seeders;

namespace FastDeliveruu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddSignalR();

        services.AddIdentity();
        services.AddAuth(configuration);

        services.AddScoped<IFastDeliveruuUnitOfWork, FastDeliveruuUnitOfWork>();
        services.AddScoped<IOnlineTrackerService, OnlineTrackerService>();

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Cache"));
        services.AddSingleton<ICacheService, CacheService>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // register email service
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddSingleton<IEmailSender, EmailSender>();

        // register geocoding service
        services.Configure<OpenCageSettings>(configuration.GetSection(OpenCageSettings.SectionName));
        services.AddSingleton<IGeocodingService, GeocodingService>();

        // register cloudinary service
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
        services.AddSingleton<IFileStorageServices, FileStorageServices>();

        // register sms service
        services.Configure<SmsSettings>(configuration.GetSection(SmsSettings.SectionName));
        services.AddSingleton<ISmsSenderService, SmsSenderService>();

        // register vnpay
        services.Configure<VnpaySettings>(configuration.GetSection(VnpaySettings.SectionName));
        services.AddSingleton<IVnpayServices, VnpayServices>();

        // register paypal
        services.AddSingleton(x => new PaypalClient(
            configuration["Payment:Paypal:AppId"],
            configuration["Payment:Paypal:AppSecret"],
            configuration["Payment:Paypal:Mode"]
        ));

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 3;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<FastDeliveruuDbContext>()
        .AddSignInManager<SignInManager<AppUser>>()
        .AddRoleManager<RoleManager<AppRole>>() //.AddRoleManager<RoleManager<IdentityRole<Guid>>>()
        .AddRoleValidator<RoleValidator<AppRole>>()
        .AddDefaultTokenProviders();

        services.AddIdentityCore<Shipper>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 3;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<FastDeliveruuDbContext>()
        .AddSignInManager<SignInManager<Shipper>>();
        //.AddDefaultTokenProviders(); // will generate the error: No IUserTwoFactorTokenProvider named 'Default' is registered.
        services.AddTransient<ShipperManager>();

        // Add Seedata
        services.AddTransient<IDataSeeder, UserRoleSeeder>();

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        JwtSettings jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        // services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyConstants.ManageResources, policy =>
                policy.RequireRole(RoleConstants.Admin, RoleConstants.Staff));
            options.AddPolicy(PolicyConstants.RequiredCustomerShipper, policy =>
                policy.RequireRole(RoleConstants.Customer, RoleConstants.Shipper));

            options.AddPolicy(PolicyConstants.RequiredAdmin, policy =>
                policy.RequireRole(RoleConstants.Admin));
            options.AddPolicy(PolicyConstants.RequiredStaff, policy =>
                policy.RequireRole(RoleConstants.Staff));
            options.AddPolicy(PolicyConstants.RequiredShipper, policy =>
                policy.RequireRole(RoleConstants.Shipper));
        });

        return services;
    }
}

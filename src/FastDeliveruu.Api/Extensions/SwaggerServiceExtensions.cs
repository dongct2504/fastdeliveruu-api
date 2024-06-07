using Microsoft.OpenApi.Models;

namespace FastDeliveruu.Api.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocument(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using bearer scheme.\r\n\r\n" +
                    "Enter 'Bearer' [space] and your token in the input text below.\r\n\r\n" +
                    "Example: \"Bearer 123456eqrt\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
            });

            options.SwaggerDoc("v1", new OpenApiInfo // 'v1' name must match the SwaggerEndpoint bellow
            {
                Version = "1.0",
                Title = "FastDeliveruu API v1",
                TermsOfService = new Uri("https://example.com/terms"),
                License = new OpenApiLicense
                {
                    Name = "License",
                    Url = new Uri("https://example.com/license")
                }
            });

            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "2.0",
                Title = "FastDeliveruu API v2",
                TermsOfService = new Uri("https://example.com/terms"),
                License = new OpenApiLicense
                {
                    Name = "License",
                    Url = new Uri("https://example.com/license")
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocument(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FastDeliveruu API v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "FastDeliveruu API v2");
        });

        return app;
    }
}

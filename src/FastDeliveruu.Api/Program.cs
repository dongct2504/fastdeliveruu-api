using AutoMapper;
using FastDeliveruu.Api.Services;
using FastDeliveruu.Application;
using FastDeliveruu.Infrastructure;
using FastDeliveruu.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Application.Profiles;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.

    // allows for the correct parsing of Patch document using NewtonsoftJson
    builder.Services.AddResponseCaching();

    builder.Services.AddControllers(options =>
    {
        options.CacheProfiles
            .Add("Default30", new CacheProfile
            {
                Duration = 30
            });
    })
        .AddNewtonsoftJson()
        .AddXmlDataContractSerializerFormatters();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
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
        options.SwaggerDoc("v1", new OpenApiInfo // 'v1' name must match the swagger endpoint bellow
        {
            Version = "1.0",
            Title = "FastDeliveruu V1",
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
            Title = "FastDeliveruu V2",
            TermsOfService = new Uri("https://example.com/terms"),
            License = new OpenApiLicense
            {
                Name = "License",
                Url = new Uri("https://example.com/license")
            }
        });
    });

    // setting connection string and register DbContext
    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
    {
        ConnectionString = builder.Configuration.GetConnectionString("FastdeliveruuSqlConnection"),
        UserID = builder.Configuration["UserID"],
        Password = builder.Configuration["Password"]
    };

    builder.Services.AddDbContext<FastDeliveruuContext>(options =>
        options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString));

    // register services in other layers
    builder.Services.AddScoped<IImageServices, ImageServices>();
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom
        .Configuration(builder.Configuration)
        .CreateLogger();

    builder.Host.UseSerilog();

    // register automapper
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.EnableNullPropagationForQueryMapping = true;
    }, typeof(FastDeliveruuProfile).Assembly);

    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true; // report the support version in the response headers
    });
    builder.Services.AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true; // automatically change to the version (v1)
    });
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FastDeliveruuApiV1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "FastDeliveruuApiV2");
        });
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseResponseCaching();

    app.UseStaticFiles(); // upload images

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
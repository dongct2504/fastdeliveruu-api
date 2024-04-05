using AutoMapper;
using FastDeliveruu.Api.Interfaces;
using FastDeliveruu.Api.Services;
using FastDeliveruu.Application;
using FastDeliveruu.Infrastructure;
using FastDeliveruu.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.

    // allows for the correct parsing of Patch document using NewtonsoftJson
    builder.Services.AddControllers()
        .AddNewtonsoftJson()
        .AddXmlDataContractSerializerFormatters();

    builder.Services.AddAuthentication(options =>
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
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                    builder.Configuration["ApiSettings:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

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
    });

    // setting connection string and register DbContext
    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
    {
        ConnectionString = builder.Configuration.GetConnectionString("FastdeliveruuSqlConnection"),
        UserID = builder.Configuration["UserID"],
        Password = builder.Configuration["Password"]
    };

    builder.Services.AddDbContext<FastdeliveruuContext>(options =>
        options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString));

    // register services in other layers
    builder.Services.AddScoped<IImageServices, ImageServices>();
    builder.Services
        .AddApplication()
        .AddInfrastructure();

    // register automapper
    builder.Services.AddAutoMapper(typeof(Program));
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles(); // upload images

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}

app.Run();
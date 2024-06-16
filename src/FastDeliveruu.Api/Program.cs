using FastDeliveruu.Application;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FastDeliveruu.Api.Middleware;
using Asp.Versioning;
using FastDeliveruu.Infrastructure;
using FastDeliveruu.Api.Extensions;
using FastDeliveruu.Infrastructure.Identity;
using FastDeliveruu.Domain.Data;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerDocument();
    
    // setting connection string and register DbContext
    var defaultSqlConnectionStringBuilder = new SqlConnectionStringBuilder
    {
        ConnectionString = builder.Configuration.GetConnectionString("FastdeliveruuSqlConnection"),
        UserID = builder.Configuration["UserID"],
        Password = builder.Configuration["Password"]
    };

    builder.Services.AddDbContext<FastDeliveruuDbContext>(options =>
        options.UseSqlServer(defaultSqlConnectionStringBuilder.ConnectionString));

    var identitySqlConnectionStringBuilder = new SqlConnectionStringBuilder
    {
        ConnectionString = builder.Configuration.GetConnectionString("IdentitySqlConnection"),
        UserID = builder.Configuration["UserID"],
        Password = builder.Configuration["Password"]
    };

    builder.Services.AddDbContext<FastDeliveruuIdentityDbContext>(options =>
        options.UseSqlServer(identitySqlConnectionStringBuilder.ConnectionString));

    // register services in other layers
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom
        .Configuration(builder.Configuration)
        .CreateLogger();

    builder.Host.UseSerilog();

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true; // report the support version in the response headers
        options.AssumeDefaultVersionWhenUnspecified = true;
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true; // automatically change to the version (v1)
        options.AddApiVersionParametersWhenVersionNeutral = true;
    });

    builder.Services.AddCors();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerDocument();
    }

    //app.UseHttpsRedirection();

    using IServiceScope serviceScope = app.Services.CreateScope();

    using FastDeliveruuIdentityDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<FastDeliveruuIdentityDbContext>();

    dbContext.Database.Migrate();

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseSerilogRequestLogging();

    app.UseResponseCaching();

    app.UseCors(policy => policy.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:4200"));

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
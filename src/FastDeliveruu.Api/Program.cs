using FastDeliveruu.Application;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FastDeliveruu.Api.Middleware;
using Asp.Versioning;
using FastDeliveruu.Infrastructure;
using FastDeliveruu.Api.Extensions;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Infrastructure.Services;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Infrastructure.Hubs;
using FastDeliveruu.Infrastructure.SeedData.Seeders;
using FastDeliveruu.Infrastructure.SeedData;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerDocument();

    // setting connection string and register DbContext
    //var defaultSqlConnectionStringBuilder = new SqlConnectionStringBuilder
    //{
    //    ConnectionString = builder.Configuration.GetConnectionString("FastdeliveruuSqlConnection"),
    //    UserID = builder.Configuration["UserID"],
    //    Password = builder.Configuration["Password"]
    //};

    builder.Services.AddDbContext<FastDeliveruuDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("FastdeliveruuSqlConnection")));

    //var identitySqlConnectionStringBuilder = new SqlConnectionStringBuilder
    //{
    //    ConnectionString = builder.Configuration.GetConnectionString("IdentitySqlConnection"),
    //    UserID = builder.Configuration["UserID"],
    //    Password = builder.Configuration["Password"]
    //};
    //builder.Services.AddDbContext<FastDeliveruuIdentityDbContext>(options =>
    //    options.UseSqlServer(identitySqlConnectionStringBuilder.ConnectionString));

    // register services in other layers
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    // configure Serilog
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.With(new VietnamDateTimeEnricher(services.GetRequiredService<IDateTimeProvider>()));
    });

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

    app.UseHttpsRedirection();

    using (IServiceScope serviceScope = app.Services.CreateScope())
    {
        var services = serviceScope.ServiceProvider;
        var dbContext = services.GetRequiredService<FastDeliveruuDbContext>();
        var seeders = services.GetServices<IDataSeeder>();
        await dbContext.Database.MigrateAsync();
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await SeedData.InitializeAsync(dbContext, seeders);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseSerilogRequestLogging();

    app.UseResponseCaching();

    app.UseCors(policy => policy
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:4200"));

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHub<OnlineHub>("hubs/online");
    app.MapHub<ChatHub>("hubs/chat");
    app.MapHub<NotificationHub>("hubs/notification");

    app.Run();
}
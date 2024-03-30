using AutoMapper;
using FastDeliveruu.Application;
using FastDeliveruu.Infrastructure;
using FastDeliveruu.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.

    // allows for the correct parsing of Patch document using NewtonsoftJson
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

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

    app.UseAuthorization();

    app.MapControllers();
}

app.Run();
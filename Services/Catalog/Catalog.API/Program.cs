using Catalog.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseSettings"));

        builder.Services.AddSingleton<IMongoClient>(conf =>
        {
            var settings = conf.GetRequiredService<IOptions<DatabaseConfiguration>>()?.Value;
            
            return new MongoClient(settings?.ConnectionString);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }
}
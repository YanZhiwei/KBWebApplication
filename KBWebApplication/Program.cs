using System.Diagnostics.CodeAnalysis;
using KBWebApplication.Extensions;
using KBWebApplication.Plugins;

namespace KBWebApplication;

public class Program
{
    [Experimental("SKEXP0001")]
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
            .Build();


        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSemanticKernel(configBuilder);
        builder.Services.AddSemanticTextMemory(configBuilder);
        builder.Services.AddSingleton<KbTextMemoryPlugin>();
        var app = builder.Build();
        //KbTextMemoryPlugin kbTextMemoryPlugin= app.Services.GetService<KbTextMemoryPlugin>();
        //kbTextMemoryPlugin.ImportAsync();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
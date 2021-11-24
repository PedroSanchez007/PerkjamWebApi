using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Perkjam.API.Entities;
using Serilog;

namespace Perkjam.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogger();
            
            var host = BuildWebHost(args);
            
            Log.Information("Web Host built");

            // migrate & seed the database.  Best practice = in Main, using service scope
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetService<PerkContext>();                   
                    // migrate & seed
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while migrating or seeding the database.");
                }
            }

            // run the web app
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        
        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
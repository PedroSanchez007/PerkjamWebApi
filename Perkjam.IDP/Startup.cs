// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Perkjam.IDP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Perkjam.IDP.Quickstart;

namespace Perkjam.IDP
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var perkjamIdpDataDBConnectionString = "Server=(localdb)\\mssqllocaldb;Database=PerkjamIdpDataDB;Trusted_Connection=True;";
            
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer()
                // .AddInMemoryIdentityResources(Config.Ids)
                // .AddInMemoryApiResources(Config.Apis)
                // .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            builder.AddDeveloperSigningCredential();
            //builder.AddSigningCredential(LoadCertificateFromStore());
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            
            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => 
                    builder.UseSqlServer(perkjamIdpDataDBConnectionString,options => options.MigrationsAssembly(migrationsAssembly));
            });
            
            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => 
                    builder.UseSqlServer(perkjamIdpDataDBConnectionString,options => options.MigrationsAssembly(migrationsAssembly));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            InitializeDatabase(app);

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
        
        public X509Certificate2 LoadCertificateFromStore()
        {
            var thumbPrint = "30f4330715891dc4fa8bc9b6565925f1e2095b5e";

            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint,thumbPrint, true);
            if (certCollection.Count == 0)
            {
                throw new Exception("The specified certificate wasn't found.");
            }
            return certCollection[0];
        }
        
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}

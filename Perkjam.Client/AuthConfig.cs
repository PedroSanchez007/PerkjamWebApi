namespace Perkjam.Client
{
    using System;
    using System.IO;
    using System.Globalization;
    using Microsoft.Extensions.Configuration;

    public class AuthConfig
    {
        public string Instance {get; set;} = "https://login.microsoftonline.com/";
        public string TenantId {get; set;}
        public string ClientId {get; set;}
        public string Authority => $"{Instance}{TenantId}";
        public string ClientSecret {get; set;}
        public string BaseAddress {get; set;}
        public string ResourceId {get; set;}

        public static AuthConfig ReadFromJsonFile(string path)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

            IConfiguration configuration = builder.Build();

            return configuration.Get<AuthConfig>();
        }
    }
}
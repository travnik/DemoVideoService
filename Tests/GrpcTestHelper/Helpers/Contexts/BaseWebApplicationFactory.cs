using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace GrpcTestHelper.Helpers.Contexts
{
    public class BaseWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        protected IEnumerable<KeyValuePair<string, string>> _properties;
        public readonly List<string> JsonFiles = new List<string>();

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureAppConfiguration(AddJsonFiles);
            builder.UseSolutionRelativeContentRoot("", "*.sln");
            AddProperties(builder);
        }

        private void AddJsonFiles(IConfigurationBuilder configBuilder)
        {
            if (JsonFiles == null)
            {
                return;
            }

            FileConfigurationExtensions.SetBasePath(configBuilder, Directory.GetCurrentDirectory());
            foreach (var jsonFile in JsonFiles)
            {
                configBuilder.AddJsonFile(jsonFile);
            }
        }

        private void AddProperties(IWebHostBuilder builder)
        {
            if (_properties == null)
            {
                return;
            }
            builder.ConfigureAppConfiguration((_, configurationBuilder) => 
                configurationBuilder.AddInMemoryCollection(_properties));
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace RankVotingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddIniFile("kafkaClient.properties", optional: false, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
					var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
					webBuilder.UseUrls($"http://0.0.0.0:{port}");
					webBuilder.UseStartup<Startup>();
                });
    }
}

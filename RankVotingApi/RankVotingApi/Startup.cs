using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RankVotingApi.Repository;
using RankVotingApi.Votes;
using System;

namespace RankVotingApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        private static ServiceProvider CreateServices()
        {
            return new ServiceCollection()                
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb                    
                    .AddSQLite()                    
                    .WithGlobalConnectionString("Data Source=RankChoiceVoting.db")                    
                    .ScanIn(typeof(Migration_20210609131700_AddLogTable).Assembly).For.Migrations())                
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }

            services.AddCors(o => o.AddPolicy("AllowEverything", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            
            services.AddRouting(options => 
            { 
                options.LowercaseUrls = true; 
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RankVotingApi", Version = "v1" });
            });

            services.AddScoped<IVoteBusiness, VoteBusiness>();
            services.AddScoped<IVoteRepository, VoteRepository>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RankVotingApi v1"));
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowEverything");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/health", () => Results.Ok());
            });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Okta.AspNetCore;
using System;

namespace APIOkta
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Similiarly we can add the health checks for Kafka
            services.AddHealthChecks()
                .AddUrlGroup(new Uri("<url here>"), name: "base URL", failureStatus: HealthStatus.Degraded)
                .AddDbContextCheck<EmployeeDbContext>(tags: new[] { "Read" }, failureStatus: HealthStatus.Degraded)
                .AddCheck<MemoryHealthCheck>("Memory", failureStatus: HealthStatus.Degraded, new[] { "Read" })
                ;
            services.AddControllers();
            services.AddDbContext<EmployeeDbContext>(x => x.UseSqlServer("<Connection String Here>"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = (x) => x.Tags.Contains("Read")
                });
                endpoints.MapHealthChecks("/api/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = (x) => false
                });
                endpoints.MapControllers();
            });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using TestTarget.Services;

namespace TestTarget
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
            services.AddControllers();
            services.AddHealthChecks()
                .AddCheck<TestHealthCheck>("app is ready", failureStatus: HealthStatus.Degraded, new[] { "appReady" })
                .AddTypeActivatedCheck<TestHealthCheckv2>("app is live", failureStatus: HealthStatus.Degraded, tags: new[] { "appLive" }, args: new object[] { 200 })
                ;
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/appReady", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("appReady")
                });
                endpoints.MapHealthChecks("/appLive", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("appLive")
                });
            });
        }
    }
}

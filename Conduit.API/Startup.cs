using Conduit.API.Extensions;

namespace Conduit.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRateLimiting(Configuration);
            services.AddApplicationServices(Configuration);
            services.AddSwaggerDocumentation();
            services.AddQuartzServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApplicationPipeline(env);
        }
    }
}

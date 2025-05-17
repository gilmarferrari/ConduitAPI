using Conduit.Application.Helpers;
using Conduit.Application.Services;
using Conduit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Conduit.API.Extensions
{
    public static class ServicesConfig
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ConduitContext>(options => options.UseSqlServer(EnvironmentVariables.GetEnvironmentVariable(VariableType.Database)));

            services.AddSingleton(configuration);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors();
            services.AddSignalR(e => { e.MaximumReceiveMessageSize = 102400000; });
            services.AddHttpContextAccessor();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IMessagingService, MessagingService>();
            services.AddScoped<ISignalRService, SignalRService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserAccountService, UserAccountService>();
        }
    }
}

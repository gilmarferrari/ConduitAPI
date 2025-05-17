using Conduit.Application.Jobs;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;

namespace Conduit.API.Extensions
{
    public static class QuartzConfig
    {
        public static void AddQuartzServices(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<NotificationJob>();
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(NotificationJob), "Notification Job", "0/10 * * * * ?"));
            services.AddHostedService<Application.Jobs.QuartzHostedService>();

            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();

            services.AddSingleton(scheduler);
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}

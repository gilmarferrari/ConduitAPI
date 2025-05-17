using Conduit.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Conduit.Application.Jobs
{
    [DisallowConcurrentExecution]
    public class NotificationJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailService _emailService;
        private readonly ISignalRService _signalRService;

        public NotificationJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _emailService = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IEmailService>();
            _signalRService = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISignalRService>();

            // Run job to clear all SignalR clients
            _signalRService.ClearClients();
        }

        public Task Execute(IJobExecutionContext context)
        {
            if (!context.MergedJobDataMap.IsEmpty)
            {
                var dataMap = context.MergedJobDataMap;
                var type = (JobType)(dataMap["type"] ?? JobType.None);

                switch (type)
                {
                    case JobType.Email:
                        break;
                    case JobType.PushNotification:
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
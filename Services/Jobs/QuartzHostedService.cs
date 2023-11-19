using Microsoft.Extensions.Hosting;
using Quartz.Spi;
using Quartz;

namespace Services.Jobs
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly JobMetadata _jobMetadata;
        public IScheduler _scheduler { get; set; }

        public QuartzHostedService(ISchedulerFactory schedulerFactory, JobMetadata jobMetadata, IJobFactory jobFactory)
        {
            _schedulerFactory = schedulerFactory;
            _jobMetadata = jobMetadata;
            _jobFactory = jobFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _jobFactory;
            var job = CreateJob(_jobMetadata);
            var trigger = CreateTrigger(_jobMetadata);
            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler?.Shutdown(cancellationToken);
        }

        private ITrigger CreateTrigger(JobMetadata jobMetadata)
        {
            return TriggerBuilder.Create()
            .WithIdentity(jobMetadata.JobID.ToString())
            .WithCronSchedule(jobMetadata.CronExpression)
            .WithDescription($"{jobMetadata.JobName}")
            .Build();
        }

        private IJobDetail CreateJob(JobMetadata jobMetadata)
        {
            return JobBuilder
            .Create(jobMetadata.JobType)
            .WithIdentity(jobMetadata.JobID.ToString())
            .WithDescription($"{jobMetadata.JobName}")
            .Build();
        }
    }
}

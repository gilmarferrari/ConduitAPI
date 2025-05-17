using Conduit.Application.Jobs;
using Quartz;

namespace Conduit.Application.Services
{
    public interface IJobService
    {
        Task CreateJob(JobType type);
        Task ScheduleJob(JobType type, DateTime startDate);
    }

    public class JobService : IJobService
    {
        private readonly IScheduler _scheduler;

        public JobService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task CreateJob(JobType type)
        {
            try
            {
                IJobDetail job = JobBuilder.Create<NotificationJob>()
                    .WithIdentity($"job-{DateTime.UtcNow.Ticks}")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"trigger-{DateTime.UtcNow.Ticks}", "group")
                    .StartNow()
                    .Build();

                switch (type)
                {
                    case JobType.Email:
                        job.JobDataMap.Put("type", JobType.Email);
                        break;
                    case JobType.PushNotification:
                        job.JobDataMap.Put("type", JobType.PushNotification);
                        break;
                }

                await _scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task ScheduleJob(JobType type, DateTime startDate)
        {
            try
            {
                IJobDetail job = JobBuilder.Create<NotificationJob>()
                    .WithIdentity($"{type}-job").Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger", "group")
                    .StartAt(new DateTimeOffset(startDate))
                    .Build();

                switch (type)
                {
                    case JobType.Email:
                        job.JobDataMap.Put("type", JobType.Email);
                        break;
                    case JobType.PushNotification:
                        job.JobDataMap.Put("type", JobType.PushNotification);
                        break;
                }

                await _scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public enum JobType
    {
        None = 0,
        Email = 1,
        PushNotification = 2
    }
}

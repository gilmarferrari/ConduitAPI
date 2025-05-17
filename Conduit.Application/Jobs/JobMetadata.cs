namespace Conduit.Application.Jobs
{
    public class JobMetadata
    {
        public Guid JobID { get; set; }
        public Type JobType { get; }
        public string JobName { get; }
        public string CronExpression { get; }
        public JobMetadata(Guid id, Type jobType, string jobName, string cronExpression)
        {
            JobID = id;
            JobType = jobType;
            JobName = jobName;
            CronExpression = cronExpression;
        }
    }
}

using Quartz;
using ScheduleQuartz.SchedulerQtz;
using System;

namespace ScheduleQuartz.Extensions
{
    public static class QuartzExtension
    {
        public static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            JobBuilder jobBuilder = JobBuilder.Create(jobType);

            if (!string.IsNullOrEmpty(schedule.GroupName))
                jobBuilder.WithIdentity(schedule.Name, schedule.GroupName);
            else
                jobBuilder.WithIdentity(schedule.Name);

            if (!string.IsNullOrEmpty(schedule.Description))
                jobBuilder.WithDescription(schedule.Description);


            return jobBuilder.StoreDurably().Build();
        }

        public static ITrigger CreateTrigger(JobSchedule schedule)
        {
            if (!CronExpression.IsValidExpression(schedule.CronExpression))
                throw new Exception("CronExpression is not valid");
            
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.Name}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }

        public static ITrigger CreateTriggerForJob(string name , string cronExpression , IJobDetail jobDetail)
        {
            if (!CronExpression.IsValidExpression(cronExpression))
                throw new Exception("CronExpression is not valid");

            return TriggerBuilder
               .Create()
               .WithIdentity($"{name}.trigger")
               .WithCronSchedule(cronExpression)
               .WithDescription(cronExpression)
               .ForJob(jobDetail.Key)
               .Build();
        }
    }
}

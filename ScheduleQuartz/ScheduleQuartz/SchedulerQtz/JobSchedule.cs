using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleQuartz.SchedulerQtz
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
            Name = jobType.Name;
        }

        public JobSchedule()
        {
        }

        public Type JobType { get; set; }

        /// <summary>
        /// Documents CronExpression : http://www.quartz-scheduler.org/documentation/quartz-2.3.0/tutorials/tutorial-lesson-06.html
        /// Example : "0/10 * * * * ?"
        /// Seconds
        /// Minutes
        /// Hours
        /// Day-of-Month
        /// Month
        /// Day-of-Week
        /// Year (optional field)
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// Identifier name
        /// </summary>
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}

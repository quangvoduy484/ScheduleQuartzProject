using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using ScheduleQuartz.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleQuartz.SchedulerQtz
{
    //http://www.quartz-scheduler.org/documentation/quartz-2.3.0/tutorials/tutorial-lesson-06.html => Cron Expressions
    //https://andrewlock.net/creating-a-quartz-net-hosted-service-with-asp-net-core/ => Quartz.NET hosted service with ASP.NET Core
    //https://stackoverflow.com/questions/35796696/multiple-triggers-of-same-job-quartz-net => job and multiple trigger
    //https://stackoverflow.com/questions/13857303/how-to-send-argument-to-class-in-quartz-net =>

    public class QuartzHostedService : IHostedService
    {
        #region
        // if QuartzHostedService register singleton then properties must is singleton and these values assign singleton
        #endregion

        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory; // return instance inherit IJob interface
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        public IScheduler Scheduler { get; set; }

        public QuartzHostedService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IEnumerable<JobSchedule> jobSchedules)
        {
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _jobFactory = jobFactory;
        }

        public async  Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                Scheduler.JobFactory = _jobFactory;

                foreach (var jobSchedule in _jobSchedules)
                {
                    //create job
                    var job = QuartzExtension.CreateJob(jobSchedule);

                    //create trigger
                    var trigger = QuartzExtension.CreateTrigger(jobSchedule);

                    // combine Job and Trigger to Scheduler
                    await Scheduler.ScheduleJob(job, trigger, cancellationToken);
                }

                await Scheduler.Start(cancellationToken);
            }
            catch (System.Exception ex)
            {

            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }
    }
}

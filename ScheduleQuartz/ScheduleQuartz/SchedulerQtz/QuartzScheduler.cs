using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleQuartz.SchedulerQtz
{
    public interface IQuartzScheduler
    {
        /// <summary>
        /// Start job sepcific trigger time 
        /// </summary>
        /// <param name="jobDetail">job</param>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartAsync(IJobDetail jobDetail, ITrigger trigger, CancellationToken cancellationToken);

        /// <summary>
        /// Start single job with multiple trigger
        /// </summary>
        /// <param name="jobDetail">job</param>
        /// <param name="triggers">triggers</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartJobMultilipeTriggers(IJobDetail jobDetail ,  IList<ITrigger> triggers , CancellationToken cancellationToken);
    }

    public class QuartzScheduler : IQuartzScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public  IScheduler Scheduler = null;

        #region contructor
        public QuartzScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }
        #endregion

        #region methods

        public  async Task StartAsync(IJobDetail jobDetail , ITrigger trigger , CancellationToken cancellationToken)
        {
            try
            {
                // check Scheduler and _jobSchedules
                Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

                if (!await Scheduler.CheckExists(jobDetail.Key))
                {
                    await Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                    await Scheduler.Start(cancellationToken);
                }
                else
                {
                    // message required
                }
            }
            catch (Exception ex) {
                //throw;
            }
        }

        public async Task StartJobMultilipeTriggers(IJobDetail jobDetail, IList<ITrigger> triggers, CancellationToken cancellationToken)
        {
            try
            {
                // check Scheduler and _jobSchedules
                Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                await Scheduler.AddJob(jobDetail, true);

                if (triggers.Any())
                {
                    foreach (var trigger in triggers)
                        await Scheduler.ScheduleJob(trigger);
                    await Scheduler.Start(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}

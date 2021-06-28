using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleQuartz.SchedulerQtz
{
    public interface IscopedTest
    {
        void TestLogic();
    }

    [DisallowConcurrentExecution]
    public class GoobyeWorldJob : IJob
    {
        private  int dem = 0;
        private readonly IServiceProvider _provider;
        private readonly ILogger<GoobyeWorldJob> _logger;
        public GoobyeWorldJob(IServiceProvider provider, ILogger<GoobyeWorldJob> logger)
        {
            _logger = logger;
            _provider = provider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            # region if you need to use a scoped service in your HelloWorldJob
            //using (var scope = _provider.CreateScope())
            //{
            //    var service = scope.ServiceProvider.GetService<IscopedTest>();
            //    service.TestLogic();
            //}
            #endregion

            _logger.LogInformation("Cron: " + context.JobDetail.Key.Name + ",Time: " + DateTime.Now.ToString("hh:mm:ss") + " Goobye World!");
            return Task.CompletedTask;
        }
    }

}

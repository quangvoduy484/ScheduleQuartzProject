using Microsoft.AspNetCore.Mvc;
using Quartz;
using ScheduleQuartz.Extensions;
using ScheduleQuartz.SchedulerQtz;
using ScheduleQuartz.Services.EmailService;
using ScheduleQuartz.Services.FileProviderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleQuartz.Controllers
{
    [ApiController]
    [Route("api/quartz")]
    public class QuartzController : ControllerBase
    {
        private readonly IQuartzScheduler _quartzScheduler;
        private readonly IEmailSender _emailSender;
        private readonly IFileProvider _fileProvider;

        /// <summary>
        /// There should only be one applicable constructor.
        /// </summary>
        /// <param name="quartzScheduler"></param>
        public QuartzController(IQuartzScheduler quartzScheduler, IEmailSender emailSender, IFileProvider fileProvider)
        {
            _quartzScheduler = quartzScheduler;
            _emailSender = emailSender;
            _fileProvider = fileProvider;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Index(string cronExpression , string name)
        {
            //jobType: typeof(GoobyeWorldJob), cronExpression: cronExpression
            JobSchedule jobSchedule = new JobSchedule() {
                JobType = typeof(GoobyeWorldJob),
                CronExpression = cronExpression,
                Name = name
            };

            #region Cron examples

            // fire between 8am and 5pm
            //JobSchedule jobSchedule1 = new JobSchedule(jobType: typeof(GoobyeWorldJob), cronExpression: "0 0/2 8-17 * * ?");
            // fire daily at 10:42 am
            //JobSchedule jobSchedule2 = new JobSchedule(jobType: typeof(GoobyeWorldJob), cronExpression: "0 42 10 * * ?");
            // fire on Wednesdays at 10:42 am, in a TimeZone other than the system’s default:
            //JobSchedule jobSchedule3 = new JobSchedule(jobType: typeof(GoobyeWorldJob), cronExpression: "0 42 10 ? * WED");
            // fires at 10:30, 11:30, 12:30, and 13:30, on every Wednesday and Friday.
            //JobSchedule jobSchedule4 = new JobSchedule(jobType: typeof(GoobyeWorldJob), cronExpression: "0 30 10-13 ? * WED,FRI");
            // fires every half hour between the hours of 8 am and 10 am on the 5th and 20th of every month.
            // Note that the trigger will NOT fire at 10:00 am, just at 8:00, 8:30, 9:00 and 9:30
            //JobSchedule jobSchedule5 = new JobSchedule(jobType: typeof(GoobyeWorldJob), cronExpression: "0 0/30 8-9 5,20 * ?");

            #endregion

            await _quartzScheduler.StartAsync(
                QuartzExtension.CreateJob(jobSchedule),
                QuartzExtension.CreateTrigger(jobSchedule)
                , 
                new System.Threading.CancellationToken()
                );

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("start-triggers")]
        public async Task<IActionResult> Start()
        {
            //Having a single job defined behind single key would not run simultaneously even if there were multiple triggers having overlapping schedules associated with it.
            var triggerTimes = new List<string>() { "0/10 * * * * ?", "0/20 * * * * ?" };

            JobSchedule jobSchedule = new JobSchedule() { JobType = typeof(GoobyeWorldJob), Name = "GoobyeWorldJob" };

            var jobDetail = QuartzExtension.CreateJob(jobSchedule);
            List<ITrigger> triggers = triggerTimes.Select((cron, index) => {
                return QuartzExtension.CreateTriggerForJob(name: "GoobyeWorldJob" + index, cron, jobDetail);
            }).ToList(); 

            await _quartzScheduler.StartJobMultilipeTriggers(
                        jobDetail,
                        triggers,
                        new System.Threading.CancellationToken()
                        );

            return Ok();
        }

        [HttpPost("send-mails")]
        public async Task<IActionResult> SendMail()
        {
            string day = "a";
            switch (day)
            {
                case "b":
                    Console.WriteLine("Monday");
                    break;
            }

            int intVal1 = 1;
            string strVal2 = "hello";
            bool boolVal3 = true;

            switch ((intVal1, strVal2, boolVal3))
            {
                case (1, "hello", false):
                    break;
                case (2, "world", false):
                    break;
                case (2, "hello", false):
                    break;
            }

            string result = (intVal1, strVal2, boolVal3) switch
            {
                (1, "hello", false) => "Combination1",
                (2, "world", false) => "Combination2",
                (2, "hello", false) => "Combination3",
                _ => "Default"
            };


            string bodyHtml = @"<!DOCTYPE html>
                                <html>
                                    <head>
                                         <title>Page Title</title>
                                    </head>
                                    <body>
                                        <h1>This is a Heading</h1>
                                        <p>This is a paragraph.</p>
                                    </body>
                                </html>";

            _emailSender.SendEmail(new Models.EmailAccount() 
            {
                Host = "smtp.gmail.com",
                Port = 465,
                EnableSsl = false,
                UseDefaultCredentials = false,
                Username = "quangvoduy32@gmail.com",
                Password ="01645077147",
                Email = "test@mail.com",
                DisplayName = "Store name"
            },
            "QuangTest", bodyHtml,
            "quangvoduy32@gmail.com", "fromQuangAddress",
            "quangvoduy34@gmail.com", "toQuangAddress",
            "quangvoduy484@gmail.com","relyQuangDaiCa",
            new List<string>() { "HoangTran@gmail.com","QuangTruong@gmail.com"},
            new List<string>() { "Moristran@gmail.com", "EndyTran@gmail.com" },
            attachmentFilePath: @"C:\Users\DELL\Desktop\v1.jpg",
            attachmentFileName: "v1.jpg",
            headers: new Dictionary<string, string>() {
                { "AAA","BBB" },
                { "CCC","DDD" },
                { "FFF","GGG" }
            });

            return Ok();
        }
    }
}

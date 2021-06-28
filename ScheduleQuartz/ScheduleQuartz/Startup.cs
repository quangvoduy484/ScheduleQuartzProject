using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using ScheduleQuartz.SchedulerQtz;
using ScheduleQuartz.Services.EmailService;
using ScheduleQuartz.Services.FileProviderService;

namespace ScheduleQuartz
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // add Cors
            services.AddCors();
            services.AddMemoryCache();

            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<ISmtpBuilder, SmtpBuilder>();
            services.AddTransient<IFileProvider, FileProvider>();

            // Add our job
            services.AddSingleton<HelloWorldJob>();
            services.AddSingleton<GoobyeWorldJob>();

            //services.AddSingleton(new JobSchedule(typeof(HelloWorldJob), "0/5 * * * * ?"));
            //services.AddSingleton(new JobSchedule(typeof(HelloWorldJob), "0/10 * * * * ?"));


            // start job cùng lúc
            //services.AddSingleton(new JobSchedule()
            //{
            //    JobType = typeof(HelloWorldJob),
            //    Name = "HelloWorldJob",
            //    CronExpression = "0/5 * * * * ?"
            //});

            services.AddTransient<IQuartzScheduler, QuartzScheduler>();
            services.AddHostedService<QuartzHostedService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}

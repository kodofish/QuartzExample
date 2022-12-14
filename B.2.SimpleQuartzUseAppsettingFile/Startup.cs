using Common;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using Quartz.Logging;
using System.Globalization;

namespace AgileSlot.GameServer
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Adding logging
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            services.AddControllersWithViews();
            services.AddMvc();

            // base configuration from appsettings.json
            services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));

            services.AddQuartz(
                q =>
                {
                    // handy when part of cluster or you want to otherwise identify multiple schedulers
                    q.SchedulerId = "Scheduler-Core";

                    // we take this from appsettings.json, just show it's possible
                    // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

                    // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
                    q.UseMicrosoftDependencyInjectionJobFactory();

                    // or for scoped service support like EF Core DbContext
                    // q.UseMicrosoftDependencyInjectionScopedJobFactory();

                    // these are the defaults
                    q.UseSimpleTypeLoader();
                    q.UseInMemoryStore();
                    q.UseDefaultThreadPool(
                        tp =>
                        {
                            tp.MaxConcurrency = 10;
                        });

                    // quickest way to create a job with single trigger is to use ScheduleJob
                    // (requires version 3.2)
                    q.ScheduleJob<HelloJob>(
                        trigger => trigger
                            .WithIdentity("Combined Configuration Trigger")
                            .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                            .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
                            .WithDescription("my awesome trigger configured for a job with single call")
                    );

                });

            // Quartz.Extensions.Hosting allows you to fire background service that handles scheduler lifecycle
            services.AddQuartzHostedService(
                options =>
                {
                    // when shutting down we want jobs to complete gracefully
                    options.WaitForJobsToComplete = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime,
            ILogger<Startup> logger)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }
}
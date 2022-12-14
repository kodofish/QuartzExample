using Common;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace A.SimpleQuartz
{
    public static class Program
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html#adding-logging
        /// </remarks>
        private async static Task Main(string[] args)
        {
            //Adding logging
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            var factory = new StdSchedulerFactory();

            // get a scheduler
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            var job = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(
                    x => x
                        .WithIntervalInSeconds(5)
                        .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            // some sleep to show what's happening
            await Task.Delay(TimeSpan.FromSeconds(20));

            // and last shut down the scheduler when you are ready to close your program
            await scheduler.Shutdown();

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }
    }

}
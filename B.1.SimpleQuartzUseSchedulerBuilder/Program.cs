using Common;
using Quartz;
using Quartz.Logging;
using System.Collections.Specialized;

namespace B
{
    public class Program
    {
        private async static Task Main(string[] args)
        {
            //Adding logging
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            // you can have base properties
            //Full documentation of available properties is available in the
            //[Quartz Configuration Reference](https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html).
            var properties = new NameValueCollection();

            // and override values via builder
            var scheduler = await SchedulerBuilder.Create(properties)
                                // default max concurrency is 10
                                .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
                                // this is the default
                                // .WithMisfireThreshold(TimeSpan.FromSeconds(60))
                                // .UsePersistentStore(x =>
                                // {
                                    // force job data map values to be considered as strings
                                    // prevents nasty surprises if object is accidentally serialized and then
                                    // serialization format breaks, defaults to false
                                    // x.UseProperties = true;
                                    // x.UseClustering();
                                    // there are other SQL providers supported too
                                    // x.UseSqlServer("my connection string");
                                    // this requires Quartz.Serialization.Json NuGet package
                                    // x.UseJsonSerializer();
                                // })
                                // job initialization plugin handles our xml reading, without it defaults are used
                                // requires Quartz.Plugins NuGet package
                                // .UseXmlSchedulingConfiguration(x =>
                                // {
                                //     x.Files = new[] { "~/quartz_jobs.xml" };
                                //     // this is the default
                                //     x.FailOnFileNotFound = true;
                                //     // this is not the default
                                //     x.FailOnSchedulingError = true;
                                // })
                                .BuildScheduler();

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
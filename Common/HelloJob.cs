using Quartz;

namespace Common
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("HelloJob is executing.");
        }
    }
}
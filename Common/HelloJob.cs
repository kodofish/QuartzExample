using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Util;

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
    
    public class SecurityIssue : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // const string badString = "'; DROP TABLE dbo.Users; --";
            var badString = context.Get("badString");
            await using var db = new MyDbContext();
            await db.Database.ExecuteSqlRawAsync("Select * from dbo.Users Where user = '" + badString + "';");
        }
    }
    
    //Create Db Context class
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public MyDbContext()
        {
            
        }
    }
    
}
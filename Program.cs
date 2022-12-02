using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyAdmin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.MSSqlServer("Data Source=10.190.1.254;database=PostCountData;trusted_connection=False;MultipleActiveResultSets=true;User ID=ASPCDAdmin;Password=quercus500*",
                new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    SchemaName = "dbo",
                    AutoCreateSqlTable = true
                }, restrictedToMinimumLevel: LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
            .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

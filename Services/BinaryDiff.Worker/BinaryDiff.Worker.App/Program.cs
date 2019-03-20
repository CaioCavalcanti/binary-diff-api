using Autofac.Extensions.DependencyInjection;
using BinaryDiff.Shared.WebApi.Extensions;
using BinaryDiff.Worker.Domain.Logic;
using BinaryDiff.Worker.Infrastructure.EventBus;
using BinaryDiff.Worker.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace BinaryDiff.Worker.App
{
    class Program
    {
        private static IConfiguration _configuration;

        static void Main(string[] args)
        {
            ConsoleLog("Starting BinaryDiff.Worker.App...");
            try
            {
                BuildHost();

                var serviceProvider = ConfigureServices();
                var worker = serviceProvider.GetRequiredService<Worker>();

                worker.ListenToNewInputs();
            }
            catch (Exception ex)
            {
                ConsoleLog(ex.Message);
                ConsoleLog(ex.InnerException?.Message);
                ConsoleLog(ex.StackTrace);
            }

            ConsoleLog("Stopping BinaryDiff.Worker.App...");
            Console.ReadLine();
        }

        private static AutofacServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddLogging(logging => logging.UseDefault())
                .UseRabbitMQ(_configuration)
                .UseMongoDb(_configuration)
                    .AddSingleton<IInputRepository, InputRepository>()
                .AddSingleton<IDiffLogic, DiffLogic>()
                    .AddSingleton<IInputEventBus, InputEventBus>()
                    .AddSingleton<IResultEventBus, ResultEventBus>()
                .AddTransient<Worker>()
                .UseAutoFacServiceProvider();
        }

        private static void BuildHost()
        {
            ConsoleLog("Building host...");

            var basePath = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            _configuration = new ConfigurationBuilder()
                .UseDefaultConfiguration(basePath, environment)
                .Build();

            ConsoleLog("Host built!");
        }

        private static void ConsoleLog(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString()} {message}");
        }
    }
}

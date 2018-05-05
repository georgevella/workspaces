﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;
using GBuild.Console.VerbOptions;
using GBuild.Core;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace GBuild.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup logging
            var configuration = new LoggerConfiguration()
                .WriteTo.Console(                    
                    theme: AnsiConsoleTheme.Code,
                    restrictedToMinimumLevel: LogEventLevel.Information
                );

            Log.Logger = configuration.CreateLogger();

            // setup dependency injection container
            var container = new SimpleInjector.Container();

            BuildCoreBootstrapper.BuildDependencyInjectionContainer(container);


            var assemblyList = new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            };

            container.Register(typeof(IVerb<>), assemblyList);
            
            // setup command line parser
            var verbTypes = Assembly.GetExecutingAssembly().DefinedTypes
                .Select(t => new
                {
                    TypeInfo = t,
                    Type = t.AsType(),
                    Verb = t.GetCustomAttribute<VerbAttribute>()
                })
                .Where(t => t.Verb != null).Select(t => t.Type)
                .ToArray();

            var parserResult = CommandLine.Parser.Default.ParseArguments(args, verbTypes);
            parserResult.WithParsed(o =>
            {
                var verbRunnerType = typeof(VerbRunner<>).MakeGenericType(o.GetType());
                var verbRunner = (IVerbRunner) container.GetInstance(verbRunnerType);

                verbRunner.Run(o);
            });

            parserResult.WithNotParsed(errors =>
            {
#if DEBUG
                foreach (var error in errors)
                {
                    System.Console.WriteLine($"{error.Tag}: {error.GetType()}");
                }
#endif
            });

#if DEBUG
            System.Console.ReadKey();
#endif
        }
    }
}

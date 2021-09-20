using System;
using System.IO;
using CommandLine;
using Zds.Cli;
using Zds.Core;
using Zds.Core.Index;
using Zds.Core.Queries;
using Zds.Core.Relations;

namespace Zds
{
    public class Options
    {
        [Option("data", Required = false, HelpText = "Path to search for data. Defaults to current directory.")]
        public string? SourceSearchPath { get; set; }

        [Option("relations", Required = false, HelpText = "Path to file containing relations.")]
        public string? RelationsPath { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            Console.CancelKeyPress += (_, _) => { running = false; };

            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    string sourceSearchPath = string.IsNullOrWhiteSpace(options.SourceSearchPath)
                        ? Directory.GetCurrentDirectory()
                        : options.SourceSearchPath;
                    FileSourceContext fileSourceContext = new(sourceSearchPath);
                    ObjectRepository objectRepository = new();
                    RelationsRepository relationsRepository = new();
                    ObjectGraphQueryHandler handler = new(objectRepository, relationsRepository, fileSourceContext);
                    new LoadFilesCommand(options, objectRepository, relationsRepository, fileSourceContext).Execute();
                    while (running)
                    {
                        ObjectGraphQuery query = new BuildQueryCommand(objectRepository).Execute();
                        new DisplayQueryResultsCommand(handler).Execute(query);
                    }
                });
        }
    }
}

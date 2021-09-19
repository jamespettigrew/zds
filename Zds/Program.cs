using System;
using CommandLine;
using Zds.Cli;
using Zds.Core.Index;
using Zds.Core.Queries;
using Zds.Core.Relations;

namespace Zds
{
    public class Options
    {
        [Option("data", Required = false, HelpText = "Path to search for data. Defaults to current directory.")]
        public string? DataSearchPath { get; set; }
        
        [Option("relations", Required = false, HelpText = "Path to file containing relations.")]
        public string? RelationsPath { get; set; }
    }
        
    
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    ObjectRepository objectRepository = new();
                    RelationsRepository relationsRepository = new();
                    ObjectGraphQueryHandler handler = new(objectRepository, relationsRepository);
                    new LoadFilesCommand(options, objectRepository, relationsRepository).Execute();
                    while (true)
                    {
                        ObjectGraphQuery query = new BuildQueryCommand(objectRepository).Execute();
                        new DisplayQueryResultsCommand(handler).Execute(query);
                    }
                });
        }
    }
}
using System;
using Zds.Cli;
using Zds.Core;
using Zds.Core.Index;
using Zds.Core.Queries;

namespace Zds
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectRepository repository = new ();
            new IndexFilesCommand(repository).Execute();
            while (true)
            {
                ObjectGraphQuery query = new BuildQueryCommand(repository).Execute();
                new DisplayQueryResultsCommand(new ObjectGraphQueryHandler(repository, new RelationsRepository())).Execute(query);
        }
    }
}
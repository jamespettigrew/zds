using System;
using Zds.Cli;
using Zds.Core;

namespace Zds
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository repository = new ();
            new IndexFilesCommand(repository).Execute();
        }
    }
}
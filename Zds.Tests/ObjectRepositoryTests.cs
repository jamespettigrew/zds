using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zds.Core;
using Zds.Core.Index;

namespace Zds.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void ListSourceCorrect()
        {
            ObjectRepository objectRepository = new();
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(new Position(0, 0), new List<PathValue>()));
            objectRepository.AddObjectRecord(
                "test2",
                new ObjectRecord(new Position(0, 0), new List<PathValue>()));

            CollectionAssert.AreEquivalent(new [] { "test1", "test2" }, objectRepository.ListSources());
        }
        
        [TestMethod]
        public void ListPathsForSourceCorrect()
        {
            ObjectRepository objectRepository = new();
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(1, 3),
                    new List<PathValue>
                    {
                        new("x", "0"),
                        new("y", "1")
                    }
                )
            );
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(4, 6),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("y", "1"),
                    }
                )
            );
            objectRepository.AddObjectRecord(
                "test2",
                new ObjectRecord(
                    new Position(7, 3),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("z", "1")
                    }
                )
            );

            CollectionAssert.AreEquivalent(new [] { "x", "y" }, objectRepository.ListPathsForSource("test1"));
        }
        
        [TestMethod]
        public void QueryForPresentValueCorrect()
        {
            ObjectRepository objectRepository = new();
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(1, 3),
                    new List<PathValue>
                    {
                        new("x", "0"),
                        new("y", "1")
                    }
                )
            );
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(4, 6),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("y", "1")
                    }
                )
            );
            objectRepository.AddObjectRecord(
                "test2",
                new ObjectRecord(
                    new Position(7, 3),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("z", "1")
                    }
                )
            );

            CollectionAssert.AreEquivalent(
                new Match[]
                {
                    new ("test1", "y", new Position(1, 3)),
                    new ("test1", "y", new Position(4, 6))
                },
                objectRepository.QuerySource("test1", "y", "1")
            );
        }
        
        
        [TestMethod]
        public void QueryForMissingValueCorrect()
        {
            ObjectRepository objectRepository = new();
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(1, 3),
                    new List<PathValue>
                    {
                        new("x", "0"),
                        new("y", "1")
                    }
                )
            );
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(4, 6),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("y", "1")
                    }
                )
            );
            objectRepository.AddObjectRecord(
                "test1",
                new ObjectRecord(
                    new Position(7, 3),
                    new List<PathValue>
                    {
                        new("x", "1"),
                        new("z", "1")
                    }
                )
            );

            CollectionAssert.AreEquivalent(
                new Match[] { new ("test1", "y", new Position(7, 3))},
                objectRepository.QuerySource("test1", "y", null)
            );
        }
    }
}
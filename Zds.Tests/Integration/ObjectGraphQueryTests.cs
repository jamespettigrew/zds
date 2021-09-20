using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zds.Core;
using Zds.Core.Index;
using Zds.Core.Queries;
using Zds.Core.Relations;

namespace Zds.Tests.Integration
{
    [TestClass]
    public class ObjectGraphQueryTests
    {
        private static string _users = @"[
          {
            ""_id"": 1,
            ""name"": ""Francisca Rasmussen"",
            ""created_at"": ""2016-04-15T05:19:46-10:00"",
            ""verified"": true
          },
          {
            ""_id"": 2,
            ""name"": ""Cross Barlow"",
            ""created_at"": ""2016-06-23T10:31:39-10:00"",
            ""verified"": true
          },
        ]"; 
        
        private static string _tickets = @"[
            {
                ""_id"": ""436bf9b0-1147-4c0a-8439-6f79833bff5b"",
                ""type"": ""incident"",
                ""assignee_id"": 1,
                ""tags"": [
                    ""Ohio"",
                    ""Pennsylvania""
                ],
            },
            {
                ""_id"": ""1a227508-9f39-427c-8f57-1b72f3fab87c"",
                ""type"": ""incident"",
                ""assignee_id"": 1,
                ""tags"": [
                    ""Puerto Rico"",
                    ""Idaho""
                ]
            },
            {
                ""_id"": ""1a227508-9f39-427c-8f57-1b72f3fab71d"",
                ""type"": ""incident"",
                ""assignee_id"": 2
            }
        ]"; 
        
        [TestMethod]
        public void QueryResultsCorrect()
        {
            ObjectRepository objectRepository = new ();
            RelationsRepository relationsRepository = new ();
            relationsRepository.AddRelations(new []
            {
                new Relation(new Key("users.json", "_id"), new Key("tickets.json", "assignee_id"))
            });
            ISourceContext fakeSourceContext = new FakeSourceContext(new List<FakeSource>
            {
                new ("users.json", _users),
                new ("tickets.json", _tickets)
            });
            foreach (string source in fakeSourceContext.ListSources())
            {
                Stream? sourceStream = fakeSourceContext.StreamSource(source);
                foreach (ObjectRecord record in JsonUtils.EnumerateObjects(sourceStream!))
                {
                    objectRepository.AddObjectRecord(source, record);
                }
                fakeSourceContext.StreamSource("users.json");
            }
            ObjectGraphQueryHandler handler = new(objectRepository, relationsRepository, fakeSourceContext);
            Page<ObjectGraph> page = handler.Execute(new ObjectGraphQuery("users.json", "_id", "1"), 3);

            Assert.AreEqual(1, page.Total);
            ObjectGraph match = page.Results.First();
            Assert.AreEqual(2, match.RelatedObjects.Count);
            Assert.AreEqual("Francisca Rasmussen", match.Obj.Value<string>("name"));
            CollectionAssert.AreEquivalent(
                new []
                {
                    "436bf9b0-1147-4c0a-8439-6f79833bff5b",
                    "1a227508-9f39-427c-8f57-1b72f3fab87c"
                },
                new []
                {
                    match.RelatedObjects[0].Value<string>("_id") ?? "",
                    match.RelatedObjects[1].Value<string>("_id") ?? ""
                }
            );
        }
    }
}
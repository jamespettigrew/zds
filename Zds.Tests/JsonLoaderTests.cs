using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zds.Core;

namespace Zds.Tests
{
    [TestClass]
    public class JsonLoaderTests
    {
        private static string TestJson = @"[
            {
                ""_id"": ""436bf9b0-1147-4c0a-8439-6f79833bff5b"",
                ""type"": ""incident"",
                ""tags"": [
                    ""Ohio"",
                    ""Pennsylvania""
                ]
            },
            {
                ""_id"": ""1a227508-9f39-427c-8f57-1b72f3fab87c"",
                ""type"": ""incident"",
                ""tags"": [
                    ""Puerto Rico"",
                    ""Idaho""
                ]
            }
        ]";

        private static string InvalidTestJson = @"[zxcvzxv}{]";
        
        [TestMethod]
        public void EnumeratedObjectsHaveCorrectPositions()
        {
            MemoryStream stream = new(Encoding.UTF8.GetBytes(TestJson));
            List<Position> positions = JsonLoader
                .EnumerateObjects(stream)
                .Select(o => o.Position)
                .ToList();

            CollectionAssert.AreEquivalent(new Position[] {new(2, 13), new(10, 13)}, positions);
        }
        
        
        [TestMethod]
        public void EnumeratedObjectsHaveCorrectPathValues()
        {
            MemoryStream stream = new(Encoding.UTF8.GetBytes(TestJson));
            List<ObjectRecord> objectRecords = JsonLoader.EnumerateObjects(stream).ToList();
            
            List<PathValue> expected = new()
            {
                new("_id", "436bf9b0-1147-4c0a-8439-6f79833bff5b"),
                new("type", "incident"),
                new("tags", "Ohio"),
                new("tags", "Pennsylvania"),
            };
            CollectionAssert.AreEquivalent(expected, objectRecords[0].PathValues.ToList());
            
            expected = new()
            {
                new("_id", "1a227508-9f39-427c-8f57-1b72f3fab87c"),
                new("type", "incident"),
                new("tags", "Puerto Rico"),
                new("tags", "Idaho")
            };
            CollectionAssert.AreEquivalent(expected, objectRecords[1].PathValues.ToList());
        }
        
        [TestMethod]
        public void EnumeratedObjectsFromInvalidJsonThrowsError()
        {
            MemoryStream stream = new(Encoding.UTF8.GetBytes(InvalidTestJson));
            Assert.ThrowsException<JsonLoaderException>(() => JsonLoader.EnumerateObjects(stream).ToList());
        }
        
        [TestMethod]
        public void GetObjectAtValidPositionReturnsCorrectObject()
        {
            MemoryStream stream = new(Encoding.UTF8.GetBytes(TestJson));
            JObject expected = JsonConvert.DeserializeObject<List<JObject>>(TestJson)![1];
            JObject actual = JsonLoader.GetObjectStartingAtPosition(stream, new Position(10, 13));

            Assert.IsTrue(JObject.DeepEquals(expected, actual));
        }
        
        [TestMethod]
        public void GetObjectAtInvalidPositionThrowsError()
        {
            MemoryStream stream = new(Encoding.UTF8.GetBytes(TestJson));
            Assert.ThrowsException<JsonLoaderException>(() =>
                JsonLoader.GetObjectStartingAtPosition(stream, new Position(1, 1)));
        }
        
        [TestMethod]
        public void GetObjectAtPositionFromInvalidJsonThrowsError()
        {
            MemoryStream stream = new(Encoding.UTF8.GetBytes(InvalidTestJson));
            Assert.ThrowsException<JsonLoaderException>(() =>
                JsonLoader.GetObjectStartingAtPosition(stream, new Position(3, 5)));
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zds
{
    public record Position(int Line, int LinePosition);
    public record ObjectRecord(Position Position, List<PathValue> PathValues);
    public record PathValue(string Path, string Value);

    public class JsonLoaderException : Exception
    {
        public JsonLoaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    
    public static class JsonLoader
    {
        public static IEnumerable<ObjectRecord> EnumerateObjects(Stream stream)
        {
            using StreamReader sr = new(stream);
            using JsonTextReader reader = new(sr);

            while (true)
            {
                ObjectRecord? record = null;
                try
                {
                    if (!reader.Read()) break;
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        Position p = new(reader.LineNumber, reader.LinePosition);
                        record = new ObjectRecord(p, Flatten(JObject.Load(reader)));
                    }
                }
                catch (JsonException e)
                {
                    throw new JsonLoaderException("Error encountered enumerating JSON objects.", e);
                }

                // If this looks clumsy it's because C# doesn't permit yielding within a try-catch.
                if (record != null) yield return record;
            }
        }

        private static List<PathValue> Flatten(JObject obj)
        {
            Stack<JToken> stack = new();
            stack.Push(obj);

            List<PathValue> pathValues = new ();
            while (stack.Count > 0)
            {
                JToken token = stack.Pop();
                switch (token.Type)
                {
                    case JTokenType.Comment:
                    case JTokenType.Constructor:
                    case JTokenType.Null:
                    case JTokenType.None:
                        continue;
                    case JTokenType.Array:
                        foreach (string element in token.Values<string>())
                        {
                            pathValues.Add(new (token.Path, element));
                        }
                        break;
                    case JTokenType.Object:
                    case JTokenType.Property:
                        foreach (JToken child in token.Children())
                        {
                            stack.Push(child);
                        }
                        break;
                    default:
                        pathValues.Add(new (token.Path, token.ToString()));
                        break;
                }
            }

            return pathValues;
        }

        public static JObject GetObjectStartingAtPosition(Stream stream, Position position)
        {
            using StreamReader sr = new(stream);
            try
            {
                // Consume stream, by line and character, until we reach first token of the object we wish to load.
                for (int i = 1; i < position.Line; i++) { sr.ReadLine(); }
                for (int i = 1; i < position.LinePosition; i++) { sr.Read(); }

                using JsonTextReader reader = new(sr);
                return JObject.Load(reader);
            }
            catch (Exception e) when (e is JsonReaderException || e is IOException || e is OutOfMemoryException)
            {
                throw new JsonLoaderException("Error encountered loading JSON object at position.", e);
            }
        }
    }
}
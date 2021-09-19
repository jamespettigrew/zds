using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Zds.Core.Relations
{
    public static class RelationsLoader
    {
        public static List<Relation> Load(string path)
        {
            try
            {
                string relationsJson = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Relation>>(relationsJson) ?? new List<Relation>();
            }
            catch (Exception e)
            {
                throw new RelationsException("Relations could not be loaded.", e);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Zds.Core.Relations
{
    public static class RelationsLoader
    {
        public static List<Relation> Load(string path)
        {
            try
            {
                string relationsJson = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Relation>>(
                    relationsJson,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new RequireObjectPropertiesContractResolver()
                    }) ?? new List<Relation>();
            }
            catch (Exception e)
            {
                throw new RelationsException("Relations could not be loaded.", e);
            }
        }
        
        private class RequireObjectPropertiesContractResolver : DefaultContractResolver
        {
            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                var contract = base.CreateObjectContract(objectType);
                contract.ItemRequired = Required.Always;
                return contract;
            }
        }
    }
}
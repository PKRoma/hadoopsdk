using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS
{
    public class DirectoryEntry
    {
        public string AccessTime { get; set; }
        public string BlockSize { get; set; }
        public string Group { get; set; }
        public Int64 Length { get; set; }
        public string ModificationTime { get; set; }
        public string Owner { get; set; }
        public string PathSuffix { get; set; }
        // todo, replace with flag enum 
        public string Permission { get; set; }
        public int Replication { get; set; }
        // todo, replace with enum 
        public string Type { get; set; }

        public DirectoryEntry(JToken value)
        {
            AccessTime = value.Value<string>("accessTime");
            BlockSize = value.Value<string>("blockSize");
            Group = value.Value<string>("group");
            Length = value.Value<Int64>("length");
            ModificationTime = value.Value<string>("modificationTime");
            Owner = value.Value<string>("owner");
            PathSuffix = value.Value<string>("fileSuffix");
            Permission = value.Value<string>("permission");
            Replication = value.Value<int>("replication");
            Type = value.Value<string>("type");
        }

    }
}

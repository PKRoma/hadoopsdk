using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS
{
    public class FileChecksum
    {
        public string Algorithm { get; set; }
        public string Checksum { get; set; }
        public int Length { get; set; }

        public FileChecksum(JToken value)
        {
            Algorithm = value.Value<string>("algorithm");
            Checksum = value.Value<string>("bytes");
            Length = value.Value<int>("length");
        }
    }
}

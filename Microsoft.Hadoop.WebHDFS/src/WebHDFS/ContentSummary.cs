using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS
{
    public class ContentSummary
    {
        public int DirectoryCount { get; set; }
        public int FileCount { get; set; }
        public int Length { get; set; }
        public int Quota { get; set; }
        public int SpaceConsumed { get; set; }
        public int SpaceQuota { get; set; }

        public ContentSummary(JToken value)
        {
            DirectoryCount = value.Value<int>("directoryCount");
            FileCount = value.Value<int>("fileCount");
            Length = value.Value<int>("length");
            Quota = value.Value<int>("quota");
            SpaceConsumed = value.Value<int>("spaceConsumed");
            SpaceQuota = value.Value<int>("spaceQuota");
        }
    }
}

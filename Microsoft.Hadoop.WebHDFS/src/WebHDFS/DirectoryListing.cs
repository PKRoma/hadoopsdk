using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS
{
    public class DirectoryListing
    {
        IEnumerable<DirectoryEntry> directoryEntries;

        public DirectoryListing(JObject rootEntry)
        {
            directoryEntries = rootEntry.Value<JObject>("FileStatuses").Value<JArray>("FileStatus").Select(fs => new DirectoryEntry(fs));
        }

        public IEnumerable<DirectoryEntry> Entries { get { return directoryEntries; } }
        public IEnumerable<DirectoryEntry> Directories { get { return directoryEntries.Where(fs => fs.Type == "DIRECTORY"); } }
        public IEnumerable<DirectoryEntry> Files { get { return directoryEntries.Where(fs => fs.Type == "FILE"); } }


    }
}

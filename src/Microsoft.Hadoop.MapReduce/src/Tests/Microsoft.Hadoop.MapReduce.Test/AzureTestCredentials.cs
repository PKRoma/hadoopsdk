using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Test
{
    [Serializable]
    public class AzureTestCredentials
    {
        public string CredentailsName { get; set; }
        public string Cluster { get; set; }
        public string AzureUserName { get; set; }
        public string AzurePassword { get; set; }
        public string HadoopUserName { get; set; }
        public string BlobStorageAccount { get; set; }
        public string BlobStorageKey { get; set; }
        public string BlobStorageContainer { get; set; }
    }
}

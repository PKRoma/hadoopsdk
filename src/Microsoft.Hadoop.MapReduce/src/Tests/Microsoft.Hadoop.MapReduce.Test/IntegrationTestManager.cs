namespace Microsoft.Hadoop.MapReduce.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class IntegrationTestManager
    {
        private Dictionary<string, AzureTestCredentials> credentialSets = new Dictionary<string, AzureTestCredentials>(); 
        public IntegrationTestManager()
        {
            string file = this.GetConfigPath();
            if (!string.IsNullOrEmpty(file))
            {
                if (File.Exists(file))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(List<AzureTestCredentials>));
                    using (var stream = File.OpenRead(file))
                    {
                        List<AzureTestCredentials> list = (List<AzureTestCredentials>)ser.Deserialize(stream);
                        foreach (var cred in list)
                        {
                            this.credentialSets.Add(cred.CredentailsName, cred);
                        }
                    }
                }
                else
                {
                    MakeFile(file);
                }
            }
        }

        public AzureTestCredentials GetCredentials(string name)
        {
            AzureTestCredentials creds = null;
            this.credentialSets.TryGetValue(name, out creds);
            return creds;
        }

        public void MakeFile(string filePath)
        {
            var def = new AzureTestCredentials();
            def.CredentailsName = "default";
            def.Cluster = "cluster";
            def.AzureUserName = "azureUserName";
            def.AzurePassword = "azurePassword";
            def.HadoopUserName = "hadoopUserName";
            def.BlobStorageAccount = "blogStorageAccount";
            def.BlobStorageKey = "blobStorageKey";
            def.BlobStorageContainer = "blogStorageContainer";
            List<AzureTestCredentials> data = new List<AzureTestCredentials>();
            data.Add(def);

            XmlSerializer ser = new XmlSerializer(typeof(List<AzureTestCredentials>));
            using (var stream = File.OpenWrite(filePath))
            {
                ser.Serialize(stream, data);
            }
        }

        public bool RunAzureTests()
        {
            return GetConfigPath() != null;
        }

        private string GetConfigPath()
        {
            return Environment.GetEnvironmentVariable("MS_HADOOP_TEST_AZURECONFIG");
        }
    }
}

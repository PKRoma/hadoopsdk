namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
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
                    using (var xmlReader = XmlReader.Create(stream))
                    {
                        List<AzureTestCredentials> list = (List<AzureTestCredentials>)ser.Deserialize(xmlReader);
                        foreach (var cred in list)
                        {
                            this.credentialSets.Add(cred.CredentialsName, cred);
                        }
                    }
                }
                else
                {
                    this.MakeFile(file);
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
            def.CredentialsName = "example";
            def.Certificate = @"C:\File\Path\To\Certificate\Uploaded\To\Azure\File.cer";
            def.InvalidCertificate = @"C:\File\Path\To\Certificate\NOTUploaded\To\Azure\File.cer";
            def.Cluster = "https://[dnsname].azurehdinsight.net:563";
            def.DnsName = "dnsname";
            def.AzureUserName = "azureUserName";
            def.AzurePassword = "azurePassword";
            def.HadoopUserName = "hadoopUserName";
            def.DefaultStorageAccount = new StorageAccountCredentials();
            def.DefaultStorageAccount.Name = "blogStorageAccount";
            def.DefaultStorageAccount.Key = "blobStorageKey";
            def.DefaultStorageAccount.Container = "blogStorageContainer";
            def.AdditionalStorageAccounts = new StorageAccountCredentials[1];
            def.AdditionalStorageAccounts[0] = def.DefaultStorageAccount;
            def.HiveStores = new MetastoreCredentials[2];
            def.HiveStores[0] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "HiveStore1",
                UserName = "userName",
                Password = "password"
            };
            def.HiveStores[1] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "HiveStore2",
                UserName = "userName",
                Password = "password"
            };
            def.OozieStores = new MetastoreCredentials[2];
            def.OozieStores[0] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "OozieStore1",
                UserName = "userName",
                Password = "password"
            };
            def.OozieStores[1] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "OozieStore2",
                UserName = "userName",
                Password = "password"
            };
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
            return this.GetConfigPath() != null;
        }

        private string GetConfigPath()
        {
            return Environment.GetEnvironmentVariable("MS_HADOOP_TEST_AZURECONFIG");
        }
    }
}

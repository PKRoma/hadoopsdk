// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities
{
    using System;
    using System.Collections.Generic;

    public class IntegrationTestManager
    {
        public IntegrationTestManager()
        {
            this.InitializeCredentialSets();
        }

        private void InitializeCredentialSets()
        {
            var defaultCredentialSet = new AzureTestCredentials()
            {
                AccessToken = Guid.NewGuid().ToString("N"),
                AzurePassword = "HDInsight123!",
                AzureUserName = "HDInsightUser",
                Certificate = @"\\sat-ci-bld-02\TestResources\Creds\Valid\sdkcli.cer",
                CloudServiceName = "HDInsight",
                SubscriptionId = new Guid("e4c4bcab-7e3b-4439-9919-d2e607f10286"),
                CredentialsName = "default",
                Endpoint = "https://management.core.windows.net:8443/",
                HadoopUserName = "HDInsightUser",
                WellKnownCluster =
                    new KnownCluster()
                    {
                        Cluster = "https://AzureHDInsightTestCluster.AzureHDInsight.net",
                        DnsName = "AzureHDInsightTestCluster",
                        Version = "1.6"
                    },
                Environments = new CreationDetails[]
                {
                    new CreationDetails()
                    {
                        Location="East US 2",
                        HiveStores = new MetastoreCredentials[]
                        {
                          new MetastoreCredentials()
                          {
                              Database = "Hivemetabase",
                              Description="Hive metabase",
                              SqlServer ="hive.sql.server.azure.net"
                          }  
                        },
                        OozieStores = new MetastoreCredentials[]
                        {
                          new MetastoreCredentials()
                          {
                              Database = "ooziemetabase",
                              Description="oozie metabase",
                              SqlServer ="oozie.sql.server.azure.net"
                          }  
                        },
                        DefaultStorageAccount = new StorageAccountCredentials()
                        {
                            Container = "deployment1",
                            Key = Guid.NewGuid().ToString("N"),
                            Name = "defaultstorageaccount.blob.core.windows.net"
                        },
                        AdditionalStorageAccounts = new StorageAccountCredentials[]
                        {
                            new StorageAccountCredentials()
                            {
                                Container = "deployment1",
                                Key = Guid.NewGuid().ToString("N"),
                                Name = "additionaltorageaccount1.blob.core.windows.net"
                            },
                            new StorageAccountCredentials()
                            {
                                Container = "deployment1",
                                Key = Guid.NewGuid().ToString("N"),
                                Name = "additionaltorageaccount2.blob.core.windows.net"
                            }
                        }
                    }
                }
            };

            this.credentialSets.Add("default", defaultCredentialSet);
        }

        private readonly Dictionary<string, AzureTestCredentials> credentialSets = new Dictionary<string, AzureTestCredentials>();

        public IEnumerable<AzureTestCredentials> GetAllCredentials()
        {
            return this.credentialSets.Values;
        }

        public AzureTestCredentials GetCredentials(string name)
        {
            AzureTestCredentials creds = null;
            this.credentialSets.TryGetValue(name, out creds);
            return creds;
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

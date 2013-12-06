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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdLetTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class AddConfigValuesCmdletTests : IntegrationTestBase
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void CanCallTheAddConfigValuesCmdletTestsCmdlet_CoreConfig()
        {
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var coreConfig = new Hashtable();
                coreConfig.Add("hadoop.logfiles.size", "12345");
                RunConfigOptionstest(runspace, CmdletConstants.CoreConfig, coreConfig, c => c.CoreConfiguration);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void CanCallTheAddConfigValuesCmdletTestsCmdlet_HdfsConfig()
        {
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var coreConfig = new Hashtable();
                coreConfig.Add("hadoop.logfiles.size", "12345");
                RunConfigOptionstest(runspace, CmdletConstants.HdfsConfig, coreConfig, c => c.HdfsConfiguration);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void CanCallTheAddConfigValuesCmdletTestsCmdlet_HiveConfig()
        {
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var hiveConfig = new Hashtable();
                hiveConfig.Add("hadoop.logfiles.size", "12345");

                var hiveServiceConfig = new AzureHDInsightHiveConfiguration
                {
                    Configuration = hiveConfig,
                    AdditionalLibraries =
                        new AzureHDInsightDefaultStorageAccount
                        {
                            StorageAccountKey = Guid.NewGuid().ToString(),
                            StorageAccountName = Guid.NewGuid().ToString(),
                            StorageContainerName = Guid.NewGuid().ToString()
                        }
                };

                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.AddAzureHDInsightConfigValues)
                            .WithParameter(CmdletConstants.ClusterConfig, new AzureHDInsightConfig())
                            .WithParameter(CmdletConstants.HiveConfig, hiveServiceConfig)
                            .Invoke();
                AzureHDInsightConfig config = results.Results.ToEnumerable<AzureHDInsightConfig>().First();
                ValidateConfigurationOptions(hiveConfig, config.HiveConfiguration.ConfigurationCollection);
                Assert.IsNotNull(config.HiveConfiguration.AdditionalLibraries);

                Assert.AreEqual(config.HiveConfiguration.AdditionalLibraries.Container, hiveServiceConfig.AdditionalLibraries.StorageContainerName);
                Assert.AreEqual(config.HiveConfiguration.AdditionalLibraries.Key, hiveServiceConfig.AdditionalLibraries.StorageAccountKey);
                Assert.AreEqual(config.HiveConfiguration.AdditionalLibraries.Name, hiveServiceConfig.AdditionalLibraries.StorageAccountName);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void CanCallTheAddConfigValuesCmdletTestsCmdlet_MapReduceConfig()
        {
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var coreConfig = new Hashtable();
                coreConfig.Add("hadoop.logfiles.size", "12345");
                var mapRedConfig = new AzureHDInsightMapReduceConfiguration { Configuration = coreConfig };
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.AddAzureHDInsightConfigValues)
                            .WithParameter(CmdletConstants.ClusterConfig, new AzureHDInsightConfig())
                            .WithParameter(CmdletConstants.MapReduceConfig, mapRedConfig)
                            .Invoke();
                AzureHDInsightConfig config = results.Results.ToEnumerable<AzureHDInsightConfig>().First();
                ValidateConfigurationOptions(coreConfig, config.MapReduceConfiguration.ConfigurationCollection);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void CanCallTheAddConfigValuesCmdletTestsCmdlet_OozieConfig()
        {
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var oozieConfig = new Hashtable();
                oozieConfig.Add("hadoop.logfiles.size", "12345");

                var oozieServiceConfig = new AzureHDInsightOozieConfiguration
                {
                    Configuration = oozieConfig,
                    AdditionalSharedLibraries =
                        new AzureHDInsightDefaultStorageAccount
                        {
                            StorageAccountKey = Guid.NewGuid().ToString(),
                            StorageAccountName = Guid.NewGuid().ToString(),
                            StorageContainerName = Guid.NewGuid().ToString()
                        },
                    AdditionalActionExecutorLibraries =
                        new AzureHDInsightDefaultStorageAccount
                        {
                            StorageAccountKey = Guid.NewGuid().ToString(),
                            StorageAccountName = Guid.NewGuid().ToString(),
                            StorageContainerName = Guid.NewGuid().ToString()
                        }
                };

                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.AddAzureHDInsightConfigValues)
                            .WithParameter(CmdletConstants.ClusterConfig, new AzureHDInsightConfig())
                            .WithParameter(CmdletConstants.OozieConfig, oozieServiceConfig)
                            .Invoke();
                AzureHDInsightConfig config = results.Results.ToEnumerable<AzureHDInsightConfig>().First();
                ValidateConfigurationOptions(oozieConfig, config.OozieConfiguration.ConfigurationCollection);

                Assert.IsNotNull(config.OozieConfiguration.AdditionalSharedLibraries);
                Assert.AreEqual(
                    config.OozieConfiguration.AdditionalSharedLibraries.Container, oozieServiceConfig.AdditionalSharedLibraries.StorageContainerName);
                Assert.AreEqual(
                    config.OozieConfiguration.AdditionalSharedLibraries.Key, oozieServiceConfig.AdditionalSharedLibraries.StorageAccountKey);
                Assert.AreEqual(
                    config.OozieConfiguration.AdditionalSharedLibraries.Name, oozieServiceConfig.AdditionalSharedLibraries.StorageAccountName);

                Assert.IsNotNull(config.OozieConfiguration.AdditionalActionExecutorLibraries);
                Assert.AreEqual(
                    config.OozieConfiguration.AdditionalActionExecutorLibraries.Container,
                    oozieServiceConfig.AdditionalActionExecutorLibraries.StorageContainerName);
                Assert.AreEqual(
                    config.OozieConfiguration.AdditionalActionExecutorLibraries.Key,
                    oozieServiceConfig.AdditionalActionExecutorLibraries.StorageAccountKey);
                Assert.AreEqual(
                    config.OozieConfiguration.AdditionalActionExecutorLibraries.Name,
                    oozieServiceConfig.AdditionalActionExecutorLibraries.StorageAccountName);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("Scenario")]
        public void CanCallTheAddConfigValuesCmdletTestsCmdlet_PreserveConfig()
        {
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                var coreConfig = new Hashtable();
                var clusterConfig = new AzureHDInsightConfig
                {
                    HiveMetastore =
                        new AzureHDInsightMetastore
                        {
                            MetastoreType = AzureHDInsightMetastoreType.HiveMetastore,
                            Credential = GetPSCredential("hadoop", Guid.NewGuid().ToString()),
                            DatabaseName = Guid.NewGuid().ToString(),
                            SqlAzureServerName = Guid.NewGuid().ToString()
                        },
                    OozieMetastore =
                        new AzureHDInsightMetastore
                        {
                            MetastoreType = AzureHDInsightMetastoreType.OozieMetastore,
                            Credential = GetPSCredential("hadoop", Guid.NewGuid().ToString()),
                            DatabaseName = Guid.NewGuid().ToString(),
                            SqlAzureServerName = Guid.NewGuid().ToString()
                        }
                };

                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.AddAzureHDInsightConfigValues)
                            .WithParameter(CmdletConstants.ClusterConfig, clusterConfig)
                            .WithParameter(CmdletConstants.CoreConfig, coreConfig)
                            .Invoke();
                AzureHDInsightConfig config = results.Results.ToEnumerable<AzureHDInsightConfig>().First();

                Assert.AreEqual(config.CoreConfiguration.Count, coreConfig.Count);

                foreach (object entry in coreConfig.Keys)
                {
                    KeyValuePair<string, string> configUnderTest =
                        config.CoreConfiguration.FirstOrDefault(c => string.Equals(c.Key, entry.ToString(), StringComparison.Ordinal));
                    Assert.IsNotNull(configUnderTest, "Unable to find core config option with name '{0}'", entry);
                    Assert.AreEqual(coreConfig[entry], configUnderTest.Value, "value doesn't match for core config option with name '{0}'", entry);
                }
                Assert.AreEqual(clusterConfig.HiveMetastore.DatabaseName, config.HiveMetastore.DatabaseName);
                Assert.AreEqual(clusterConfig.HiveMetastore.SqlAzureServerName, config.HiveMetastore.SqlAzureServerName);
                Assert.AreEqual(clusterConfig.HiveMetastore.Credential.UserName, config.HiveMetastore.Credential.UserName);
                Assert.AreEqual(clusterConfig.HiveMetastore.Credential.GetCleartextPassword(), config.HiveMetastore.Credential.GetCleartextPassword());

                Assert.AreEqual(clusterConfig.OozieMetastore.DatabaseName, config.OozieMetastore.DatabaseName);
                Assert.AreEqual(clusterConfig.OozieMetastore.SqlAzureServerName, config.OozieMetastore.SqlAzureServerName);
                Assert.AreEqual(clusterConfig.OozieMetastore.Credential.UserName, config.OozieMetastore.Credential.UserName);
                Assert.AreEqual(
                    clusterConfig.OozieMetastore.Credential.GetCleartextPassword(), config.OozieMetastore.Credential.GetCleartextPassword());
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        private static void RunConfigOptionstest(
            IRunspace runspace, string configOptionName, Hashtable expected, Func<AzureHDInsightConfig, ConfigValuesCollection> configPropertyAccessor)
        {
            IPipelineResult results =
                runspace.NewPipeline()
                        .AddCommand(CmdletConstants.AddAzureHDInsightConfigValues)
                        .WithParameter(CmdletConstants.ClusterConfig, new AzureHDInsightConfig())
                        .WithParameter(configOptionName, expected)
                        .Invoke();
            AzureHDInsightConfig config = results.Results.ToEnumerable<AzureHDInsightConfig>().First();
            ValidateConfigurationOptions(expected, configPropertyAccessor(config));
        }

        private static void ValidateConfigurationOptions(Hashtable expected, ConfigValuesCollection actual)
        {
            foreach (object entry in expected.Keys)
            {
                KeyValuePair<string, string> configUnderTest =
                    actual.FirstOrDefault(c => string.Equals(c.Key, entry.ToString(), StringComparison.Ordinal));
                Assert.IsNotNull(configUnderTest, "Unable to find config option with name '{0}'", entry);
                Assert.AreEqual(expected[entry], configUnderTest.Value, "value doesn't match for config option with name '{0}'", entry);
            }
        }
    }
}

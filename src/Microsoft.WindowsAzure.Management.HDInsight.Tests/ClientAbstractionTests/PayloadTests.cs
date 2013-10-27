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
namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.ServerDataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.ServerDataObjects.Rdfe;

    [TestClass]
    public class PayloadTests : IntegrationTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_ExtractResourceOutputValue()
        {
            // During serialization\deserialization it loses ms precission. Therefore using ms-less times 
            DateTime time1 = TruncateMiliseconds(DateTime.UtcNow), time2 = TruncateMiliseconds(DateTime.Now);

            // Creates a response
            var res = new Resource();
            res.OutputItems = new OutputItemList
            {
                new OutputItem { Key = "boolean", Value = "true" },
                new OutputItem { Key = "time1", Value = time1.ToString(CultureInfo.InvariantCulture) },
                new OutputItem { Key = "time2", Value = time2.ToString(CultureInfo.InvariantCulture) },
                new OutputItem { Key = "string", Value = "value" },
                new OutputItem { Key = "number", Value = "7" },
                new OutputItem { Key = "uri", Value = new Uri("https://some/long/uri/").AbsoluteUri },
            };

            XElement resourceElement = ServerSerializer.SerializeResource(res);

            // Validates nonexisting properties
            var payloadConverter = new PayloadConverter();
            Assert.AreEqual(DateTime.MinValue, payloadConverter.ExtractClusterPropertyDateTimeValue(resourceElement, Enumerable.Empty<KeyValuePair<string, string>>(), "nonexist"));
            Assert.AreEqual(null, payloadConverter.ExtractResourceOutputStringValue(resourceElement, "nonexist"));
            Assert.AreEqual(0, payloadConverter.ExtractClusterPropertyIntValue(resourceElement, Enumerable.Empty<KeyValuePair<string, string>>(), "nonexist"));

            // Validates existing properties
            Assert.AreEqual(time1, payloadConverter.ExtractClusterPropertyDateTimeValue(resourceElement, Enumerable.Empty<KeyValuePair<string, string>>(), "time1"));
            Assert.AreEqual(time2, payloadConverter.ExtractClusterPropertyDateTimeValue(resourceElement, Enumerable.Empty<KeyValuePair<string, string>>(), "time2"));
            Assert.AreEqual("value", payloadConverter.ExtractResourceOutputStringValue(resourceElement, "string"));
            Assert.AreEqual(7, payloadConverter.ExtractClusterPropertyIntValue(resourceElement, Enumerable.Empty<KeyValuePair<string, string>>(), "number"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_ExtractIntrinsicSettingsValue()
        {
            // During serialization\deserialization it loses ms precission. Therefore using ms-less times 
            DateTime time1 = TruncateMiliseconds(DateTime.UtcNow), time2 = TruncateMiliseconds(DateTime.Now);

            // Creates a response
            var res = new Resource();
            res.OutputItems = new OutputItemList();
            var intrinsicSettings = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string,string>("boolean", "true"),
                new KeyValuePair<string,string>("time1", time1.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string,string>("time2", time2.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string,string>("string", "value"),
                new KeyValuePair<string,string>("number", "7")
            };

            XElement resourceElement = ServerSerializer.SerializeResource(res, intrinsicSettings);

            // Validates nonexisting properties
            var payloadConverter = new PayloadConverter();
            Assert.AreEqual(DateTime.MinValue, payloadConverter.ExtractClusterPropertyDateTimeValue(resourceElement, intrinsicSettings, "nonexist"));
            Assert.AreEqual(null, payloadConverter.GetClusterProperty(resourceElement, intrinsicSettings, "nonexist"));
            Assert.AreEqual(0, payloadConverter.ExtractClusterPropertyIntValue(resourceElement, intrinsicSettings, "nonexist"));

            // Validates existing properties
            Assert.AreEqual(time1, payloadConverter.ExtractClusterPropertyDateTimeValue(resourceElement, intrinsicSettings, "time1"));
            Assert.AreEqual(time2, payloadConverter.ExtractClusterPropertyDateTimeValue(resourceElement, intrinsicSettings, "time2"));
            Assert.AreEqual("value", payloadConverter.GetClusterProperty(resourceElement, intrinsicSettings, "string"));
            Assert.AreEqual(7, payloadConverter.ExtractClusterPropertyIntValue(resourceElement, intrinsicSettings, "number"));
        }

        private static DateTime TruncateMiliseconds(DateTime time)
        {
            return time.AddTicks(-(time.Ticks % TimeSpan.TicksPerSecond));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationListContainersResult()
        {
            var storageAccount = new WabStorageAccountConfiguration(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            // Creates two random containers
            var container1 = new ClusterDetails(base.GetRandomClusterName(), "Running")
            {
                CreatedDate = DateTime.Now,
                ConnectionUrl = @"https://some/long/uri/",
                HttpUserName = "someuser",
                Location = "East US",
                ClusterSizeInNodes = 20,
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version
            };
            container1.DefaultStorageAccount = storageAccount;
            container1.AdditionalStorageAccounts = new List<WabStorageAccountConfiguration>() 
            { 
                new WabStorageAccountConfiguration(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
                new WabStorageAccountConfiguration(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) 
            };
            var container2 = new ClusterDetails(base.GetRandomClusterName(), "ClusterStorageProvisioned")
            {
                CreatedDate = DateTime.Now,
                ConnectionUrl = @"https://some/long/uri/",
                HttpUserName = "someuser2",
                Location = "West US",
                ClusterSizeInNodes = 10,
                Error = new ClusterErrorStatus(10, "error", "create"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version
            };

            var originalContainers = new Collection<ClusterDetails> { container1, container2 };

            // Roundtrip serialize\deserialize
            Guid subscriptionId = new Guid();
            var payload = ServerSerializer.SerializeListContainersResult(originalContainers, "namespace");
            var finalContainers = new PayloadConverter().DeserializeListContainersResult(payload, "namespace", subscriptionId);

            // Compares the lists
            Assert.AreEqual(originalContainers.Count, finalContainers.Count);
            foreach (var expectedCluster in originalContainers)
            {
                var deserializedCluster = finalContainers.FirstOrDefault(cluster => cluster.Name == expectedCluster.Name);
                Assert.IsNotNull(deserializedCluster);
                Assert.AreEqual(deserializedCluster.SubscriptionId, subscriptionId);
                Assert.IsTrue(Equals(expectedCluster, deserializedCluster), "Failed to deserialize cluster pigJobCreateParameters {0}", expectedCluster.Name);
            }
        }

        private static bool Equals(ClusterDetails expectedCluster, ClusterDetails deserializedCluster)
        {
            if (expectedCluster == null && deserializedCluster == null)
            {
                return true;
            }
            if (expectedCluster == null || deserializedCluster == null)
            {
                return false;
            }

            var comparisonTuples = new List<Tuple<object, object>>
            {
                new Tuple<object, object>(expectedCluster.Name, deserializedCluster.Name),
                new Tuple<object, object>(expectedCluster.State, deserializedCluster.State),
                new Tuple<object, object>(expectedCluster.StateString, deserializedCluster.StateString),
                new Tuple<object, object>(TruncateMiliseconds(expectedCluster.CreatedDate), TruncateMiliseconds(deserializedCluster.CreatedDate)),
                new Tuple<object, object>(expectedCluster.Location, deserializedCluster.Location),
                new Tuple<object, object>(expectedCluster.HttpUserName, deserializedCluster.HttpUserName),
                new Tuple<object, object>(expectedCluster.ConnectionUrl, deserializedCluster.ConnectionUrl),
                new Tuple<object, object>(expectedCluster.ClusterSizeInNodes, deserializedCluster.ClusterSizeInNodes),
            };
            if (expectedCluster.Error == null && deserializedCluster.Error != null)
                return false;
            if (expectedCluster.Error != null && deserializedCluster.Error == null)
                return false;
            if (expectedCluster.Error != null && deserializedCluster.Error != null)
            {
                comparisonTuples.Add(new Tuple<object, object>(expectedCluster.Error.HttpCode, deserializedCluster.Error.HttpCode));
                comparisonTuples.Add(new Tuple<object, object>(expectedCluster.Error.Message, deserializedCluster.Error.Message));
                comparisonTuples.Add(new Tuple<object, object>(expectedCluster.Error.OperationType, deserializedCluster.Error.OperationType));
            }

            if (expectedCluster.DefaultStorageAccount != null)
            {
                Assert.IsNotNull(deserializedCluster.DefaultStorageAccount, "DefaultStorageAccount");
                Assert.AreEqual(expectedCluster.DefaultStorageAccount.Key, deserializedCluster.DefaultStorageAccount.Key, "Key");
                Assert.AreEqual(expectedCluster.DefaultStorageAccount.Container, deserializedCluster.DefaultStorageAccount.Container, "Container");
            }

            foreach (var storageAccount in expectedCluster.AdditionalStorageAccounts)
            {
                var deserializedStorageAccount = deserializedCluster.AdditionalStorageAccounts.FirstOrDefault(acc => acc.Name == storageAccount.Name);
                Assert.IsNotNull(deserializedStorageAccount, storageAccount.Name);
                Assert.AreEqual(storageAccount.Key, deserializedStorageAccount.Key, "Key");
                Assert.AreEqual(storageAccount.Container, deserializedStorageAccount.Container, "Container");
            }

            return CompareTuples(comparisonTuples);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequest()
        {
            var cluster1 = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                ClusterSizeInNodes = new Random().Next()
            };
            cluster1.AdditionalStorageAccounts.Add(new WabStorageAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));
            cluster1.AdditionalStorageAccounts.Add(new WabStorageAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(cluster1);
            var cluster2 = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(cluster1, cluster2));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequest_MayContracts()
        {
            var cluster1 = new ClusterCreateParameters
            {
                Name = "bcarlson",
                ClusterSizeInNodes = 1,
                UserName = "bcarlson",
                Version = "default",
                Password = "SuperPass1!",
                Location = "East US"
            };

            cluster1.DefaultStorageAccountName = "hdicurrenteastus.blob.core.windows.net";
            cluster1.DefaultStorageContainer = "newcontainer";
            cluster1.DefaultStorageAccountKey = "jKe7cqoU0a9OmDFlwi3DHZLf7JoKwGOU2pV1iZdBKifxwQuDOKwZFyXMJrPSLtGgDV9b7pVKSGz6lbBWcfX2lA==";

            var metaStore = new Metastore("lbl44y45cd.bigbean.windowsazure.mscds.com", "newmaytestdb", "bcarlson", "SuperPass1!");
            cluster1.HiveMetastore = cluster1.OozieMetastore = metaStore;

            string payload = new PayloadConverter().SerializeClusterCreateRequest(cluster1);
            var resource = ServerSerializer.DeserializeClusterCreateRequestIntoResource(payload);
            Assert.AreEqual(resource.SchemaVersion, "2.0");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithMetastore()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };
            expected.AdditionalStorageAccounts.Add(new WabStorageAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));
            expected.AdditionalStorageAccounts.Add(new WabStorageAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));
            expected.OozieMetastore = new Metastore(Guid.NewGuid().ToString("N"),
                                                             Guid.NewGuid().ToString("N"),
                                                             Guid.NewGuid().ToString("N"),
                                                             Guid.NewGuid().ToString("N"));
            expected.HiveMetastore = new Metastore(Guid.NewGuid().ToString("N"),
                                                            Guid.NewGuid().ToString("N"),
                                                            Guid.NewGuid().ToString("N"),
                                                            Guid.NewGuid().ToString("N"));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_ConvertStringToVersion_Case1()
        {
            Version version = new Version(1, 5);
            Assert.AreEqual(version, new PayloadConverter().ConvertStringToVersion("1.5"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_ConvertStringToVersion_Case2()
        {
            Version version = new Version(1, 6, 0, 0);
            Assert.AreEqual(version, new PayloadConverter().ConvertStringToVersion("1.6.0.0.LargeSKU"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_ConvertStringToVersion_Case3()
        {
            Version version = new Version();
            Assert.AreEqual(version, new PayloadConverter().ConvertStringToVersion(""));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithCoreConfiguration()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };

            expected.CoreConfiguration.Add(new KeyValuePair<string, string>("my setting 1", "my value 1"));
            expected.CoreConfiguration.Add(new KeyValuePair<string, string>("my setting 2", "my value 2"));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithOozieConfiguration()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };

            expected.OozieConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 1", "my value 1"));
            expected.OozieConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 2", "my value 2"));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithOozieLibraries()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };

            expected.OozieConfiguration.AdditionalSharedLibraries = new WabStorageAccountConfiguration(
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            expected.OozieConfiguration.AdditionalActionExecutorLibraries = new WabStorageAccountConfiguration(
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            expected.OozieConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 1", "my value 1"));
            expected.OozieConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 2", "my value 2"));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithHiveConfiguration()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };

            expected.HiveConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 1", "my value 1"));
            expected.HiveConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 2", "my value 2"));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }
        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithHiveConfiguration_Resources()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };

            expected.HiveConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 1", "my value 1"));
            expected.HiveConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 2", "my value 2"));
            expected.HiveConfiguration.AdditionalLibraries = new WabStorageAccountConfiguration(
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithMapReduceConfiguration()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };

            expected.MapReduceConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 1", "my value 1"));
            expected.MapReduceConfiguration.ConfigurationCollection.Add(new KeyValuePair<string, string>("my setting 2", "my value 2"));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithHdfsConfiguration()
        {
            var expected = new ClusterCreateParameters
            {
                UserName = Guid.NewGuid().ToString("N"),
                Password = Guid.NewGuid().ToString("N"),
                Version = IntegrationTestBase.TestCredentials.WellKnownCluster.Version,
                DefaultStorageAccountKey = Guid.NewGuid().ToString("N"),
                DefaultStorageAccountName = Guid.NewGuid().ToString("N"),
                DefaultStorageContainer = Guid.NewGuid().ToString("N"),
                Name = base.GetRandomClusterName(),
                Location = "East US",
                ClusterSizeInNodes = new Random().Next()
            };

            expected.HdfsConfiguration.Add(new KeyValuePair<string, string>("my setting 1", "my value 1"));
            expected.HdfsConfiguration.Add(new KeyValuePair<string, string>("my setting 2", "my value 2"));

            string payload = new PayloadConverter().SerializeClusterCreateRequest(expected);
            var actual = ServerSerializer.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(expected, actual));
        }

        private static bool Equals(ClusterCreateParameters expected, ClusterCreateParameters actual)
        {
            if (expected == null && actual == null)
            {
                return true;
            }
            if (expected == null || actual == null)
            {
                return false;
            }

            // Compares the properties and fails if there is a mismatch
            int headNodeCount = 1;
            var comparisonTuples = new List<Tuple<object, object>>
            {
                new Tuple<object, object>(expected.UserName, actual.UserName),
                new Tuple<object, object>(expected.Password, actual.Password),
                new Tuple<object, object>(expected.Version, actual.Version),
                new Tuple<object, object>(expected.DefaultStorageAccountKey, actual.DefaultStorageAccountKey),
                new Tuple<object, object>(expected.DefaultStorageAccountName, actual.DefaultStorageAccountName),
                new Tuple<object, object>(expected.DefaultStorageContainer, actual.DefaultStorageContainer),
                new Tuple<object, object>(expected.Name, actual.Name),
                new Tuple<object, object>(expected.Location, actual.Location),
                new Tuple<object, object>(expected.ClusterSizeInNodes + headNodeCount, actual.ClusterSizeInNodes ),
                new Tuple<object, object>(expected.AdditionalStorageAccounts.Count, actual.AdditionalStorageAccounts.Count),
            };
            if (expected.OozieMetastore != null)
            {
                Assert.IsNotNull(actual.OozieMetastore, "OozieMetaStore");

                comparisonTuples.Add(new Tuple<object, object>(expected.OozieMetastore.Server, actual.OozieMetastore.Server));
                comparisonTuples.Add(new Tuple<object, object>(expected.OozieMetastore.Database, actual.OozieMetastore.Database));
                comparisonTuples.Add(new Tuple<object, object>(expected.OozieMetastore.User, actual.OozieMetastore.User));
                comparisonTuples.Add(new Tuple<object, object>(expected.OozieMetastore.Password, actual.OozieMetastore.Password));
            }
            if (expected.HiveMetastore != null)
            {
                Assert.IsNotNull(actual.HiveMetastore, "HiveMetastore");
                comparisonTuples.Add(new Tuple<object, object>(expected.HiveMetastore.Server, actual.HiveMetastore.Server));
                comparisonTuples.Add(new Tuple<object, object>(expected.HiveMetastore.Database, actual.HiveMetastore.Database));
                comparisonTuples.Add(new Tuple<object, object>(expected.HiveMetastore.User, actual.HiveMetastore.User));
                comparisonTuples.Add(new Tuple<object, object>(expected.HiveMetastore.Password, actual.HiveMetastore.Password));
            }
            if (!CompareTuples(comparisonTuples))
            {
                return false;
            }

            foreach (var storageAccount in expected.AdditionalStorageAccounts)
            {
                var storageAccountUnderTest = actual.AdditionalStorageAccounts.FirstOrDefault(storage => storage.Name == storageAccount.Name);
                Assert.IsNotNull(storageAccountUnderTest, "Storage account '{0}' was not found.", storageAccount.Name);
                Assert.AreEqual(storageAccountUnderTest.Key, storageAccountUnderTest.Key);
            }

            if (expected.OozieConfiguration.AdditionalSharedLibraries != null)
            {
                Assert.IsNotNull(actual.OozieConfiguration.AdditionalSharedLibraries);
                Assert.AreEqual(actual.OozieConfiguration.AdditionalSharedLibraries.Container, actual.OozieConfiguration.AdditionalSharedLibraries.Container);
                Assert.AreEqual(actual.OozieConfiguration.AdditionalSharedLibraries.Name, actual.OozieConfiguration.AdditionalSharedLibraries.Name);
                Assert.AreEqual(actual.OozieConfiguration.AdditionalSharedLibraries.Key, actual.OozieConfiguration.AdditionalSharedLibraries.Key);
            }

            if (expected.OozieConfiguration.AdditionalActionExecutorLibraries != null)
            {
                Assert.IsNotNull(actual.OozieConfiguration.AdditionalActionExecutorLibraries);
                Assert.AreEqual(actual.OozieConfiguration.AdditionalActionExecutorLibraries.Container, actual.OozieConfiguration.AdditionalActionExecutorLibraries.Container);
                Assert.AreEqual(actual.OozieConfiguration.AdditionalActionExecutorLibraries.Name, actual.OozieConfiguration.AdditionalActionExecutorLibraries.Name);
                Assert.AreEqual(actual.OozieConfiguration.AdditionalActionExecutorLibraries.Key, actual.OozieConfiguration.AdditionalActionExecutorLibraries.Key);
            }

            AssertConfiguration(expected.CoreConfiguration, actual.CoreConfiguration);
            AssertConfiguration(expected.OozieConfiguration.ConfigurationCollection, actual.OozieConfiguration.ConfigurationCollection);
            AssertConfiguration(expected.HiveConfiguration, actual.HiveConfiguration);
            AssertConfiguration(expected.MapReduceConfiguration.ConfigurationCollection, actual.MapReduceConfiguration.ConfigurationCollection);
            AssertConfiguration(expected.HdfsConfiguration, actual.HdfsConfiguration);

            return true;
        }

        private static void AssertConfiguration(HiveConfiguration expected, HiveConfiguration actual)
        {
            if (expected.AdditionalLibraries != null)
            {
                Assert.IsNotNull(actual.AdditionalLibraries);
                Assert.AreEqual(expected.AdditionalLibraries.Container, actual.AdditionalLibraries.Container);
                Assert.AreEqual(expected.AdditionalLibraries.Key, actual.AdditionalLibraries.Key);
                Assert.AreEqual(expected.AdditionalLibraries.Name, actual.AdditionalLibraries.Name);
            }

            AssertConfiguration(expected.ConfigurationCollection, actual.ConfigurationCollection);
        }

        private static void AssertConfiguration(IEnumerable<KeyValuePair<string, string>> expected, ConfigValuesCollection actual)
        {
            foreach (var configProperty in expected)
            {
                var propertyUnderTest = actual.FirstOrDefault(storage => storage.Key == configProperty.Key);
                Assert.IsNotNull(propertyUnderTest, "Oozie Configuration '{0}' was not found.", configProperty.Key);
                Assert.AreEqual(configProperty.Value, propertyUnderTest.Value);
            }
        }

        private static bool CompareTuples(IEnumerable<Tuple<object, object>> tuples)
        {
            if (tuples.Any(tuple => tuple.Item1 == null && tuple.Item2 != null))
            {
                Assert.AreEqual(
                    tuples.First(tuple => tuple.Item1 == null && tuple.Item2 != null).Item1,
                    tuples.First(tuple => tuple.Item1 == null && tuple.Item2 != null).Item2);
            }
            if (tuples.Any(tuple => tuple.Item1 != null && tuple.Item2 == null))
            {
                Assert.AreEqual(
                    tuples.First(tuple => tuple.Item1 != null && tuple.Item2 == null).Item1,
                    tuples.First(tuple => tuple.Item1 != null && tuple.Item2 == null).Item2);
            }

            tuples = tuples.Where(tuple => tuple.Item1 != null);
            foreach (var nonNullTuple in tuples)
            {
                Assert.AreEqual(nonNullTuple.Item1, nonNullTuple.Item2);
            }

            return true;
        }
    }
}
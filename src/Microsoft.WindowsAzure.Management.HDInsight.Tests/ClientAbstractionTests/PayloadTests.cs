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
    using System.Collections.Generic;
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.ObjectModel;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old;

    [TestClass]
    public class PayloadTests : IntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
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

            // Validates nonexisting properties
            Assert.AreEqual(false, PayloadConverter.ExtractResourceOutputValue<bool>(res, "nonexist"));
            Assert.AreEqual(DateTime.MinValue, PayloadConverter.ExtractResourceOutputValue<DateTime>(res, "nonexist"));
            Assert.AreEqual(null, PayloadConverter.ExtractResourceOutputValue<string>(res, "nonexist"));
            Assert.AreEqual(0, PayloadConverter.ExtractResourceOutputValue<int>(res, "nonexist"));

            // Validates existing properties
            Assert.AreEqual(true, PayloadConverter.ExtractResourceOutputValue<bool>(res, "boolean"));
            Assert.AreEqual(time1, PayloadConverter.ExtractResourceOutputValue<DateTime>(res, "time1"));
            Assert.AreEqual(time2, PayloadConverter.ExtractResourceOutputValue<DateTime>(res, "time2"));
            Assert.AreEqual("value", PayloadConverter.ExtractResourceOutputValue<string>(res, "string"));
            Assert.AreEqual(7, PayloadConverter.ExtractResourceOutputValue<int>(res, "number"));
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
            // Creates two random containers
            var container1 = new ListClusterContainerResult(base.GetRandomClusterName(), "Running")
            {
                CreatedDate = DateTime.Now,
                ConnectionUrl = @"https://some/long/uri/",
                UserName = "someuser",
                Location = "East US",
                WorkerNodesCount = 20,
            };
            var container2 = new ListClusterContainerResult(base.GetRandomClusterName(), "ClusterStorageProvisioned")
            {
                CreatedDate = DateTime.Now,
                ConnectionUrl = @"https://some/long/uri/",
                UserName = "someuser2",
                Location = "West US",
                WorkerNodesCount = 10,
                ResultError = new ClusterErrorStatus(10, "error", "create")
            };
            var originalContainers = new Collection<ListClusterContainerResult> { container1, container2 };

            // Roundtrip serialize\deserialize
            var payload = PayloadConverter.SerializeListContainersResult(originalContainers, "namespace");
            var finalContainers = PayloadConverter.DeserializeListContainersResult(payload, "namespace");

            // Compares the lists
            Assert.AreEqual(originalContainers.Count, finalContainers.Count);
            Assert.IsTrue(originalContainers.All(c1 => finalContainers.Count(c2 => Equals(c1, c2)) == 1));
        }

        private static bool Equals(ListClusterContainerResult cluster1, ListClusterContainerResult cluster2)
        {
            if (cluster1 == null && cluster2 == null)
            {
                return true;
            }
            if (cluster1 == null || cluster2 == null)
            {
                return false;
            }

            var comparisonTuples = new List<Tuple<object, object>>
            {
                new Tuple<object, object>(cluster1.DnsName, cluster2.DnsName),
                new Tuple<object, object>(cluster1.ParsedState, cluster2.ParsedState),
                new Tuple<object, object>(cluster1.State, cluster2.State),
                new Tuple<object, object>(TruncateMiliseconds(cluster1.CreatedDate), TruncateMiliseconds(cluster2.CreatedDate)),
                new Tuple<object, object>(cluster1.Location, cluster2.Location),
                new Tuple<object, object>(cluster1.UserName, cluster2.UserName),
                new Tuple<object, object>(cluster1.ConnectionUrl, cluster2.ConnectionUrl),
                new Tuple<object, object>(cluster1.WorkerNodesCount, cluster2.WorkerNodesCount),
            };
            if (cluster1.ResultError == null && cluster2.ResultError != null)
                return false;
            if (cluster1.ResultError != null && cluster2.ResultError == null)
                return false;
            if (cluster1.ResultError != null && cluster2.ResultError != null)
            {
                comparisonTuples.Add(new Tuple<object, object>(cluster1.ResultError.HttpCode, cluster2.ResultError.HttpCode));
                comparisonTuples.Add(new Tuple<object, object>(cluster1.ResultError.Message, cluster2.ResultError.Message));
                comparisonTuples.Add(new Tuple<object, object>(cluster1.ResultError.OperationType, cluster2.ResultError.OperationType));
            }

            return CompareTuples(comparisonTuples);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequest()
        {
            var cluster1 = new CreateClusterRequest
            {
                ClusterUserName = Guid.NewGuid().ToString("N"),
                ClusterUserPassword = Guid.NewGuid().ToString("N"),
                DefaultAsvAccountKey = Guid.NewGuid().ToString("N"),
                DefaultAsvAccountName = Guid.NewGuid().ToString("N"),
                DefaultAsvContainer = Guid.NewGuid().ToString("N"),
                DnsName = base.GetRandomClusterName(),
                Location = "East US",
                WorkerNodeCount = new Random().Next()
            };
            cluster1.AsvAccounts.Add(new AsvAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));
            cluster1.AsvAccounts.Add(new AsvAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));

            string payload = PayloadConverter.SerializeClusterCreateRequest(cluster1, Guid.NewGuid());
            var cluster2 = PayloadConverter.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(cluster1, cluster2));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Payload")]
        public void InternalValidation_PayloadConverter_SerializationCreateRequestWithMetastore()
        {
            var cluster1 = new CreateClusterRequest
            {
                ClusterUserName = Guid.NewGuid().ToString("N"),
                ClusterUserPassword = Guid.NewGuid().ToString("N"),
                DefaultAsvAccountKey = Guid.NewGuid().ToString("N"),
                DefaultAsvAccountName = Guid.NewGuid().ToString("N"),
                DefaultAsvContainer = Guid.NewGuid().ToString("N"),
                DnsName = base.GetRandomClusterName(),
                Location = "East US",
                WorkerNodeCount = new Random().Next()
            };
            cluster1.AsvAccounts.Add(new AsvAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));
            cluster1.AsvAccounts.Add(new AsvAccountConfiguration(Guid.NewGuid().ToString("N"),
                                                                 Guid.NewGuid().ToString("N")));
            cluster1.OozieMetastore = new ComponentMetastore(Guid.NewGuid().ToString("N"),
                                                             Guid.NewGuid().ToString("N"),
                                                             Guid.NewGuid().ToString("N"),
                                                             Guid.NewGuid().ToString("N"));
            cluster1.HiveMetastore = new ComponentMetastore(Guid.NewGuid().ToString("N"),
                                                            Guid.NewGuid().ToString("N"),
                                                            Guid.NewGuid().ToString("N"),
                                                            Guid.NewGuid().ToString("N"));

            string payload = PayloadConverter.SerializeClusterCreateRequest(cluster1, Guid.NewGuid());
            var cluster2 = PayloadConverter.DeserializeClusterCreateRequest(payload);
            Assert.IsTrue(Equals(cluster1, cluster2));
        }

        private static bool Equals(CreateClusterRequest req1, CreateClusterRequest req2)
        {
            if (req1 == null && req2 == null)
            {
                return true;
            }
            if (req1 == null || req2 == null)
            {
                return false;
            }

            // Compares the properties and fails if there is a mismatch
            var comparisonTuples = new List<Tuple<object, object>>
            {
                new Tuple<object, object>(req1.ClusterUserName, req2.ClusterUserName),
                new Tuple<object, object>(req1.ClusterUserPassword, req2.ClusterUserPassword),
                new Tuple<object, object>(req1.DefaultAsvAccountKey, req2.DefaultAsvAccountKey),
                new Tuple<object, object>(req1.DefaultAsvAccountName, req2.DefaultAsvAccountName),
                new Tuple<object, object>(req1.DefaultAsvContainer, req2.DefaultAsvContainer),
                new Tuple<object, object>(req1.DnsName, req2.DnsName),
                new Tuple<object, object>(req1.Location, req2.Location),
                new Tuple<object, object>(req1.WorkerNodeCount, req2.WorkerNodeCount),
                new Tuple<object, object>(req1.AsvAccounts.Count, req2.AsvAccounts.Count),
            };
            if (req1.OozieMetastore != null)
            {
                if (req2.OozieMetastore == null)
                {
                    return false;
                }
                comparisonTuples.Add(new Tuple<object, object>(req1.OozieMetastore.Server, req2.OozieMetastore.Server));
                comparisonTuples.Add(new Tuple<object, object>(req1.OozieMetastore.Database, req2.OozieMetastore.Database));
                comparisonTuples.Add(new Tuple<object, object>(req1.OozieMetastore.User, req2.OozieMetastore.User));
                comparisonTuples.Add(new Tuple<object, object>(req1.OozieMetastore.Password, req2.OozieMetastore.Password));
            }
            if (req1.HiveMetastore != null)
            {
                if (req2.HiveMetastore == null)
                {
                    return false;
                }
                comparisonTuples.Add(new Tuple<object, object>(req1.HiveMetastore.Server, req2.HiveMetastore.Server));
                comparisonTuples.Add(new Tuple<object, object>(req1.HiveMetastore.Database, req2.HiveMetastore.Database));
                comparisonTuples.Add(new Tuple<object, object>(req1.HiveMetastore.User, req2.HiveMetastore.User));
                comparisonTuples.Add(new Tuple<object, object>(req1.HiveMetastore.Password, req2.HiveMetastore.Password));
            }
            if (!CompareTuples(comparisonTuples))
            {
                return false;
            }

            // Compares the AsvAccounts and fails if there is a mismatch
            if (req1.AsvAccounts.Any(asv1 => req2.AsvAccounts.Count(asv2 => asv1.AccountName == asv2.AccountName && asv1.Key == asv2.Key) != 1))
            {
                return false;
            }
            return true;
        }

        private static bool CompareTuples(IEnumerable<Tuple<object, object>> tuples)
        {
            if (tuples.Any(tuple => tuple.Item1 == null && tuple.Item2 != null))
            {
                return false;
            }
            if (tuples.Any(tuple => tuple.Item1 != null && tuple.Item2 == null))
            {
                return false;
            }

            tuples = tuples.Where(tuple => tuple.Item1 != null);
            return tuples.All(tuple => tuple.Item1.Equals(tuple.Item2));
        }

    }
}
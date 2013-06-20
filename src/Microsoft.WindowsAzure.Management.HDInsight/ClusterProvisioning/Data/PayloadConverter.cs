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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using Microsoft.WindowsAzure.Management.Framework.DynamicXml.Writer;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old;

    internal static class PayloadConverter
    {
        internal static Collection<HDInsightCluster> DeserializeListContainersResult(string payload, string deploymentNamespace)
        {
            var data = from svc in DeserializeFromXml<CloudServiceList>(payload)
                       from res in svc.Resources
                       where (res.ResourceProviderNamespace != null && res.ResourceProviderNamespace == deploymentNamespace) && (res.Type != null && res.Type == "containers")
                       select ListClusterContainerResult_FromInternal(res, svc);

            return new Collection<HDInsightCluster>(data.ToList());
        }

        internal static string SerializeListContainersResult(Collection<HDInsightCluster> containers, string deploymentNamespace)
        {
            var serviceList = new CloudServiceList();
            foreach (var containerGroup in containers.GroupBy(container => container.Location))
            {
                serviceList.Add(new CloudService()
                {
                    GeoRegion = containerGroup.Key,
                    Resources = new ResourceList(from container in containerGroup
                                                 select ListClusterContainerResult_ToInternal(container, deploymentNamespace))
                });
            }

            return serviceList.SerializeToXml();
        }

        internal static HDInsightClusterCreationDetails DeserializeClusterCreateRequest(string payload)
        {
            var resource = DeserializeFromXml<Resource>(payload);
            var createPayload = DeserializeFromXml<ClusterContainerPayload>(resource.IntrinsicSettings[0].OuterXml);
            return CreateClusterRequest_FromInternal(createPayload);
        }

        internal static string SerializeClusterCreateRequest(HDInsightClusterCreationDetails cluster, Guid subscriptionId)
        {
            return CreateClusterRequest_ToInternal(cluster, subscriptionId);
        }

        internal static T ExtractResourceOutputValue<T>(Resource res, string name)
        {
            if (res.OutputItems == null)
            {
                return default(T);
            }

            var value = res.OutputItems.FirstOrDefault(e => e.Key.Equals(name));
            if (value == null)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(value.Value, typeof(T), CultureInfo.InvariantCulture);
        }

        private static HDInsightClusterCreationDetails CreateClusterRequest_FromInternal(ClusterContainerPayload payloadObject)
        {
            var cluster = new HDInsightClusterCreationDetails
            {
                Location = payloadObject.AzureStorageLocation,
                Name = payloadObject.DnsName
            };

            cluster.UserName = payloadObject.Deployment.ClusterUsername;
            cluster.Password = payloadObject.Deployment.ClusterPassword;
            cluster.DefaultStorageAccountName = payloadObject.Deployment.ASVAccounts[0].AccountName;
            cluster.DefaultStorageAccountKey = payloadObject.Deployment.ASVAccounts[0].SecretKey;
            cluster.DefaultStorageContainer = payloadObject.Deployment.ASVAccounts[0].BlobContainerName;
            foreach (var asv in payloadObject.Deployment.ASVAccounts.Skip(1))
            {
                cluster.AdditionalStorageAccounts.Add(new StorageAccountConfiguration(asv.AccountName, asv.SecretKey));
            }

            var oozieMetaStore = payloadObject.Deployment.SqlMetaStores.FirstOrDefault(
                metastore => metastore.Type == SqlAzureMetaStorePayload.SqlMetastoreType.OozieMetastore);
            if (oozieMetaStore != null)
            {
                cluster.OozieMetastore = new HDInsightMetastore(oozieMetaStore.AzureServerName,
                                                                oozieMetaStore.DatabaseName,
                                                                oozieMetaStore.Username,
                                                                oozieMetaStore.Password);
            }

            var hiveMetaStore = payloadObject.Deployment.SqlMetaStores.FirstOrDefault(
                metastore => metastore.Type == SqlAzureMetaStorePayload.SqlMetastoreType.HiveMetastore);
            if (hiveMetaStore != null)
            {
                cluster.HiveMetastore = new HDInsightMetastore(hiveMetaStore.AzureServerName,
                                                               hiveMetaStore.DatabaseName,
                                                               hiveMetaStore.Username,
                                                               hiveMetaStore.Password);
            }

            var workernodes = from nodes in payloadObject.Deployment.NodeSizes
                              where nodes.RoleType == ClusterNodeType.DataNode
                              select nodes;
            cluster.ClusterSizeInNodes = workernodes.First().Count;
            return cluster;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "This is a result of interface flowing and not a true measure of complexity.")]
        private static string CreateClusterRequest_ToInternal(HDInsightClusterCreationDetails cluster, Guid subscriptionId)
        {
            dynamic dynaXml = DynaXmlBuilder.Create(false, Formatting.None);

            dynaXml.xmlns("http://schemas.microsoft.com/windowsazure")
                   .Resource
                   .b
                     .IntrinsicSettings
                     .b
                       .xmlns("http://schemas.datacontract.org/2004/07/Microsoft.ClusterServices.DataAccess.Context")
                       .ClusterContainer
                       .b
                         .AzureStorageLocation(cluster.Location)
                         .Deployment
                         .b
                           .ASVAccounts
                           .b
                             .ASVAccount
                             .b
                               .AccountName(cluster.DefaultStorageAccountName)
                               .BlobContainerName(cluster.DefaultStorageContainer)
                               .SecretKey(cluster.DefaultStorageAccountKey)
                             .d
                             .sp("asv")
                           .d
                           .ClusterPassword(cluster.Password)
                           .ClusterUsername(cluster.UserName)
                           .NodeSizes
                           .b
                             .ClusterNodeSize
                             .b
                               .Count(1)
                               .RoleType(ClusterNodeType.HeadNode)
                               .VMSize(NodeVMSize.ExtraLarge)
                             .d
                             .ClusterNodeSize
                             .b
                               .Count(cluster.ClusterSizeInNodes)
                               .RoleType(ClusterNodeType.DataNode)
                               .VMSize(NodeVMSize.Large)
                             .d
                           .d
                           .SqlMetaStores
                           .b
                             .xmlns("http://schemas.datacontract.org/2004/07/Microsoft.ClusterServices.DataAccess")
                             .sp("metastore")
                           .d
                           .Version(ClusterDeploymentPayload.DEFAULTVERSION)
                         .d
                         .DeploymentAction(AzureClusterDeploymentAction.Create)
                         .DnsName(cluster.Name)
                         .IncarnationID(Guid.NewGuid())
                         .SubscriptionId(subscriptionId)
                       .d
                     .d
                   .d
                   .End();

            dynaXml.rp("asv");
            foreach (var asv in cluster.AdditionalStorageAccounts)
            {
                dynaXml.ASVAccount
                       .b
                         .AccountName(asv.Name)
                         .BlobContainerName("deploymentcontainer")
                         .SecretKey(asv.Key)
                       .d
                       .End();
            }

            if (cluster.OozieMetastore != null)
            {
                dynaXml.rp("metastore")
                        .SqlAzureMetaStore
                        .b
                            .AzureServerName(cluster.OozieMetastore.Server)
                            .DatabaseName(cluster.OozieMetastore.Database)
                            .Password(cluster.OozieMetastore.Password)
                            .Type(SqlAzureMetaStorePayload.SqlMetastoreType.OozieMetastore)
                            .Username(cluster.OozieMetastore.User)
                        .d
                        .End();
            }
            if (cluster.HiveMetastore != null)
            {
                dynaXml.rp("metastore")
                        .SqlAzureMetaStore
                        .b
                            .AzureServerName(cluster.HiveMetastore.Server)
                            .DatabaseName(cluster.HiveMetastore.Database)
                            .Password(cluster.HiveMetastore.Password)
                            .Type(SqlAzureMetaStorePayload.SqlMetastoreType.HiveMetastore)
                            .Username(cluster.HiveMetastore.User)
                        .d
                        .End();
            }

            string xml;
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                dynaXml.Save(stream);
                stream.Position = 0;
                xml = reader.ReadToEnd();
            }
            return xml;

            //// Container with the basic info
            //var deployment = new ClusterDeploymentPayload()
            //{
            //    ClusterPassword = cluster.Password,
            //    ClusterUsername = cluster.UserName,
            //    Version = ClusterDeploymentPayload.DEFAULTVERSION
            //};

            //// Node information
            //deployment.NodeSizes.Add(new ClusterNodeSizePayload()
            //{
            //    Count = 1,
            //    RoleType = ClusterNodeType.HeadNode,
            //    VMSize = NodeVMSize.ExtraLarge
            //});
            //deployment.NodeSizes.Add(new ClusterNodeSizePayload()
            //{
            //    Count = cluster.ClusterSizeInNodes,
            //    RoleType = ClusterNodeType.DataNode,
            //    VMSize = NodeVMSize.Large
            //});

            //// ASV information
            //deployment.ASVAccounts.Add(new ASVAccountPayload()
            //{
            //    AccountName = cluster.DefaultStorageAccountName,
            //    SecretKey = cluster.DefaultStorageAccountKey,
            //    BlobContainerName = cluster.DefaultStorageContainer
            //});
            //foreach (var asv in cluster.AdditionalStorageAccounts)
            //{
            //    deployment.ASVAccounts.Add(new ASVAccountPayload() { AccountName = asv.Name, SecretKey = asv.Key, BlobContainerName = "deploymentcontainer" });
            //}

            //// Metastores
            //if (cluster.OozieMetastore != null)
            //{
            //    deployment.SqlMetaStores.Add(new SqlAzureMetaStorePayload
            //    {
            //        AzureServerName = cluster.OozieMetastore.Server,
            //        DatabaseName = cluster.OozieMetastore.Database,
            //        Password = cluster.OozieMetastore.Password,
            //        Username = cluster.OozieMetastore.User,
            //        Type = SqlAzureMetaStorePayload.SqlMetastoreType.OozieMetastore
            //    });
            //}
            //if (cluster.HiveMetastore != null)
            //{
            //    deployment.SqlMetaStores.Add(new SqlAzureMetaStorePayload
            //    {
            //        AzureServerName = cluster.HiveMetastore.Server,
            //        DatabaseName = cluster.HiveMetastore.Database,
            //        Password = cluster.HiveMetastore.Password,
            //        Username = cluster.HiveMetastore.User,
            //        Type = SqlAzureMetaStorePayload.SqlMetastoreType.HiveMetastore
            //    });
            //}

            //// Container information
            //var payloadObject = new ClusterContainerPayload()
            //{
            //    AzureStorageLocation = cluster.Location,
            //    DnsName = cluster.Name,
            //    SubscriptionId = subscriptionId,
            //    DeploymentAction = AzureClusterDeploymentAction.Create,
            //    Deployment = deployment,
            //    IncarnationID = Guid.NewGuid(),
            //};

            //var input = new Resource { IntrinsicSettings = new[] { payloadObject.SerializeToXmlNode() } };
            //return input.SerializeToXml();

            //// TODO: EXTRACT METADATA INFO
        }

        private static Resource ListClusterContainerResult_ToInternal(HDInsightCluster result, string nameSpace)
        {
            var resource = new Resource { Name = result.Name, SubState = result.StateString, ResourceProviderNamespace = nameSpace, Type = "containers" };
            if (result.Error != null)
            {
                resource.OperationStatus = new ResourceOperationStatus { Type = result.Error.OperationType };
                resource.OperationStatus.Error = new ResourceErrorInfo
                {
                    HttpCode = result.Error.HttpCode,
                    Message = result.Error.Message
                };
            }

            resource.OutputItems = new OutputItemList
            {
                new OutputItem { Key = "CreatedDate", Value = result.CreatedDate.ToString(CultureInfo.InvariantCulture) },
                new OutputItem { Key = "ConnectionURL", Value = result.ConnectionUrl },
                new OutputItem { Key = "ClusterUsername", Value = result.UserName },
                new OutputItem { Key = "NodesCount", Value = result.ClusterSizeInNodes.ToString(CultureInfo.InvariantCulture) }
            };

            return resource;
        }

        private static HDInsightCluster ListClusterContainerResult_FromInternal(Resource res, CloudService service)
        {
            // Extracts static properties from the Resource
            var c = new HDInsightCluster(res.Name, res.SubState) { Location = service.GeoRegion };

            // Extracts dynamic properties from the resource
            c.CreatedDate = ExtractResourceOutputValue<DateTime>(res, "CreatedDate");
            c.ConnectionUrl = ExtractResourceOutputValue<string>(res, "ConnectionURL");
            c.UserName = ExtractResourceOutputValue<string>(res, "ClusterUsername");
            c.ClusterSizeInNodes = ExtractResourceOutputValue<int>(res, "NodesCount");

            // Extracts a possible error status
            c.Error = res.OperationStatus == null || res.OperationStatus.Error == null
                ? null
                : new ClusterErrorStatus(res.OperationStatus.Error.HttpCode, res.OperationStatus.Error.Message, res.OperationStatus.Type);

            // Ideally, we want to serialize the Resource and put in a property for better debuggability

            return c;
        }

        private static T DeserializeFromXml<T>(string data) where T : new()
        {
            var ser = new DataContractSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return (T)ser.ReadObject(ms);
            }
        }

    }
}
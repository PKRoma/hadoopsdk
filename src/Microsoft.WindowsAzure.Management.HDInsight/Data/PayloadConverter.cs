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

namespace Microsoft.WindowsAzure.Management.HDInsight.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.WindowsAzure.Management.HDInsight.Old;

    internal static class PayloadConverter
    {
        internal static Collection<ListClusterContainerResult> DeserializeListContainersResult(string payload, string deploymentNamespace)
        {
            var data = from svc in DeserializeFromXml<CloudServiceList>(payload)
                       from res in svc.Resources
                       where (res.ResourceProviderNamespace != null && res.ResourceProviderNamespace == deploymentNamespace) && (res.Type != null && res.Type == "containers")
                       select ListClusterContainerResult_FromInternal(res, svc);

            return new Collection<ListClusterContainerResult>(data.ToList());
        }

        internal static string SerializeListContainersResult(Collection<ListClusterContainerResult> containers, string deploymentNamespace)
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

        internal static CreateClusterRequest DeserializeClusterCreateRequest(string payload)
        {
            var resource = DeserializeFromXml<Resource>(payload);
            var createPayload = DeserializeFromXml<ClusterContainerPayload>(resource.IntrinsicSettings[0].OuterXml);
            return CreateClusterRequest_FromInternal(createPayload);
        }

        internal static string SerializeClusterCreateRequest(CreateClusterRequest cluster, Guid subscriptionId)
        {
            var payloadObject = CreateClusterRequest_ToInternal(cluster, subscriptionId);
            var input = new Resource { IntrinsicSettings = new[] { payloadObject.SerializeToXmlNode() } };
            return input.SerializeToXml();
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

        private static CreateClusterRequest CreateClusterRequest_FromInternal(ClusterContainerPayload payloadObject)
        {
            var cluster = new CreateClusterRequest
            {
                Location = payloadObject.AzureStorageLocation,
                DnsName = payloadObject.DnsName
            };

            cluster.ClusterUserName = payloadObject.Deployment.ClusterUsername;
            cluster.ClusterUserPassword = payloadObject.Deployment.ClusterPassword;
            cluster.DefaultAsvAccountName = payloadObject.Deployment.ASVAccounts[0].AccountName;
            cluster.DefaultAsvAccountKey = payloadObject.Deployment.ASVAccounts[0].SecretKey;
            cluster.DefaultAsvContainer = payloadObject.Deployment.ASVAccounts[0].BlobContainerName;
            foreach (var asv in payloadObject.Deployment.ASVAccounts.Skip(1))
            {
                cluster.AsvAccounts.Add(new AsvAccountConfiguration(asv.AccountName, asv.SecretKey));
            }

            var oozieMetaStore = payloadObject.Deployment.SqlMetaStores.FirstOrDefault(
                metastore => metastore.Type == SqlAzureMetaStorePayload.SqlMetastoreType.OozieMetastore);
            if (oozieMetaStore != null)
            {
                cluster.OozieMetastore = new ComponentMetastore(oozieMetaStore.AzureServerName,
                                                                oozieMetaStore.DatabaseName,
                                                                oozieMetaStore.Username,
                                                                oozieMetaStore.Password);
            }

            var hiveMetaStore = payloadObject.Deployment.SqlMetaStores.FirstOrDefault(
                metastore => metastore.Type == SqlAzureMetaStorePayload.SqlMetastoreType.HiveMetastore);
            if (hiveMetaStore != null)
            {
                cluster.HiveMetastore = new ComponentMetastore(hiveMetaStore.AzureServerName,
                                                               hiveMetaStore.DatabaseName,
                                                               hiveMetaStore.Username,
                                                               hiveMetaStore.Password);
            }

            var workernodes = from nodes in payloadObject.Deployment.NodeSizes
                              where nodes.RoleType == ClusterNodeType.DataNode
                              select nodes;
            cluster.WorkerNodeCount = workernodes.First().Count;
            return cluster;
        }

        private static ClusterContainerPayload CreateClusterRequest_ToInternal(CreateClusterRequest cluster, Guid subscriptionId)
        {
            // Container with the basic info
            var deployment = new ClusterDeploymentPayload()
            {
                ClusterPassword = cluster.ClusterUserPassword,
                ClusterUsername = cluster.ClusterUserName,
                Version = ClusterDeploymentPayload.DEFAULTVERSION
            };

            // Node information
            deployment.NodeSizes.Add(new ClusterNodeSizePayload()
            {
                Count = 1,
                RoleType = ClusterNodeType.HeadNode,
                VMSize = NodeVMSize.ExtraLarge
            });
            deployment.NodeSizes.Add(new ClusterNodeSizePayload()
            {
                Count = cluster.WorkerNodeCount,
                RoleType = ClusterNodeType.DataNode,
                VMSize = NodeVMSize.Large
            });

            // ASV information
            deployment.ASVAccounts.Add(new ASVAccountPayload()
            {
                AccountName = cluster.DefaultAsvAccountName,
                SecretKey = cluster.DefaultAsvAccountKey,
                BlobContainerName = cluster.DefaultAsvContainer
            });
            foreach (var asv in cluster.AsvAccounts)
            {
                deployment.ASVAccounts.Add(new ASVAccountPayload() { AccountName = asv.AccountName, SecretKey = asv.Key, BlobContainerName = "deploymentcontainer" });
            }

            // Metastores
            if (cluster.OozieMetastore != null)
            {
                deployment.SqlMetaStores.Add(new SqlAzureMetaStorePayload
                {
                    AzureServerName = cluster.OozieMetastore.Server,
                    DatabaseName = cluster.OozieMetastore.Database,
                    Password = cluster.OozieMetastore.Password,
                    Username = cluster.OozieMetastore.User,
                    Type = SqlAzureMetaStorePayload.SqlMetastoreType.OozieMetastore
                });
            }
            if (cluster.HiveMetastore != null)
            {
                deployment.SqlMetaStores.Add(new SqlAzureMetaStorePayload
                {
                    AzureServerName = cluster.HiveMetastore.Server,
                    DatabaseName = cluster.HiveMetastore.Database,
                    Password = cluster.HiveMetastore.Password,
                    Username = cluster.HiveMetastore.User,
                    Type = SqlAzureMetaStorePayload.SqlMetastoreType.HiveMetastore
                });
            }

            // Container information
            return new ClusterContainerPayload()
            {
                AzureStorageLocation = cluster.Location,
                DnsName = cluster.DnsName,
                SubscriptionId = subscriptionId,
                DeploymentAction = AzureClusterDeploymentAction.Create,
                Deployment = deployment,
                IncarnationID = Guid.NewGuid(),
            };

            // TODO: EXTRACT METADATA INFO
        }

        private static Resource ListClusterContainerResult_ToInternal(ListClusterContainerResult result, string nameSpace)
        {
            var resource = new Resource { Name = result.DnsName, SubState = result.State, ResourceProviderNamespace = nameSpace, Type = "containers" };
            if (result.ResultError != null)
            {
                resource.OperationStatus = new ResourceOperationStatus { Type = result.ResultError.OperationType };
                resource.OperationStatus.Error = new ResourceErrorInfo
                {
                    HttpCode = result.ResultError.HttpCode,
                    Message = result.ResultError.Message
                };
            }

            resource.OutputItems = new OutputItemList
            {
                new OutputItem { Key = "CreatedDate", Value = result.CreatedDate.ToString(CultureInfo.InvariantCulture) },
                new OutputItem { Key = "ConnectionURL", Value = result.ConnectionUrl },
                new OutputItem { Key = "ClusterUsername", Value = result.UserName },
                new OutputItem { Key = "NodesCount", Value = result.WorkerNodesCount.ToString(CultureInfo.InvariantCulture) }
            };

            return resource;
        }

        private static ListClusterContainerResult ListClusterContainerResult_FromInternal(Resource res, CloudService service)
        {
            // Extracts static properties from the Resource
            var c = new ListClusterContainerResult(res.Name, res.SubState) { Location = service.GeoRegion };

            // Extracts dynamic properties from the resource
            c.CreatedDate = ExtractResourceOutputValue<DateTime>(res, "CreatedDate");
            c.ConnectionUrl = ExtractResourceOutputValue<string>(res, "ConnectionURL");
            c.UserName = ExtractResourceOutputValue<string>(res, "ClusterUsername");
            c.WorkerNodesCount = ExtractResourceOutputValue<int>(res, "NodesCount");

            // Extracts a possible error status
            c.ResultError = res.OperationStatus == null || res.OperationStatus.Error == null
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
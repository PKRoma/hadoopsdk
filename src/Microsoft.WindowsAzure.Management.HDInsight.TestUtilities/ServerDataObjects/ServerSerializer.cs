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
namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.ServerDataObjects
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.HDInsight.Management.Contracts.May2013;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.ServerDataObjects.Rdfe;

    internal static class ServerSerializer
    {
        private const string WindowsAzureNamespace = "http://schemas.microsoft.com/windowsazure";
        private static readonly XName ResourceElementName = XName.Get("Resource", WindowsAzureNamespace);
        private static readonly XName OutputItemElementName = XName.Get("OutputItem", WindowsAzureNamespace);
        private static readonly XName OutputItemsElementName = XName.Get("OutputItems", WindowsAzureNamespace);
        private static readonly XName IntrinsicSettingsElementName = XName.Get("IntrinsicSettings", WindowsAzureNamespace);
        private static readonly XName KeyElementName = XName.Get("Key", WindowsAzureNamespace);
        private static readonly XName ValueElementName = XName.Get("Value", WindowsAzureNamespace);
        private const string ClusterUserName = "ClusterUsername";
        private const string HttpUserName = "Http_Username";
        private const string HttpPassword = "Http_Password";
        private const string RdpUserName = "RDP_Username";
        private const string NodesCount = "NodesCount";
        private const string ConnectionUrl = "ConnectionURL";
        private const string CreatedDate = "CreatedDate";
        private const string Version = "Version";
        private const string BlobContainers = "BlobContainers";
        private const string ExtendedErrorMessage = "ExtendedErrorMessage";

        internal static string SerializeListContainersResult(IEnumerable<ClusterDetails> containers, string deploymentNamespace, bool writeError, bool writeExtendedError)
        {
            var serviceList = new CloudServiceList();
            foreach (var containerGroup in containers.GroupBy(container => container.Location))
            {
                serviceList.Add(new CloudService()
                {
                    GeoRegion = containerGroup.Key,
                    Resources = new ResourceList(from container in containerGroup
                                                 select ListClusterContainerResult_ToInternal(container, deploymentNamespace, writeError, writeExtendedError))
                });
            }

            return serviceList.SerializeToXml();
        }

        internal static Resource DeserializeClusterCreateRequestIntoResource(string payload)
        {
            return DeserializeFromXml<Resource>(payload);
        }

        internal static ClusterCreateParameters DeserializeClusterCreateRequest(string payload)
        {
            var resource = DeserializeFromXml<Resource>(payload);
            var createPayload = DeserializeFromXml<ClusterContainer>(resource.IntrinsicSettings[0].OuterXml);
            return CreateClusterRequest_FromInternal(createPayload);
        }

        internal static ClusterContainer DeserializeClusterCreateRequestToInternal(string payload)
        {
            var resource = DeserializeFromXml<Resource>(payload);
            var createPayload = DeserializeFromXml<ClusterContainer>(resource.IntrinsicSettings[0].OuterXml);
            return createPayload;
        }

        internal static XElement SerializeResource(Resource resource)
        {
            var xdoc = new XDocument();
            xdoc.Add(
                new XElement(ResourceElementName,
                    new XElement(OutputItemsElementName,
                        from outputItem in resource.OutputItems
                        select new XElement(OutputItemElementName,
                            new XElement(KeyElementName, outputItem.Key),
                            new XElement(ValueElementName, outputItem.Value)))));

            return xdoc.Root;
        }

        internal static XElement SerializeResource<TIntrinsic>(Resource resource, IEnumerable<TIntrinsic> intrinsicSettings)
        {
            var intrinsicTextNode = SerializeToJson(intrinsicSettings);
            var xdoc = new XDocument();
            xdoc.Add(
                new XElement(ResourceElementName,
                    new XElement(IntrinsicSettingsElementName, intrinsicTextNode),
                    new XElement(OutputItemsElementName,
                        from outputItem in resource.OutputItems
                        select new XElement(OutputItemElementName,
                            new XElement(KeyElementName, outputItem.Key),
                            new XElement(ValueElementName, outputItem.Value)))));

            return xdoc.Root;
        }

        internal static object SerializeXml(XmlNode xNode)
        {
            var ser = new DataContractSerializer(typeof(XmlNode[]));
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, new XmlNode[] { xNode });
                ms.Seek(0, SeekOrigin.Begin);
                return new StreamReader(ms).ReadToEnd();
            }
        }

        public static XmlNode SerializeToXmlNode<T>(T o)
        {
            var objAsJson = SerializeToJson(o);
            var doc = new XmlDocument();
            //var readerSettings = new XmlReaderSettings();
            //readerSettings.DtdProcessing = DtdProcessing.Prohibit;
            return doc.CreateTextNode(objAsJson);
        }

        public static string SerializeToJson<T>(T o)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, o);
                ms.Seek(0, SeekOrigin.Begin);
                return new StreamReader(ms).ReadToEnd();
            }
        }

        private static ClusterCreateParameters CreateClusterRequest_FromInternal(ClusterContainer payloadObject)
        {
            var cluster = new ClusterCreateParameters
            {
                Location = payloadObject.Region,
                Name = payloadObject.ClusterName
            };

            cluster.UserName = payloadObject.Deployment.ClusterUsername;
            cluster.Password = payloadObject.Deployment.ClusterPassword;
            cluster.Version = payloadObject.Deployment.Version;
            cluster.DefaultStorageAccountName = payloadObject.StorageAccounts[0].AccountName;
            cluster.DefaultStorageAccountKey = payloadObject.StorageAccounts[0].Key;
            cluster.DefaultStorageContainer = payloadObject.StorageAccounts[0].BlobContainerName;
            foreach (var asv in payloadObject.StorageAccounts.Skip(1))
            {
                cluster.AdditionalStorageAccounts.Add(new WabStorageAccountConfiguration(asv.AccountName, asv.Key));
            }

            if (payloadObject.Settings != null)
            {
                CopyConfiguration(payloadObject, cluster);

                if (payloadObject.Settings.Oozie != null)
                {
                    if (payloadObject.Settings.Oozie.Catalog != null)
                    {
                        var oozieMetaStore = payloadObject.Settings.Oozie.Catalog;
                        cluster.OozieMetastore = new Metastore(oozieMetaStore.Server,
                                                                        oozieMetaStore.DatabaseName,
                                                                        oozieMetaStore.Username,
                                                                        oozieMetaStore.Password);
                    }
                }

                if (payloadObject.Settings.Hive != null)
                {
                    if (payloadObject.Settings.Hive.Catalog != null)
                    {
                        var hiveMetaStore = payloadObject.Settings.Hive.Catalog;
                        cluster.HiveMetastore = new Metastore(hiveMetaStore.Server,
                                                                       hiveMetaStore.DatabaseName,
                                                                       hiveMetaStore.Username,
                                                                       hiveMetaStore.Password);
                    }
                }
            }

            cluster.ClusterSizeInNodes = payloadObject.Deployment.TotalNodeCount;
            return cluster;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This complexity is needed to handle all the types in the submit payload.")]
        private static void CopyConfiguration(ClusterContainer payloadObject, ClusterCreateParameters cluster)
        {
            if (payloadObject.Settings.Core != null && payloadObject.Settings.Core.Configuration != null)
            {
                cluster.CoreConfiguration.AddRange(
                    payloadObject.Settings.Core.Configuration.Select(config => new KeyValuePair<string, string>(config.Name, config.Value)));
            }

            if (payloadObject.Settings.Hive != null)
            {
                if (payloadObject.Settings.Hive.AdditionalLibraries != null)
                {
                    cluster.HiveConfiguration.AdditionalLibraries =
                        new WabStorageAccountConfiguration(
                            payloadObject.Settings.Hive.AdditionalLibraries.AccountName,
                            payloadObject.Settings.Hive.AdditionalLibraries.Key,
                            payloadObject.Settings.Hive.AdditionalLibraries.BlobContainerName);
                }

                if (payloadObject.Settings.Hive.Configuration != null)
                {
                    cluster.HiveConfiguration.ConfigurationCollection.AddRange(
                        payloadObject.Settings.Hive.Configuration.Select(config => new KeyValuePair<string, string>(config.Name, config.Value)));
                }
            }

            if (payloadObject.Settings.Hdfs != null && payloadObject.Settings.Hdfs.Configuration != null)
            {
                cluster.HdfsConfiguration.AddRange(
                    payloadObject.Settings.Hdfs.Configuration.Select(config => new KeyValuePair<string, string>(config.Name, config.Value)));
            }

            if (payloadObject.Settings.MapReduce != null && payloadObject.Settings.MapReduce.Configuration != null)
            {
                cluster.MapReduceConfiguration = new HDInsight.MapReduceConfiguration();

                if (payloadObject.Settings.MapReduce.Configuration != null)
                {
                    cluster.MapReduceConfiguration.ConfigurationCollection.AddRange(
                        payloadObject.Settings.MapReduce.Configuration.Select(config => new KeyValuePair<string, string>(config.Name, config.Value)));
                }

                if (payloadObject.Settings.MapReduce.CapacitySchedulerConfiguration != null)
                {
                    cluster.MapReduceConfiguration.CapacitySchedulerConfigurationCollection.AddRange(
                        payloadObject.Settings.MapReduce.CapacitySchedulerConfiguration.Select(config => new KeyValuePair<string, string>(config.Name, config.Value)));
                }
            }

            if (payloadObject.Settings.Oozie != null && payloadObject.Settings.Oozie.Configuration != null)
            {
                if (cluster.OozieConfiguration.ConfigurationCollection != null)
                {
                    cluster.OozieConfiguration.ConfigurationCollection.AddRange(
                        payloadObject.Settings.Oozie.Configuration.Select(config => new KeyValuePair<string, string>(config.Name, config.Value)));
                }

                if (payloadObject.Settings.Oozie.AdditionalSharedLibraries != null)
                {
                    cluster.OozieConfiguration.AdditionalSharedLibraries =
                       new WabStorageAccountConfiguration(
                           payloadObject.Settings.Oozie.AdditionalSharedLibraries.AccountName,
                           payloadObject.Settings.Oozie.AdditionalSharedLibraries.Key,
                           payloadObject.Settings.Oozie.AdditionalSharedLibraries.BlobContainerName);
                }

                if (payloadObject.Settings.Oozie.AdditionalActionExecutorLibraries != null)
                {
                    cluster.OozieConfiguration.AdditionalActionExecutorLibraries =
                       new WabStorageAccountConfiguration(
                           payloadObject.Settings.Oozie.AdditionalActionExecutorLibraries.AccountName,
                           payloadObject.Settings.Oozie.AdditionalActionExecutorLibraries.Key,
                           payloadObject.Settings.Oozie.AdditionalActionExecutorLibraries.BlobContainerName);
                }
            }
        }

        private static Resource ListClusterContainerResult_ToInternal(ClusterDetails result, string nameSpace, bool writeError, bool writeExtendedError)
        {
            var resource = new Resource { Name = result.Name, SubState = result.StateString, ResourceProviderNamespace = nameSpace, Type = "containers" };
            if (result.AdditionalStorageAccounts == null)
            {
                result.AdditionalStorageAccounts = new List<WabStorageAccountConfiguration>();
            }

            resource.OutputItems = new OutputItemList
            {
                new OutputItem { Key = CreatedDate, Value = result.CreatedDate.ToString(CultureInfo.InvariantCulture) },
                new OutputItem { Key = ConnectionUrl, Value = result.ConnectionUrl },
                new OutputItem { Key = ClusterUserName, Value = result.HttpUserName },
                new OutputItem { Key = Version, Value = result.Version },
                new OutputItem { Key = BlobContainers, Value = SerializeStorageAccounts(result) },
                new OutputItem { Key = NodesCount, Value = result.ClusterSizeInNodes.ToString(CultureInfo.InvariantCulture) }
            };

            if (result.Error != null)
            {
                if (writeError)
                {
                    resource.OperationStatus = new ResourceOperationStatus { Type = result.Error.OperationType };
                    resource.OperationStatus.Error = new ResourceErrorInfo { HttpCode = result.Error.HttpCode, Message = result.Error.Message };
                }

                if (writeExtendedError)
                {
                    resource.OperationStatus = new ResourceOperationStatus { Type = result.Error.OperationType };
                    resource.OperationStatus.Error = new ResourceErrorInfo { HttpCode = result.Error.HttpCode, Message = result.Error.Message };
                    resource.OutputItems.Add(new OutputItem { Key = ExtendedErrorMessage, Value = result.Error.Message });
                    resource.Type = result.Error.OperationType;
                }
            }

            var intrinsicSettings = new List<OutputItem> 
            {
                new OutputItem { Key = RdpUserName, Value = result.RdpUserName },
                new OutputItem { Key = HttpUserName, Value = result.HttpUserName },
                new OutputItem { Key = HttpPassword, Value = result.HttpPassword },
                new OutputItem { Key = Version, Value = result.Version },
            };

            resource.IntrinsicSettings = new XmlNode[] { SerializeToXmlNode(intrinsicSettings) };
            return resource;
        }

        private static string SerializeStorageAccounts(ClusterDetails cluster)
        {
            var blobContainerReferences = new List<BlobContainerSerializedAsJson>();
            if (cluster.DefaultStorageAccount != null)
            {
                blobContainerReferences.Add(
                    new BlobContainerSerializedAsJson()
                    {
                        AccountName = cluster.DefaultStorageAccount.Name,
                        Key = cluster.DefaultStorageAccount.Key,
                        Container = cluster.DefaultStorageAccount.Container
                    });
            }

            blobContainerReferences.AddRange(cluster.AdditionalStorageAccounts.Select(
                        acc => new BlobContainerSerializedAsJson() { AccountName = acc.Name, Key = acc.Key, Container = acc.Container }));

            return SerializeToJson(blobContainerReferences.ToList());
        }

        private static T DeserializeFromXml<T>(string data) where T : new()
        {
            var ser = new DataContractSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return (T)ser.ReadObject(ms);
            }
        }

        [DataContract]
        internal class BlobContainerSerializedAsJson
        {
            [DataMember]
            public string Key { get; set; }

            [DataMember]
            public string AccountName { get; set; }

            [DataMember]
            public string Container { get; set; }
        }
    }
}

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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    internal class AzureHdInsightPowerShellConstants
    {
        public const string Exec = "Exec";
        public const string Hive = "Hive";
        public const string MapReduce = "MapReduce";
        public const string HiveCmd = "hive";
        public const string HiveCmdExecute = "execute";
        public const string HiveCmdExecuteAlias = "e";
        public const string AzureHDInsightCluster = "AzureHDInsightCluster";
        public const string AzureHDInsightHttpServicesAccess = "AzureHDInsightHttpServicesAccess";
        public const string AzureHDInsightProperties = "AzureHDInsightProperties";
        public const string AzureHDInsightClusterConfig = "AzureHDInsightClusterConfig";
        public const string AzureHDInsightConfigValues = "AzureHDInsightConfigValues";
        public const string AzureHDInsightDefaultStorage = "AzureHDInsightDefaultStorage";
        public const string AzureHDInsightStorage = "AzureHDInsightStorage";
        public const string AzureHDInsightMetastore = "AzureHDInsightMetastore";
        public const string AzureHDInsightJobs = "AzureHDInsightJob";
        public const string AzureHDInsightJobOutput = "AzureHDInsightJobOutput";
        public const string AzureHDInsightMapReduceJobDefinition = "AzureHDInsightMapReduceJobDefinition";
        public const string AzureHDInsightStreamingMapReduceJobDefinition = "AzureHDInsightStreamingMapReduceJobDefinition";
        public const string AzureHDInsightHiveJobDefinition = "AzureHDInsightHiveJobDefinition";
        public const string AzureHDInsightSqoopJobDefinition = "AzureHDInsightSqoopJobDefinition";
        public const string AzureHDInsightPigJobDefinition = "AzureHDInsightPigJobDefinition";

        public const string JobDefinition = "jobDetails";
        public const string JobId = "Id";
        public const string Jobs = "Jobs";
        public const string Skip = "Skip";
        public const string Show = "Show";
        public const string FromDateTime = "From";
        public const string ToDateTime = "To";

        public const string AliasDnsName = "DnsName";
        public const string AliasClusterName = "ClusterName";

        public const string AliasTaskLogsDirectory = "LogsDir";
        public const string AliasCredentials = "Cred";
        public const string AliasEndpoint = "Endpoint";
        public const string AliasCert = "Cert";
        public const string AliasSub = "Sub";
        public const string AliasSubscription = "Subscription";

        public const string AliasLoc = "Loc";
        public const string AliasVersion = "Ver";

        public const string AliasStorageAccount = "StorageAccount";
        public const string AliasStorageKey = "StorageKey";
        public const string AliasStorageContainer = "StorageContainer";

        public const string AliasSize = "Size";
        public const string AliasNodes = "Nodes";

        public const string AliasJarFile = "Jar";
        public const string AliasQuery = "QueryText";
        public const string AliasQueryFile = "QueryFile";
        public const string AliasClassName = "Class";
        public const string AliasParameters = "Params";
        public const string AliasArguments = "Args";
        public const string AliasJobName = "Name";

        public const string AliasInput = "Input";
        public const string AliasOutput = "Output";
        public const string AliasInputFormat = "InputFormat";
        public const string AliasOutputFormat = "OutputFormat";
        public const string AliasPartitioner = "Partitioner";
        public const string AliasInputReader = "InputReader";

        public const string ParameterSetAddMetastore = "Add Metastore";
        public const string ParameterSetDefaultStorageAccount = "Set Default Storage Account";
        public const string ParameterSetAddStorageAccount = "Add Storage Account";
        public const string ParameterSetClusterByNameWithSpecificSubscriptionCredentials = "Cluster By Name (with Specific Subscription Credential)";
        public const string ParameterSetJobHistoryByName = "Get jobDetails History of a HDInsight Cluster";
        public const string ParameterSetJobHistoryByNameAndJobId = "Get jobDetails History for a specific jobDetails in a HDInsight Cluster";
        public const string ParameterSetJobHistoryByNameWithSpecificSubscriptionCredentials = "Get jobDetails History of a HDInsight Cluster (with Specific Subscription Credential)";
        public const string ParameterSetStartJobByName = "Start jobDetails on an HDInsight Cluster";
        public const string ParameterSetStartJobByNameWithSpecificSubscriptionCredentials = "Start jobDetails on an HDInsight Cluster (with Specific Subscription Credential)";

        public const string ParameterSetClusterByConfigWithSpecificSubscriptionCredentials = "Cluster By Config (with Specific Subscription Credential)";
        public const string ParameterSetConfigClusterSizeInNodesOnly = "Config ClusterSizeInNodes Only";
    }
}

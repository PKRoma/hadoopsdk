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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    internal class AzureHDInsightCommandFactory : IAzureHDInsightCommandFactory
    {
        public IGetAzureHDInsightClusterCommand CreateGet()
        {
            return Help.SafeCreate<GetAzureHDInsightClusterCommand>();
        }

        public IWaitAzureHDInsightJobCommand CreateWaitJobs()
        {
            return Help.SafeCreate<WaitAzureHDInsightJobCommand>();
        }

        public IUseAzureHDInsightClusterCommand CreateUseCluster()
        {
            return Help.SafeCreate<UseAzureHDInsightClusterCommand>();
        }

        public IInvokeHiveCommand CreateInvokeHive()
        {
            return new InvokeHiveCommand();
        }

        public IGetAzureHDInsightPropertiesCommand CreateGetProperties()
        {
            return Help.SafeCreate<GetAzureHDInsightPropertiesCommand>();
        }

        public INewAzureHDInsightClusterCommand CreateCreate()
        {
            return Help.SafeCreate<NewAzureHDInsightClusterCommand>();
        }

        public IRemoveAzureHDInsightClusterCommand CreateDelete()
        {
            return Help.SafeCreate<RemoveAzureHDInsightClusterCommand>();
        }

        public INewAzureHDInsightClusterConfigCommand CreateNewConfig()
        {
            return Help.SafeCreate<NewAzureHDInsightClusterConfigCommand>();
        }

        public IAddAzureHDInsightConfigValuesCommand CreateAddConfig()
        {
            return Help.SafeCreate<AddAzureHDInsightConfigValuesCommand>();
        }

        public ISetAzureHDInsightDefaultStorageCommand CreateSetDefaultStorage()
        {
            return Help.SafeCreate<SetAzureHDInsightDefaultStorageCommand>();
        }

        public IAddAzureHDInsightStorageCommand CreateAddStorage()
        {
            return new AddAzureHDInsightStorageCommand();
        }

        public IAddAzureHDInsightMetastoreCommand CreateAddMetastore()
        {
            return Help.SafeCreate<AddAzureHDInsightMetastoreCommand>();
        }

        public IGetAzureHDInsightJobCommand CreateGetJobs()
        {
            return Help.SafeCreate<GetAzureHDInsightJobCommand>();
        }

        public IGetAzureHDInsightJobOutputCommand CreateGetJobOutput()
        {
            return Help.SafeCreate<GetAzureHDInsightJobOutputCommand>();
        }

        public IStopAzureHDInsightJobCommand CreateStopJob()
        {
            return Help.SafeCreate<StopAzureHDInsightJobCommand>();
        }

        public INewAzureHDInsightMapReduceJobDefinitionCommand CreateNewMapReduceDefinition()
        {
            return Help.SafeCreate<NewAzureHDInsightMapReduceJobJobDefinitionCommand>();
        }

        public INewAzureHDInsightStreamingJobDefinitionCommand CreateNewStreamingMapReduceDefinition()
        {
            return Help.SafeCreate<NewAzureHDInsightStreamingJobDefinitionCommand>();
        }

        public INewAzureHDInsightHiveJobDefinitionCommand CreateNewHiveDefinition()
        {
            return Help.SafeCreate<NewAzureHDInsightHiveJobDefinitionCommand>();
        }

        public INewAzureHDInsightSqoopJobDefinitionCommand CreateNewSqoopDefinition()
        {
            return Help.SafeCreate<NewAzureHDInsightSqoopJobDefinitionCommand>();
        }

        public INewAzureHDInsightPigJobDefinitionCommand CreateNewPigJobDefinition()
        {
            return Help.SafeCreate<NewAzureHDInsightPigJobDefinitionCommand>();
        }

        public IStartAzureHDInsightJobCommand CreateStartJob()
        {
            return Help.SafeCreate<StartAzureHDInsightJobCommand>();
        }

        public IManageAzureHDInsightHttpAccessCommand CreateManageHttpAccess()
        {
            return Help.SafeCreate<ManageAzureHDInsightHttpAccessCommand>();
        }
    }
}
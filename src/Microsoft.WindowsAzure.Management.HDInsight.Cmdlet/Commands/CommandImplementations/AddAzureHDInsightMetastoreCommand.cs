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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations
{
    using System.Management.Automation;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;

    internal class AddAzureHDInsightMetastoreCommand : AzureHDInsightCommand<AzureHDInsightConfig>, IAddAzureHDInsightMetastoreCommand
    {
        private readonly AzureHDInsightMetastore metastore = new AzureHDInsightMetastore();

        public AddAzureHDInsightMetastoreCommand()
        {
            this.Config = new AzureHDInsightConfig();
        }

        public AzureHDInsightConfig Config { get; set; }

        public PSCredential Credential
        {
            get { return this.metastore.Credential; }
            set { this.metastore.Credential = value; }
        }

        public string DatabaseName
        {
            get { return this.metastore.DatabaseName; }
            set { this.metastore.DatabaseName = value; }
        }

        public AzureHDInsightMetastoreType MetastoreType
        {
            get { return this.metastore.MetastoreType; }
            set { this.metastore.MetastoreType = value; }
        }

        public string SqlAzureServerName
        {
            get { return this.metastore.SqlAzureServerName; }
            set { this.metastore.SqlAzureServerName = value; }
        }

        public override Task EndProcessing()
        {
            if (this.MetastoreType == AzureHDInsightMetastoreType.HiveMetastore)
            {
                this.Config.HiveMetastore = this.metastore;
            }
            else
            {
                this.Config.OozieMetastore = this.metastore;
            }
            this.Output.Add(this.Config);
            return TaskEx.GetCompletedTask();
        }
    }
}

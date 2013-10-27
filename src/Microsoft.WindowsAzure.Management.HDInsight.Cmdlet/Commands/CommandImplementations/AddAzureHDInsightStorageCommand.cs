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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;

    /// <summary>
    /// Adds a storage account to the HDInsight cluster configuration.
    /// </summary>
    internal class AddAzureHDInsightStorageCommand : IAddAzureHDInsightStorageCommand
    {
        /// <summary>
        /// Initializes a new instance of the AddAzureHDInsightStorageCommand class.
        /// </summary>
        public AddAzureHDInsightStorageCommand()
        {
            this.Config = new AzureHDInsightConfig();
            this.Output = new Collection<AzureHDInsightConfig>();
        }

        public Task EndProcessing()
        {
            var account = new AzureHDInsightStorageAccount();
            account.StorageAccountName = this.StorageAccountName;
            account.StorageAccountKey = this.StorageAccountKey;
            this.Config.AdditionalStorageAccounts.Add(account);
            this.Output.Add(this.Config);
            return TaskEx.GetCompletedTask();
        }

        public void Cancel()
        {
        }

        public CancellationToken CancellationToken
        {
            get { return CancellationToken.None; }
        }

        public ICollection<AzureHDInsightConfig> Output { get; private set; }

        public AzureHDInsightConfig Config { get; set; }

        public string StorageAccountName { get; set; }

        public string StorageAccountKey { get; set; }
    }
}

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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects
{
    /// <summary>
    /// Represents a Default Storage Account used for an HDInsight cluster.
    /// </summary>
    public class AzureHDInsightDefaultStorageAccount : AzureHDInsightStorageAccount
    {
        /// <summary>
        /// Gets or sets the Storage Container for the Default Storage Account.
        /// </summary>
        public string StorageContainerName { get; set; }

        /// <summary>
        /// Creates an SDK object from this Powershell object type.
        /// </summary>
        /// <returns>A storage account configuration.</returns>
        public WabStorageAccountConfiguration ToWabStorageAccountConfiguration()
        {
            return new WabStorageAccountConfiguration(this.StorageAccountName, this.StorageAccountKey, this.StorageContainerName);
        }
    }
}

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
    ///     Represents an Azure HD Insight Cluster connection object for the PowerShell Cmdlets.
    /// </summary>
    public class AzureHDInsightClusterConnection
    {
        /// <summary>
        ///     Gets or sets the Azure HDInsight cluster we're connected to.
        /// </summary>
        public AzureHDInsightCluster Cluster { get; set; }

        /// <summary>
        ///     Gets or sets the subscription credentials for the connection.
        /// </summary>
        public IHDInsightCertificateCredential Credential { get; set; }
    }
}

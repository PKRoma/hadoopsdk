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
    using System.Management.Automation;

    /// <summary>
    /// Represents an AzureHDInsightMetastore.
    /// </summary>
    public class AzureHDInsightMetastore
    {
        /// <summary>
        /// Gets or sets the Azure SQL Server for the metastore.
        /// </summary>
        public string SqlAzureServerName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server user credentials.
        /// </summary>
        public PSCredential Credential { get; set; }

        /// <summary>
        /// Gets or sets the type of metastore represented by this object.
        /// </summary>
        public AzureHDInsightMetastoreType MetastoreType { get; set; }
    }
}

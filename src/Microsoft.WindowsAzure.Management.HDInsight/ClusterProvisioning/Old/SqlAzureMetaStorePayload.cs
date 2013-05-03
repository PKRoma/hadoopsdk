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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old
{
    using System.Runtime.Serialization;

    [DataContract(Name = "SqlAzureMetaStore", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.ClusterServices.DataAccess")]
    internal class SqlAzureMetaStorePayload : Payload
    {
        internal enum SqlMetastoreType
        {
            HiveMetastore,
            OozieMetastore
        }

        [DataMember]
        internal string AzureServerName { get; set; }

        [DataMember]
        internal string Username { get; set; }

        [DataMember]
        internal string Password { get; set; }

        [DataMember]
        internal string DatabaseName { get; set; }

        [DataMember]
        internal SqlMetastoreType Type { get; set; }
    }
}
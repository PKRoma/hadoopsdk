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
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "ClusterContainer",
        Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.ClusterServices.DataAccess.Context")]
    internal class ClusterContainerPayload : Payload
    {
        [DataMember(EmitDefaultValue = true)]
        internal ClusterDeploymentPayload Deployment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        internal string DnsName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        internal Guid SubscriptionId { get; set; }

        [DataMember(EmitDefaultValue = true)]
        internal string ContainerState { get; set; }

        [DataMember(EmitDefaultValue = true)]
        internal string ContainerError { get; set; }

        [DataMember(EmitDefaultValue = false)]
        internal string AzureStorageLocation { get; set; }

        [DataMember(EmitDefaultValue = true)]
        internal string AzureStorageAccount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        internal DateTime CreatedDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        internal DateTime UpdatedDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        internal Guid IncarnationID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        internal AzureClusterDeploymentAction DeploymentAction { get; set; }
    }

    internal enum AzureClusterDeploymentAction
    {
        None,
        Create,
        Delete
    }
}
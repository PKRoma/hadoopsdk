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

    /// <summary>
    ///     Represents the size of a Cluster during provisioning time.
    /// </summary>
    [DataContract(Name = "ClusterNodeSize",
        Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.ClusterServices.DataAccess.Context")]
    internal class ClusterNodeSizePayload : Payload
    {
        /// <summary>
        ///     Gets or sets represents the # of nodes.
        /// </summary>
        [DataMember]
        internal int Count { get; set; }

        /// <summary>
        ///     Gets or sets represents the ClusterNodeType.
        /// </summary>
        [DataMember]
        internal ClusterNodeType RoleType { get; set; }

        /// <summary>
        ///     Gets or sets represents the Size of the node.
        /// </summary>
        [DataMember]
        internal NodeVMSize VMSize { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ClusterNodeSizePayload;
            if (other == null)
            {
                return false;
            }

            if (this.Count != other.Count)
            {
                return false;
            }
            if (this.RoleType != other.RoleType)
            {
                return false;
            }
            if (this.VMSize != other.VMSize)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     This enum enumerates hadoop node types.
    /// </summary>
    internal enum ClusterNodeType
    {
        /// <summary>
        ///     Namenode/Headnode in the cluster.
        /// </summary>
        HeadNode,

        /// <summary>
        ///     Datanode in the cluster.
        /// </summary>
        DataNode,
    }

    /// <summary>
    ///     Sizes for the nodes.
    ///     TODO: Grab this values from the Azure OneSdk.
    /// </summary>
    internal enum NodeVMSize
    {
        ExtraSmall,
        Small,
        Medium,
        Large,
        ExtraLarge
    }
}
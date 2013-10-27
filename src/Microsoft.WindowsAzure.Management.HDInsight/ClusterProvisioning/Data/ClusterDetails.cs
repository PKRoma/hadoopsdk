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
namespace Microsoft.WindowsAzure.Management.HDInsight
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Object that encapsulates all the properties of a List Request.
    /// </summary>
    public sealed class ClusterDetails
    {
        private string stateString;

        /// <summary>
        /// Gets the Name of the cluster.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the a string value of the state of the cluster.
        /// </summary>
        public string StateString
        {
            get { return this.stateString; }
            internal set
            {
                this.stateString = value;
                this.UpdateState(this.stateString);
            }
        }

        /// <summary>
        /// Gets or sets the version of the HDInsight cluster.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets the version status of the HDInsight cluster.
        /// </summary>
        public VersionStatus VersionStatus { get; internal set; }

        /// <summary>
        /// Gets or sets the version of the HDInsight cluster as a number.
        /// </summary>
        public Version VersionNumber { get; set; }

        /// <summary>
        /// Gets the parsed value of the state of the cluster
        /// Note: For compatibility reassons, any new or null states will revert to "UNKOWN".
        /// but the value will be preserved in State.
        /// </summary>
        public ClusterState State { get; internal set; }

        /// <summary>
        /// Gets a possible error state for the cluster (if exists).
        /// </summary>
        public ClusterErrorStatus Error { get; internal set; }

        /// <summary>
        /// Gets the CreateDate of the cluster.
        /// </summary>
        public DateTime CreatedDate { get; internal set; }

        /// <summary>
        /// Gets the connection URL for the cluster.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "Value coming from the server. Value of parsing vs value of breaking is not worth it")]
        public string ConnectionUrl { get; internal set; }

        /// <summary>
        /// Gets the login username for the cluster.
        /// </summary>
        public string HttpUserName { get; internal set; }

        /// <summary>
        /// Gets the password associated with Http requests to the cluster.
        /// </summary>
        public string HttpPassword { get; internal set; }

        /// <summary>
        /// Gets the Rdp user name associated with the cluster.
        /// </summary>
        public string RdpUserName { get; internal set; }

        /// <summary>
        /// Gets the Datacenter location of the cluster.
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// Gets the count of worker nodes.
        /// </summary>
        public int ClusterSizeInNodes { get; internal set; }

        /// <summary>
        /// Gets the default storage account registered with this cluster.
        /// </summary>
        public WabStorageAccountConfiguration DefaultStorageAccount { get; internal set; }

        /// <summary>
        /// Gets the subscriptionid associated with this cluster.
        /// </summary>
        public Guid SubscriptionId { get; internal set; }

        /// <summary>
        /// Gets the additional storage accounts registered with this cluster.
        /// </summary>
        public IEnumerable<WabStorageAccountConfiguration> AdditionalStorageAccounts { get; internal set; }

        internal ClusterDetails()
        {
        }

        internal ClusterDetails(string dnsName, string state)
        {
            this.Name = dnsName;
            this.StateString = state;
            this.UpdateState(state);
        }

        private void UpdateState(string state)
        {
            ClusterState parsedState;
            this.State = (state == null || !Enum.TryParse(state, true, out parsedState)) ? ClusterState.Unknown : parsedState;
        }

        internal void ChangeState(ClusterState newState)
        {
            this.StateString = newState.ToString();
            this.State = newState;
        }
    }
}

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
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Type to represent the HDInsight capabilities of a subscription.
    /// </summary>
    public class AzureHDInsightCapabilities
    {
        /// <summary>
        ///     Initializes a new instance of the AzureHDInsightCapabilities class.
        /// </summary>
        /// <param name="capabilities">Capabilities for this subscription.</param>
        public AzureHDInsightCapabilities(IEnumerable<KeyValuePair<string, string>> capabilities)
        {
            this.Capabilities = capabilities;
        }

        /// <summary>
        ///     Gets the number of HDInsight clusters created with the subscription.
        /// </summary>
        public int ClusterCount { get; internal set; }

        /// <summary>
        ///     Gets the number of CPU cores available for the subscription.
        /// </summary>
        public int CoresAvailable
        {
            get { return Math.Abs(this.MaxCoresAllowed - this.CoresUsed); }
        }

        /// <summary>
        ///     Gets the number of CPU cores used by the subscription.
        /// </summary>
        public int CoresUsed { get; internal set; }

        /// <summary>
        ///     Gets the Azure Locations allowed for the subscription.
        /// </summary>
        public IEnumerable<string> Locations { get; internal set; }

        /// <summary>
        ///     Gets the maximum number of CPU cores that can be used by the subscription.
        /// </summary>
        public int MaxCoresAllowed { get; internal set; }

        /// <summary>
        ///     Gets the HDInsight Versions allowed for the subscription.
        /// </summary>
        public IEnumerable<HDInsightVersion> Versions { get; internal set; }

        /// <summary>
        ///     Gets the HDInsight Capabilities allowed for the subscription.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, string>> Capabilities { get; private set; }
    }
}

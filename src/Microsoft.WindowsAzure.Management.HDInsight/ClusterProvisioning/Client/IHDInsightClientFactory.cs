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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Client
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Factory for HDInsight.Client.
    /// </summary>
    public interface IHDInsightClientFactory
    {
        /// <summary>
        /// Creates an instance of the HDInsight.Client class.
        /// </summary>
        /// <param name="subscriptionId">Subscription to connect to.</param>
        /// <param name="certificate">Client certificate that has been enabled in the subscription.</param>
        /// <returns>Client onbject that can be used to interact with HDInsight clusters.</returns>
        IHDInsightClient Create(Guid subscriptionId, X509Certificate2 certificate);

        // TODO:
        ///// <summary>
        ///// Creates an instance of the HDInsight.Client class.
        ///// </summary>
        ///// <param name="subscriptionId">Subscription to connect to.</param>
        ///// <param name="certificate">Client certificate that has been enabled in the subscription.</param>
        ///// <param name="endpoint">Azure Endpoint.</param>
        ///// <returns>Client onbject that can be used to interact with HDInsight clusters.</returns>
        //IHDInsightClient Create(Guid subscriptionId, X509Certificate2 certificate, Uri endpoint);
    }
}

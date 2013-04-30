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

namespace Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    ///     Class that provides the credentials to talk to an RDFE.
    /// </summary>
    public class ConnectionCredentials : IConnectionCredentials
    {
        /// <summary>
        ///     Initializes a new instance of the ConnectionCredentials class.
        /// </summary>
        /// <param name="endPoint">Azure Enpoint for RDFE.</param>
        /// <param name="deploymentNamespace">Namespace for the HDInsight service.</param>
        /// <param name="subscriptionId">Subscription Id to be used.</param>
        /// <param name="certificate">Certificate to be used to connect authenticate the user with the subscription in azure.</param>
        public ConnectionCredentials(Uri endPoint,
                                     string deploymentNamespace,
                                     Guid subscriptionId, 
                                     X509Certificate2 certificate)
        {
            this.Endpoint = endPoint;
            this.DeploymentNamespace = deploymentNamespace;
            this.SubscriptionId = subscriptionId;
            this.Certificate = certificate;
        }

        /// <summary>
        ///     Gets the Azure Enpoint for RDFE.
        /// </summary>
        public Uri Endpoint { get; private set; }

        /// <summary>
        ///     Gets the Namespace for the HDInsight service.
        /// </summary>
        public string DeploymentNamespace { get; private set; }

        /// <summary>
        ///     Gets the Subscription Id to be used.
        /// </summary>
        public Guid SubscriptionId { get; private set; }

        /// <summary>
        ///     Gets the Certificate to be used to connect authenticate the user with the subscription in azure.
        /// </summary>
        public X509Certificate2 Certificate { get; private set; }
    }
}
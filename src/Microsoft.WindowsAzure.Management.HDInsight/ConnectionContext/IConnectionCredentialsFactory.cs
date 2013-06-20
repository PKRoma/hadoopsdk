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

    // TODO: REPLACE THIS CLASS WITH THE AZURE ONE

    /// <summary>
    /// Connection logic.
    /// </summary>
    public interface IConnectionCredentialsFactory
    {
        /// <summary>
        /// Creates the connection credentials object that provides connection credentials.
        /// </summary>
        /// <param name="subscriptionId">
        /// The subscription.
        /// </param>
        /// <param name="certificate">
        /// The certificate.
        /// </param>
        /// <returns>
        /// The connection credentials object.
        /// </returns>
        IConnectionCredentials Create(Guid subscriptionId, X509Certificate2 certificate);
    }
}
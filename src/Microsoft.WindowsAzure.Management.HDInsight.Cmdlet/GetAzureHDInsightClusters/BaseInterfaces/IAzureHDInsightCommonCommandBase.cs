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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    internal interface IAzureHDInsightCommonCommandBase
    {
        /// <summary>
        ///     Gets or sets the certificate File to be used.
        /// </summary>
        X509Certificate2 Certificate { get; set; }

        /// <summary>
        ///     Gets or sets the cloud service name to use (if provided).
        /// </summary>
        string CloudServiceName { get; set; }

        /// <summary>
        ///     Gets or sets the EndPoint URI to use (if provided).
        /// </summary>
        Uri EndPoint { get; set; }

        /// <summary>
        ///     Gets or sets a logger to write log messages to.
        /// </summary>
        ILogWriter Logger { get; set; }

        /// <summary>
        ///     Gets or sets the Azure Subscription to be used.
        /// </summary>
        string Subscription { get; set; }
    }
}

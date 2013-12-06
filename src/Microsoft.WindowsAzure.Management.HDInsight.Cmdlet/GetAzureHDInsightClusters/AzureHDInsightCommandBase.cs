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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    internal abstract class AzureHDInsightCommandBase : DisposableObject, IAzureHDInsightCommandBase
    {
        public virtual CancellationToken CancellationToken
        {
            get { return CancellationToken.None; }
        }

        public X509Certificate2 Certificate { get; set; }

        public string CloudServiceName { get; set; }

        public Uri EndPoint { get; set; }

        public ILogWriter Logger { get; set; }

        public string Subscription { get; set; }

        public virtual void Cancel()
        {
        }

        public abstract Task EndProcessing();
    }
}

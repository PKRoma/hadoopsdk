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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Management.Automation;
    using System.Threading;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;

    internal abstract class AzureHDInsightJobCommandExecutorBase : AzureHDInsightCommandBase, IAzureHDInsightJobCommandCredentialsBase
    {
        protected CancellationTokenSource tokenSource = new CancellationTokenSource();

        public override CancellationToken CancellationToken
        {
            get { return this.tokenSource.Token; }
        }

        public PSCredential Credential { get; set; }

        public override void Cancel()
        {
            this.tokenSource.Cancel();
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly",
            Justification = "Url resolution is done when the EndProcessing method is called.")]
        internal IJobSubmissionClient GetClient(string cluster)
        {
            IJobSubmissionClient client = null;
            IJobSubmissionClientCredential clientCredential = null;
            cluster.ArgumentNotNull("ClusterEndpoint");
            if (this.Credential != null)
            {
                clientCredential = new BasicAuthCredential
                {
                    Server = GatewayUriResolver.GetGatewayUri(cluster),
                    UserName = this.Credential.UserName,
                    Password = this.Credential.GetCleartextPassword()
                };
            }

            if (this.Subscription != null)
            {
                clientCredential = new JobSubmissionCertificateCredential(this.GetSubscriptionCertificateCredentials(), cluster);
            }

            if (clientCredential != null)
            {
                client = ServiceLocator.Instance.Locate<IAzureHDInsightJobSubmissionClientFactory>().Create(clientCredential);
            }

            if (client.IsNotNull())
            {
                client.SetCancellationSource(this.tokenSource);
                if (this.Logger.IsNotNull())
                {
                    client.AddLogWriter(this.Logger);
                }

                return client;
            }

            throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, "Expected either a Subscription or Credential parameter."));
        }
    }
}

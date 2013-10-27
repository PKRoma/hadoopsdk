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
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;

    internal static class AzureHDInsightCommandExtensions
    {
        internal static Guid ResolveSubscriptionId(string subscription)
        {
            Guid subscriptionId;
            if (!Guid.TryParse(subscription, out subscriptionId))
            {
                SubscriptionData subscriptionData;
                var subscriptionsManager = ServiceLocator.Instance.Locate<IAzureHDInsightSubscriptionsManagerFactory>().Create();
                if (!subscriptionsManager.TryGetSubscriptionData(subscription, out subscriptionData))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unable to resolve subscription '{0}'", subscription));
                }

                subscriptionId = subscriptionData.SubscriptionId;
            }

            return subscriptionId;
        }

        internal static X509Certificate2 ResolveCertificate(Guid subscriptionId, X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                SubscriptionData subscriptionData;
                var subscriptionsManager = ServiceLocator.Instance.Locate<IAzureHDInsightSubscriptionsManagerFactory>().Create();
                if (!subscriptionsManager.TryGetSubscriptionData(subscriptionId, out subscriptionData))
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture, "Unable to find certificate for subscription '{0}'", subscriptionId));
                }

                certificate = subscriptionData.Certificate;
            }

            return certificate;
        }

        internal static void AssignCredentialsToCommand(this IAzureHDInsightCommonCommandBase command, IHDInsightCertificateCredential creds)
        {
            command.Certificate = creds.Certificate;
            command.Subscription = creds.SubscriptionId.ToString();
            command.EndPoint = creds.Endpoint;
        }

        public static HDInsightCertificateCredential GetSubscriptionCertificateCredentials(this IAzureHDInsightCommonCommandBase command)
        {
            var subscriptionId = ResolveSubscriptionId(command.Subscription);
            var certificate = ResolveCertificate(subscriptionId, command.Certificate);

            if (command.CloudServiceName.IsNotNullOrEmpty() && command.EndPoint.IsNotNull())
            {
                return new HDInsightCertificateCredential()
                {
                    SubscriptionId = subscriptionId,
                    Certificate = certificate,
                    Endpoint = command.EndPoint,
                    DeploymentNamespace = command.CloudServiceName
                };
            }

            if (command.EndPoint.IsNotNull())
            {
                return new HDInsightCertificateCredential()
                {
                    SubscriptionId = subscriptionId,
                    Certificate = certificate,
                    Endpoint = command.EndPoint,
                };
            }

            return new HDInsightCertificateCredential() { SubscriptionId = subscriptionId, Certificate = certificate };
        }
    }
}

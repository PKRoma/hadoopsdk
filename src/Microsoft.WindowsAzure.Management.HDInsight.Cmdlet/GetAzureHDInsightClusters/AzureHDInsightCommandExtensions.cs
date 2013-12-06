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
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;

    internal static class AzureHDInsightCommandExtensions
    {
        public static HDInsightCertificateCredential GetSubscriptionCertificateCredentials(this IAzureHDInsightCommonCommandBase command)
        {
            Guid subscriptionId = ResolveSubscriptionId(command.Subscription);
            X509Certificate2 certificate = ResolveCertificate(subscriptionId, command.Certificate);

            if (command.CloudServiceName.IsNotNullOrEmpty() && command.EndPoint.IsNotNull())
            {
                return new HDInsightCertificateCredential
                {
                    SubscriptionId = subscriptionId,
                    Certificate = certificate,
                    Endpoint = command.EndPoint,
                    DeploymentNamespace = command.CloudServiceName
                };
            }

            if (command.EndPoint.IsNotNull())
            {
                return new HDInsightCertificateCredential { SubscriptionId = subscriptionId, Certificate = certificate, Endpoint = command.EndPoint, };
            }

            return new HDInsightCertificateCredential { SubscriptionId = subscriptionId, Certificate = certificate };
        }

        internal static void AssignCredentialsToCommand(this IAzureHDInsightCommonCommandBase command, IHDInsightCertificateCredential creds)
        {
            command.Certificate = creds.Certificate;
            command.Subscription = creds.SubscriptionId.ToString();
            command.EndPoint = creds.Endpoint;
        }

        internal static X509Certificate2 ResolveCertificate(Guid subscriptionId, X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                SubscriptionData subscriptionData;
                var subscriptionsManager = ServiceLocator.Instance.Locate<IAzureHDInsightSubscriptionsManagerFactory>().Create();
                if (!(subscriptionsManager.TryGetSubscriptionData(subscriptionId, out subscriptionData) && subscriptionData.Certificate.IsNotNull()))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to retrieve Certificate for the subscription '{0}'.\r\n" +
                            "Please register subcription certificate as described here: http://go.microsoft.com/fwlink/?LinkID=325564&clcid=0x409",
                            subscriptionId));
                }

                certificate = subscriptionData.Certificate;
            }

            return certificate;
        }

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
    }
}

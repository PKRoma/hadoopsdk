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
    using System.Linq;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;

    internal class AzureHDInsightSubscriptionsManager : IAzureHDInsightSubscriptionsManager
    {
        public bool TryGetSubscriptionData(Guid subscriptionId, out SubscriptionData subscriptionData)
        {
            subscriptionData = null;
            using (var subscriptionsFileManager = ServiceLocator.Instance.Locate<IAzureHDInsightSubscriptionsFileManagerFactory>().Create())
            {
                subscriptionData = subscriptionsFileManager.GetSubscriptions().FirstOrDefault(sub => sub.SubscriptionId == subscriptionId);
            }

            return subscriptionData.IsNotNull();
        }

        public bool TryGetSubscriptionData(string subscriptionName, out SubscriptionData subscriptionData)
        {
            subscriptionData = null;
            using (var subscriptionsFileManager = ServiceLocator.Instance.Locate<IAzureHDInsightSubscriptionsFileManagerFactory>().Create())
            {
                subscriptionData = subscriptionsFileManager.GetSubscriptions().FirstOrDefault(sub => sub.SubscriptionName == subscriptionName);
            }

            return subscriptionData.IsNotNull();
        }
    }
}

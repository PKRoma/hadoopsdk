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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdletAbstractionTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class ManageAzureHDInsightHttpAccessCommandTests : IntegrationTestBase
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Rdfe")]
        public void CanGrantAccessToHttpServices()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            AzureHDInsightCluster testCluster = GetClusterWithHttpAccessDisabled(creds);
            AzureHDInsightCluster cluster = EnableHttpAccessToCluster(
                creds, testCluster, TestCredentials.AzureUserName, TestCredentials.AzurePassword);
            Assert.IsNotNull(cluster);
            Assert.AreEqual(cluster.HttpUserName, TestCredentials.AzureUserName);
            Assert.AreEqual(cluster.HttpPassword, TestCredentials.AzurePassword);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("Rdfe")]
        public void CanRevokeAccessToHttpServices()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            AzureHDInsightCluster testCluster = GetClusterWithHttpAccessDisabled(creds);
            EnableHttpAccessToCluster(creds, testCluster, TestCredentials.AzureUserName, TestCredentials.AzurePassword);
            AzureHDInsightCluster cluster = DisableHttpAccessToCluster(creds, testCluster);
            Assert.IsNotNull(cluster);
            Assert.IsTrue(string.IsNullOrEmpty(cluster.HttpUserName));
            Assert.IsTrue(string.IsNullOrEmpty(cluster.HttpPassword));
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        private static AzureHDInsightCluster GetClusterWithHttpAccessDisabled(IHDInsightCertificateCredential creds)
        {
            IGetAzureHDInsightClusterCommand client = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
            client.Subscription = creds.SubscriptionId.ToString();
            client.Certificate = creds.Certificate;
            client.EndProcessing();
            List<AzureHDInsightCluster> clusters = client.Output.ToList();
            AzureHDInsightCluster containerWithHttpAccessDisabled = clusters.FirstOrDefault(cluster => cluster.HttpUserName.IsNullOrEmpty());
            if (containerWithHttpAccessDisabled == null)
            {
                containerWithHttpAccessDisabled = clusters.Last();
                DisableHttpAccessToCluster(creds, containerWithHttpAccessDisabled);
            }

            return containerWithHttpAccessDisabled;
        }

        private static AzureHDInsightCluster DisableHttpAccessToCluster(
            IHDInsightCertificateCredential creds, AzureHDInsightCluster containerWithHttpAccessDisabled)
        {
            IManageAzureHDInsightHttpAccessCommand httpManagementClient =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateManageHttpAccess();
            httpManagementClient.Subscription = creds.SubscriptionId.ToString();
            httpManagementClient.Certificate = creds.Certificate;
            httpManagementClient.Credential = GetAzurePsCredentials();
            httpManagementClient.Name = containerWithHttpAccessDisabled.Name;
            httpManagementClient.Location = containerWithHttpAccessDisabled.Location;
            httpManagementClient.Enable = false;
            httpManagementClient.EndProcessing();
            return httpManagementClient.Output.First();
        }

        private static AzureHDInsightCluster EnableHttpAccessToCluster(
            IHDInsightCertificateCredential creds, AzureHDInsightCluster containerWithHttpAccessDisabled, string httpUserName, string httpPassword)
        {
            IManageAzureHDInsightHttpAccessCommand httpManagementClient =
                ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateManageHttpAccess();
            httpManagementClient.Subscription = creds.SubscriptionId.ToString();
            httpManagementClient.Certificate = creds.Certificate;
            httpManagementClient.Credential = GetPSCredential(httpUserName, httpPassword);
            httpManagementClient.Name = containerWithHttpAccessDisabled.Name;
            httpManagementClient.Location = containerWithHttpAccessDisabled.Location;
            httpManagementClient.Enable = true;
            httpManagementClient.EndProcessing();
            return httpManagementClient.Output.First();
        }
    }
}

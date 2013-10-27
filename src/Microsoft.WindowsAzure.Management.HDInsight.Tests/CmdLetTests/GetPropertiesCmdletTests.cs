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
namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using Microsoft.Hadoop.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.VersionFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdletAbstractionTests;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;

    [TestClass]
    public class GetPropertiesCmdletTests : IntegrationTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        [TestCategory("Rdfe")]
        public void CanCallTheGetHDInsightPropertiesCmdlet()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .Invoke();

                HDInsightGetPropertiesCommandTests.ValidateCapabilities(results.Results.ToEnumerable<AzureHDInsightCapabilities>());
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        [TestCategory("Rdfe")]
        public void CanCallTheGetHDInsightPropertiesCmdletWithVersionsSwitch()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Versions, null)
                                      .Invoke();

                var versionsFromPowerShell = results.Results.ToEnumerable<IEnumerable<HDInsightVersion>>().SelectMany(ver => ver.ToList()).ToList();

                var versions = VersionFinderClient.ParseVersions(IntegrationTestBase.TestCredentials.ResourceProviderProperties.ToDictionary(prop => prop.Key, prop2 => prop2.Value));
                foreach (var version in versions)
                {
                    Assert.IsTrue(versionsFromPowerShell.Any(capVersion => string.Equals(version.Version, capVersion.Version, StringComparison.Ordinal)), "unable to find version '{0}' in capabilities", version.Version);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        [TestCategory("Rdfe")]
        public void CanCallTheGetHDInsightPropertiesCmdletWithDebugSwitch()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Debug, null)
                                      .Invoke();

                var capabilities = results.Results.ToEnumerable<IEnumerable<KeyValuePair<string, string>>>().SelectMany(ver => ver.ToList()).ToList();
                Assert.IsNotNull(capabilities);
                Assert.IsTrue(capabilities.Count > 0);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        [TestCategory("Rdfe")]
        public void CanCallTheGetHDInsightPropertiesCmdletWithLocationsSwitch()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                                      .WithParameter(CmdletConstants.Locations, null)
                                      .Invoke();

                var locationsFromPowerShell = results.Results.ToEnumerable<IEnumerable<string>>().SelectMany(ver => ver.ToList()).ToList();

                var locations = LocationFinderClient.ParseLocations(IntegrationTestBase.TestCredentials.ResourceProviderProperties.ToDictionary(prop => prop.Key, prop2 => prop2.Value));
                foreach (var Location in locations)
                {
                    Assert.IsTrue(locationsFromPowerShell.Any(capLocation => string.Equals(Location, capLocation, StringComparison.Ordinal)), "unable to find Location '{0}' in capabilities", Location);
                }
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory("PowerShell")]
        [TestCategory("Scenario")]
        [TestCategory("Rdfe")]
        public void CanCallTheGetHDInsightPropertiesCmdletWithoutCertificate()
        {
            var creds = GetValidCredentials();
            using (var runspace = this.GetPowerShellRunspace())
            {
                var results = runspace.NewPipeline()
                                      .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                                      .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                                      .Invoke();

                HDInsightGetPropertiesCommandTests.ValidateCapabilities(results.Results.ToEnumerable<AzureHDInsightCapabilities>());
            }
        }
    }
}

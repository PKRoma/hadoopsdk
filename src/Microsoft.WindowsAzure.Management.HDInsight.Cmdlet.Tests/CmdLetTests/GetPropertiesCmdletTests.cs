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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdLetTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.CmdletAbstractionTests;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.PowerShellTestAbstraction.Interfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities;

    [TestClass]
    public class GetPropertiesCmdletTests : IntegrationTestBase
    {
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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
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
        public void CanCallTheGetHDInsightPropertiesCmdletWithDebugSwitch()
        {
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Debug, null)
                            .Invoke();

                List<KeyValuePair<string, string>> capabilities =
                    results.Results.ToEnumerable<IEnumerable<KeyValuePair<string, string>>>().SelectMany(ver => ver.ToList()).ToList();
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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Locations, null)
                            .Invoke();

                List<string> locationsFromPowerShell = results.Results.ToEnumerable<IEnumerable<string>>().SelectMany(ver => ver.ToList()).ToList();

                var locations = new Collection<string> { "East US", "East US 2", "West US", "North Europe" };
                foreach (string Location in locations)
                {
                    Assert.IsTrue(
                        locationsFromPowerShell.Any(capLocation => string.Equals(Location, capLocation, StringComparison.Ordinal)),
                        "unable to find Location '{0}' in capabilities",
                        Location);
                }
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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .WithParameter(CmdletConstants.Certificate, creds.Certificate)
                            .WithParameter(CmdletConstants.Versions, null)
                            .Invoke();

                List<HDInsightVersion> versionsFromPowerShell =
                    results.Results.ToEnumerable<IEnumerable<HDInsightVersion>>().SelectMany(ver => ver.ToList()).ToList();
                var versions = new Collection<HDInsightVersion>();
                versions.Add(new HDInsightVersion { Version = "1.2", VersionStatus = VersionStatus.Obsolete });
                versions.Add(new HDInsightVersion { Version = "1.5", VersionStatus = VersionStatus.Obsolete });
                versions.Add(new HDInsightVersion { Version = "1.6", VersionStatus = VersionStatus.Compatible });
                versions.Add(new HDInsightVersion { Version = "2.1", VersionStatus = VersionStatus.Compatible });
                foreach (HDInsightVersion version in versions)
                {
                    Assert.IsTrue(
                        versionsFromPowerShell.Any(capVersion => string.Equals(version.Version, capVersion.Version, StringComparison.Ordinal)),
                        "unable to find version '{0}' in capabilities",
                        version.Version);
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
            IHDInsightCertificateCredential creds = GetValidCredentials();
            using (IRunspace runspace = this.GetPowerShellRunspace())
            {
                IPipelineResult results =
                    runspace.NewPipeline()
                            .AddCommand(CmdletConstants.GetAzureHDInsightProperties)
                            .WithParameter(CmdletConstants.Subscription, creds.SubscriptionId.ToString())
                            .Invoke();

                HDInsightGetPropertiesCommandTests.ValidateCapabilities(results.Results.ToEnumerable<AzureHDInsightCapabilities>());
            }
        }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}

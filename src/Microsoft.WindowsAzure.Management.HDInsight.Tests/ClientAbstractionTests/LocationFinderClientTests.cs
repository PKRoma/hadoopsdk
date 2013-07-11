namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.XPath;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.AzureManagementClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.LocationFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    [TestClass]
    public class LocationFinderClientTests : IntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.ApplyFullMocking();
            this.ResetIndividualMocks();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("LocationFinderClient")]
        public void ICanPerformA_PositiveCase_LocationFinderXmlParsing()
        {
            string xml = @"<ResourceProviderProperties xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><ResourceProviderProperty><Key>CAPABILITY_REGION_EAST_US</Key><Value>East US</Value></ResourceProviderProperty><ResourceProviderProperty><Key>CAPABILITY_REGION_EAST_US_2</Key><Value>East US 2</Value></ResourceProviderProperty><ResourceProviderProperty><Key>CAPABILITY_REGION_NORTH_EUROPE</Key><Value>North Europe</Value></ResourceProviderProperty><ResourceProviderProperty><Key>CAPABILITY_VERSION_1.2.0.0.LARGESKU-AMD64-134231</Key><Value>1.2.0.0.LargeSKU-amd64-134231</Value></ResourceProviderProperty><ResourceProviderProperty><Key>CONTAINERS_Count</Key><Value>13</Value></ResourceProviderProperty><ResourceProviderProperty><Key>CONTAINERS_CoresUsed</Key><Value>168</Value></ResourceProviderProperty><ResourceProviderProperty><Key>CONTAINERS_MaxCoresAllowed</Key><Value>170</Value></ResourceProviderProperty></ResourceProviderProperties>";
            var locations = LocationFinderClient.ParseLocations(xml);
            Assert.AreEqual(3, locations.Count);
            Assert.AreEqual(1, locations.Count(location => location == "East US"));
            Assert.AreEqual(1, locations.Count(location => location == "East US 2"));
            Assert.AreEqual(1, locations.Count(location => location == "North Europe"));
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("LocationFinderClient")]
        public void ICanPerformA_EmptyXmlParsing_LocationFinderXmlParsing()
        {
            string xml = @"<root/>";
            var locations = LocationFinderClient.ParseLocations(xml);
            Assert.AreEqual(0, locations.Count);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("LocationFinderClient")]
        public void ICanPerformA_PositiveAdditionalProppertiesXmlParsing_LocationFinderXmlParsing()
        {
            string xml = @"<ResourceProviderProperties xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><P></P><ResourceProviderProperty><Key>CAPABILITY_REGION_EAST_US</Key><Value>East US</Value></ResourceProviderProperty></ResourceProviderProperties>";
            var locations = LocationFinderClient.ParseLocations(xml);
            Assert.AreEqual(1, locations.Count);
            Assert.AreEqual("East US", locations[0]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("LocationFinderClient")]
        public void ICanPerformA_InvalidProppertiesXmlParsing_LocationFinderXmlParsing()
        {
            string xml = @"<ResourceProviderProperties xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><ResourceProviderProperty><KeyA>CAPABILITY_REGION_EAST_US</KeyA><Value>East US</Value></ResourceProviderProperty></ResourceProviderProperties>";
            var locations = LocationFinderClient.ParseLocations(xml);
            Assert.AreEqual(0, locations.Count);

            xml = @"<ResourceProviderProperties xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""><ResourceProviderProperty><Key>CAPABILITY_REGION_EAST_US</Key><ValueB>East US</ValueB></ResourceProviderProperty></ResourceProviderProperties>";
            locations = LocationFinderClient.ParseLocations(xml);
            Assert.AreEqual(0, locations.Count);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("LocationFinderClient")]
        [TestCategory("Scenario")]
        public async Task ICanPerformA_PositiveSubscriptionValidation_Using_LocationFinderAbstraction() // Always goes against azure to quickly validate end2end
        {
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();

            // Makes sure we get region locations even if we unregister the location
            DeleteClusters(credentials, "North Europe");
            var registrationClient = new SubscriptionRegistrationClient(credentials);
            if (await registrationClient.ValidateSubscriptionLocation("North Europe"))
            {
                await registrationClient.UnregisterSubscriptionLocation("North Europe");
            }

            // Validate locations
            var client = new LocationFinderClient(credentials);
            var locations = await client.ListAvailableLocations();
            Assert.AreEqual(1, locations.Count(location => location == "East US"));
            Assert.AreEqual(1, locations.Count(location => location == "East US 2"));
            Assert.AreEqual(1, locations.Count(location => location == "North Europe"));
        }
    }
}
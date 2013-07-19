namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.Framework.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;
    using Moq;

    [TestClass]
    public class AsvClientTests : IntegrationTestBase
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
        [TestCategory("RestAsvClient")]
        public async Task ICanPerformA_PositiveValidateAccount_Using_RestAsvClientAbstraction()
        {
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();

            await client.ValidateAccount(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name,
                                         IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key);
            await client.ValidateAccount(IntegrationTestBase.TestCredentials.Environments[0].AdditionalStorageAccounts[0].Name,
                                         IntegrationTestBase.TestCredentials.Environments[0].AdditionalStorageAccounts[0].Key);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("RestAsvClient")]
        public async Task ICanPerformA_PositiveValidateContainer_Using_RestAsvClientAbstraction()
        {
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();

            await client.ValidateContainer(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name,
                                           IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key,
                                           IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Container);
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Defect")]
        public void WhenIPerformTwoRequestsForValidateAccountViaTheAsvClient_TwoDifferentTimesAreUsed()
        {
            // Locate the test manager so we can override the lower layer.
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationIndividualTestManager>();

            // Create a Mock of the HTTP layer response.
            var moqResponse = new Moq.Mock<IHttpResponseMessageAbstraction>(MockBehavior.Loose);
            // Always return 200 OK
            moqResponse.SetupGet(res => res.StatusCode)
                       .Returns(HttpStatusCode.OK);

            // Create a mock of the Request client.
            var moqClient = new Moq.Mock<IHttpClientAbstraction>(MockBehavior.Loose);

            // A dictionary to hold the request headers supplied.
            var requestHeaders = new Dictionary<string, string>();

            // Mock the return to set the request headers.
            moqClient.SetupGet(client => client.RequestHeaders)
                     .Returns(() => requestHeaders);

            // Mock the SendAsync method (to just return the response object previously created).
            moqClient.Setup(c => c.SendAsync())
                     .Returns(() => Task.Run(() => moqResponse.Object));

            // Mock the factory to return our mock client.
            var moqFactory = new Moq.Mock<IHttpClientAbstractionFactory>();

            // Overload both create methods.
            moqFactory.Setup(fac => fac.Create(It.IsAny<X509Certificate2>()))
                      .Returns(() => moqClient.Object);
            moqFactory.Setup(fac => fac.Create())
                      .Returns(() => moqClient.Object);

            // Override the factory in the Service Locator (for this test only).
            manager.Override<IHttpClientAbstractionFactory>(moqFactory.Object);

            // Get the Asv Client factory.
            var factory = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>();

            // Create a client.
            var restClient = factory.Create();

            // Call Validate container.
            restClient.ValidateAccount(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name, IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key).WaitForResult();

            // Wait for 2 seconds to ensure the dates are different.
            Thread.Sleep(2000);

            // Get the "x-ms-date" header.
            var firstCallDate = requestHeaders[HDInsightRestHardcodes.XMsDate];

            // Clear the dictionary (to prepare for the second call).
            requestHeaders.Clear();

            // Call Validate container a second time.
            restClient.ValidateAccount(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name, IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key).WaitForResult();

            // Get the "x-ms-date" header.
            var secondCallDate = requestHeaders[HDInsightRestHardcodes.XMsDate];

            // The two dates should not be the same.
            Assert.AreNotEqual(firstCallDate, secondCallDate);
        }

        [TestMethod]
        [TestCategory(TestRunMode.CheckIn)]
        [TestCategory("Defect")]
        public void WhenIPerformTwoRequestsForValidateContainerViaTheAsvClient_TwoDifferentTimesAreUsed()
        {
            // Locate the test manager so we can override the lower layer.
            var manager = ServiceLocator.Instance.Locate<IIocServiceLocationIndividualTestManager>();

            // Create a Mock of the HTTP layer response.
            var moqResponse = new Moq.Mock<IHttpResponseMessageAbstraction>(MockBehavior.Loose);
            // Always return 200 OK
            moqResponse.SetupGet(res => res.StatusCode)
                       .Returns(HttpStatusCode.OK);

            // Create a mock of the Request client.
            var moqClient = new Moq.Mock<IHttpClientAbstraction>(MockBehavior.Loose);

            // A dictionary to hold the request headers supplied.
            var requestHeaders = new Dictionary<string, string>();

            // Mock the return to set the request headers.
            moqClient.SetupGet(client => client.RequestHeaders)
                     .Returns(() => requestHeaders);

            // Mock the SendAsync method (to just return the response object previously created).
            moqClient.Setup(c => c.SendAsync())
                     .Returns(() => Task.Run(() => moqResponse.Object));

            // Mock the factory to return our mock client.
            var moqFactory = new Moq.Mock<IHttpClientAbstractionFactory>();

            // Overload both create methods.
            moqFactory.Setup(fac => fac.Create(It.IsAny<X509Certificate2>()))
                      .Returns(() => moqClient.Object);
            moqFactory.Setup(fac => fac.Create())
                      .Returns(() => moqClient.Object);

            // Override the factory in the Service Locator (for this test only).
            manager.Override<IHttpClientAbstractionFactory>(moqFactory.Object);

            // Get the Asv Client factory.
            var factory = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>();

            // Create a client.
            var restClient = factory.Create();

            // Call Validate container.
            restClient.ValidateContainer(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name, IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key, IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Container).WaitForResult();

            // Wait for 2 seconds to ensure the dates are different.
            Thread.Sleep(2000);

            // Get the "x-ms-date" header.
            var firstCallDate = requestHeaders[HDInsightRestHardcodes.XMsDate];

            // Clear the dictionary (to prepare for the second call).
            requestHeaders.Clear();

            // Call Validate container a second time.
            restClient.ValidateContainer(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name, IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key, IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Container).WaitForResult();

            // Get the "x-ms-date" header.
            var secondCallDate = requestHeaders[HDInsightRestHardcodes.XMsDate];

            // The two dates should not be the same.
            Assert.AreNotEqual(firstCallDate, secondCallDate);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory(TestRunMode.Nightly)] // Commenting since this takes 15 sec
        [TestCategory("RestAsvClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task ICanPerformA_NegativeValidateAccount_PartialAccount_RestAsvClientAbstraction()
        {
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();
            await client.ValidateAccount(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name.Split(new char[] { '.' })[0],
                                         IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key);

        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory(TestRunMode.Nightly)] // Commenting since this takes 15 sec
        [TestCategory("RestAsvClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task ICanPerformA_NegativeValidateAccount_WrongAccount_RestAsvClientAbstraction()
        {
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();
            await client.ValidateAccount("invalid." + IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name,
                                         IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("RestAsvClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task ICanPerformA_NegativeValidateAccount_InvalidKey_RestAsvClientAbstraction()
        {
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();
            await client.ValidateAccount(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name, "key");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("RestAsvClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task ICanPerformA_NegativeValidateAccount_WrongKey_RestAsvClientAbstraction()
        {
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();
            await client.ValidateAccount(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name,
                                         IntegrationTestBase.TestCredentials.Environments[0].AdditionalStorageAccounts[0].Key);

        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("CheckIn")]
        [TestCategory("RestAsvClient")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public async Task ICanPerformA_NegativeValidateContainer_NonExistingContainer_RestAsvClientAbstraction()
        {
            var client = ServiceLocator.Instance.Locate<IAsvValidatorClientFactory>().Create();
            await client.ValidateContainer(IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Name,
                                           IntegrationTestBase.TestCredentials.Environments[0].DefaultStorageAccount.Key,
                                           Guid.NewGuid().ToString("N").ToLowerInvariant());

        }
    }
}

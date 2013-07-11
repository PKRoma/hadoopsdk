namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ClientAbstractionTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.Framework.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    [TestClass]
    public class HttpClientTests : IntegrationTestBase
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
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory(TestRunMode.Nightly)]
        public async Task ICanPerformA_Get_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //       And I set the uri for the request object
                client.RequestUri = new Uri("http://httpbin.org/get");
                //       And I set the method to Get
                client.Method = HttpMethod.Get;
                //      When I call Client.SendAsync
                var responseTask = client.SendAsync();
                var response = await responseTask;
                //  Then I should receive a response
                Assert.IsNotNull(response);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
                //   And the response should have been successful.
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                //   And the content should have a non zero length
                Assert.AreNotEqual(0, response.Content.Length);
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("Integration")]
        [TestCategory("HttpClient")]
        public async Task ICanPerformA_Put_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //       And I set the uri for the request object
                client.RequestUri = new Uri("http://httpbin.org/put");
                //       And I set the method to Post
                client.Method = HttpMethod.Put;
                //       And I set the content to "Hello World"
                client.Content = "Hello World";

                //      When I call Client.SendAsync
                var responseTask = client.SendAsync();
                var response = await responseTask;
                //  Then I should receive a response
                Assert.IsNotNull(response);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
                //   And the response should have been successful.
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                //   And the content should have a non zero length
                Assert.AreNotEqual(0, response.Content.Length);
                //   And the content should contain "Hello World"
                Assert.IsTrue(response.Content.Contains("\"data\": \"Hello World\""));
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("Integration")]
        [TestCategory("HttpClient")]
        public async Task ICanPerformA_Post_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //       And I set the uri for the request object
                client.RequestUri = new Uri("http://httpbin.org/post");
                //       And I set the method to Post
                client.Method = HttpMethod.Post;
                //       And I set the content to "Hello World"
                client.Content = "Hello World";

                //      When I call Client.SendAsync
                var responseTask = client.SendAsync();
                var response = await responseTask;
                //  Then I should receive a response
                Assert.IsNotNull(response);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
                //   And the response should have been successful.
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                //   And the content should have a non zero length
                Assert.AreNotEqual(0, response.Content.Length);
                //   And the content should contain "Hello World"
                Assert.IsTrue(response.Content.Contains("\"data\": \"Hello World\""));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        [TestCategory(TestRunMode.Nightly)]
        public async Task ICanPerformA_Delete_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //       And I set the uri for the request object
                client.RequestUri = new Uri("http://httpbin.org/delete");
                //       And I set the method to Post
                client.Method = HttpMethod.Delete;

                //      When I call Client.SendAsync
                var responseTask = client.SendAsync();
                var response = await responseTask;
                //  Then I should receive a response
                Assert.IsNotNull(response);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
                //   And the response should have been successful.
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                //   And the content should have a non zero length
                Assert.AreNotEqual(0, response.Content.Length);
            }
        }

        [TestMethod]
        [TestCategory(TestRunMode.Nightly)]
        [TestCategory("Integration")]
        [TestCategory("HttpClient")]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ICanPerformA_MultipleRequests_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //      Then for create a simple requests
                client.RequestUri = new Uri("http://httpbin.org/get");
                client.Method = HttpMethod.Get;

                //      And then send multiple requests on the same client
                await client.SendAsync();
                await client.SendAsync();
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("HttpClient")]
        [TestCategory(TestRunMode.Nightly)]
        public async Task ICanPerformA_GetWithCert_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //       And I set the uri to the base of Azure Services
                client.RequestUri = new Uri(credentials.Endpoint, new Uri(credentials.SubscriptionId.ToString(), UriKind.Relative));
                client.RequestHeaders.Add("x-ms-version", "2012-08-01");
                client.RequestHeaders.Add("accept", "application/xml");

                //       And I set the method to Get
                client.Method = HttpMethod.Get;

                //      When I call Client.SendAsync
                var responseTask = client.SendAsync();
                var response = await responseTask;
                //  Then I should receive a response
                Assert.IsNotNull(response);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);

                //   And the response should have been successful.
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                //   And the content should have a non zero length
                Assert.AreNotEqual(0, response.Content.Length);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("HttpClient")]
        [TestCategory(TestRunMode.Nightly)]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task ICanHandleA_ClientTimeout_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //       And I set the uri for a http that will take 30 sec to complete
                client.RequestUri = new Uri("http://httpbin.org/delay/30000");
                client.RequestHeaders.Add("x-ms-version", "2012-08-01");
                client.RequestHeaders.Add("accept", "application/xml");

                //       And I set the client timeout to a value << 30 sec
                client.Timeout = TimeSpan.FromMilliseconds(1);

                //      The call Client.SendAsync to trigger exception
                var responseTask = client.SendAsync();
                var response = await responseTask;
                Console.WriteLine(response.Content);
                Console.WriteLine(response.ToString());
                Console.WriteLine(response.StatusCode);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("HttpClient")]
        [TestCategory(TestRunMode.Nightly)]
        public async Task ICanHandleA_ErrorStatus_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {
                //       And I set the uri for a http that will trigger a 404 (NotFound)
                client.RequestUri = new Uri("http://httpbin.org/status/404");
                client.RequestHeaders.Add("x-ms-version", "2012-08-01");
                client.RequestHeaders.Add("accept", "application/xml");

                //      When I call Client.SendAsync
                var responseTask = client.SendAsync();
                var response = await responseTask;
                //  Then I should receive a response
                Assert.IsNotNull(response);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
                //   And the response should have been unsuccessful.
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                //   And the content should be empty
                Assert.AreEqual(string.Empty, response.Content);
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("HttpClient")]
        [TestCategory(TestRunMode.Nightly)]
        public async Task ICanHandleA_AuthenticationError_Using_HttpClientAbstraction()
        {
            //         Given I want to use an X509 Cert for authentication
            IConnectionCredentials credentials = IntegrationTestBase.GetValidCredentials();
            //           And I have an Http Client
            using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create(credentials.Certificate))
            {

                //       And I set the uri to the base of Azure Services
                client.RequestUri = new Uri(credentials.Endpoint, new Uri(Guid.Empty.ToString(), UriKind.Relative));
                client.RequestHeaders.Add("x-ms-version", "2012-08-01");
                client.RequestHeaders.Add("accept", "application/xml");

                //       And I set the method to Get
                client.Method = HttpMethod.Get;

                //      When I call Client.SendAsync
                var responseTask = client.SendAsync();
                var response = await responseTask;
                //  Then I should receive a response
                Assert.IsNotNull(response);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);

                //   And the response should have been successful.
                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
                //   And the content should contain the error
                Assert.AreNotEqual(string.Empty, response.Content);
            }
        }
    }
}

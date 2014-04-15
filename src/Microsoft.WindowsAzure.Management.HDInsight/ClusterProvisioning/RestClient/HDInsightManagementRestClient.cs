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
namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.Hadoop.Client.WebHCatRest;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.ClusterManager;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    internal class HDInsightManagementRestClient : IHDInsightManagementRestClient
    {
        private readonly IHDInsightSubscriptionCredentials credentials;
        private readonly HDInsight.IAbstractionContext context;
        private const string HelpLinkForException = @"http://go.microsoft.com/fwlink/?LinkID=324137";
        private readonly bool ignoreSslErrors;

        public IHDInsightSubscriptionCredentials Credentials
        {
            get { return this.credentials; }
        }

        internal HDInsightManagementRestClient(IHDInsightSubscriptionCredentials credentials, HDInsight.IAbstractionContext context, bool ignoreSslErrors)
        {
            this.context = context;
            this.credentials = credentials;
            this.ignoreSslErrors = ignoreSslErrors;
            if (context.Logger.IsNotNull())
            {
                this.Logger = context.Logger;
            }
            else
            {
                this.Logger = new Logger();
            }
        }

        private async Task<IHttpResponseMessageAbstraction> ProcessListCloudServices(IHttpClientAbstraction client)
        {
            var httpLogic = ServiceLocator.Instance.Locate<IHttpOperationManager>();
            client.Timeout = httpLogic.HttpOperationTimeout;
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.context, this.ignoreSslErrors);
            var uriBuilder = overrideHandlers.UriBuilder;

            client.RequestUri = uriBuilder.GetListCloudServicesUri();
            client.RequestHeaders.Add(HDInsightRestConstants.XMsVersion);
            client.RequestHeaders.Add(HDInsightRestConstants.Accept);
            client.RequestHeaders.Add(HDInsightRestConstants.UserAgent);
            client.Method = HttpMethod.Get;

            // Sends, validates and parses the response
            var httpResponse = await client.SendAsync();
            return httpResponse;
        }

        // Method = "GET", UriTemplate = "{subscriptionId}/cloudservices"
        public async Task<IHttpResponseMessageAbstraction> ListCloudServices()
        {
            int i = 0;
            var start = DateTime.UtcNow;
            var timingManager = ServiceLocator.Instance.Locate<IHttpOperationManager>();
            var factory = ServiceLocator.Instance.Locate<IHDInsightHttpClientAbstractionFactory>();
            var result = await factory.Retry(this.credentials,
                                             this.context,
                                             this.ProcessListCloudServices,
                                             r =>
                                             {
                                                 i++;
                                                 return r.StatusCode != HttpStatusCode.Accepted && r.StatusCode != HttpStatusCode.OK;
                                             },
                                             timingManager.RetryCount,
                                             timingManager.RetryInterval,
                                             this.ignoreSslErrors);

            if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpLayerException(result.StatusCode, result.Content, i, DateTime.UtcNow - start);
            }
            return result;
        }

        // Method = "PUT", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceProviderNamespace}/{resourceType}/{resourceName}"
        public async Task<IHttpResponseMessageAbstraction> CreateResource(string resourceId, string resourceType, string location, string clusterPayload)
        {
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.context, this.ignoreSslErrors);
            var uriBuilder = overrideHandlers.UriBuilder;
            // Creates an HTTP client
            using (var client = ServiceLocator.Instance.Locate<IHDInsightHttpClientAbstractionFactory>().Create(this.credentials, this.context, this.ignoreSslErrors))
            {
                client.RequestUri = uriBuilder.GetCreateResourceUri(resourceId, resourceType, location);
                client.Method = HttpMethod.Put;
                client.RequestHeaders.Add(HDInsightRestConstants.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestConstants.SchemaVersion2);
                client.RequestHeaders.Add(HDInsightRestConstants.Accept);
                client.Content = clusterPayload;

                var httpResponse = await client.SendAsync();
                if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                {
                    throw new HttpLayerException(httpResponse.StatusCode,
                                                            httpResponse.Content)
                    {
                        HelpLink = HelpLinkForException
                    };
                }
                return httpResponse;
            }
        }

        // Method = "PUT", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceProviderNamespace}/{resourceType}/{resourceName}"
        public async Task<IHttpResponseMessageAbstraction> CreateContainer(string dnsName, string location, string clusterPayload)
        {
            return await this.CreateResource(dnsName,
                                             "containers",
                                             location,
                                             clusterPayload);
        }

        // Method = "DELETE", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/{resourceProviderNamespace}/{resourceType}/{resourceName}"
        public async Task<IHttpResponseMessageAbstraction> DeleteContainer(string dnsName, string location)
        {
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.context, this.ignoreSslErrors);
            var uriBuilder = overrideHandlers.UriBuilder;
            // Creates an HTTP client
            using (var client = ServiceLocator.Instance.Locate<IHDInsightHttpClientAbstractionFactory>().Create(this.credentials, this.context, this.ignoreSslErrors))
            {
                client.RequestUri = uriBuilder.GetDeleteContainerUri(dnsName, location);

                client.Method = HttpMethod.Delete;
                client.RequestHeaders.Add(HDInsightRestConstants.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestConstants.Accept);

                var httpResponse = await client.SendAsync();
                if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                {
                    throw new HttpLayerException(httpResponse.StatusCode,
                                                            httpResponse.Content)
                    {
                        HelpLink = HelpLinkForException
                    };
                }
                return httpResponse;
            }
        }

        // Method = "POST", UriTemplate = "{subscriptionId}/cloudservices/{cloudServiceName}/resources/hdinsight/~/containers/{containerName}/services/http"
        public async Task<IHttpResponseMessageAbstraction> EnableDisableUserChangeRequest(string dnsName, string location, UserChangeRequestUserType requestType, string payload)
        {
            var manager = ServiceLocator.Instance.Locate<IUserChangeRequestManager>();
            var handler = manager.LocateUserChangeRequestHandler(this.credentials.GetType(), requestType);
            // Creates an HTTP client
            if (handler.IsNull())
            {
                throw new NotSupportedException("Request to submit a UserChangeRequest that is not supported by this client");
            }
            using (IHttpClientAbstraction client = ServiceLocator.Instance.Locate<IHDInsightHttpClientAbstractionFactory>().Create(this.credentials, this.context, this.ignoreSslErrors))
            {
                var hadoopContext = new HDInsightSubscriptionAbstractionContext(this.credentials, this.context);
                client.RequestUri = handler.Item1(hadoopContext, dnsName, location);
                client.Method = HttpMethod.Post;
                client.RequestHeaders.Add(HDInsightRestConstants.XMsVersion);
                client.RequestHeaders.Add(HDInsightRestConstants.SchemaVersion2);
                client.RequestHeaders.Add(HDInsightRestConstants.Accept);
                client.Content = payload;

                IHttpResponseMessageAbstraction httpResponse = await client.SendAsync();
                if (httpResponse.StatusCode != HttpStatusCode.Accepted)
                {
                    throw new HttpLayerException(httpResponse.StatusCode, httpResponse.Content)
                    {
                        HelpLink = HelpLinkForException
                    };
                }
                return httpResponse;
            }
        }

        public async Task<IHttpResponseMessageAbstraction> ProcessGetOperationStatus(IHttpClientAbstraction client, string dnsName, string location, Guid operationId)
        {
            var httpLogic = ServiceLocator.Instance.Locate<IHttpOperationManager>();
            client.Timeout = httpLogic.HttpOperationTimeout;
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(this.credentials, this.context, this.ignoreSslErrors);
            var uriBuilder = overrideHandlers.UriBuilder;
            client.RequestUri = uriBuilder.GetOperationStatusUri(dnsName, location, this.credentials.DeploymentNamespace, operationId);
            client.Method = HttpMethod.Get;
            client.RequestHeaders.Add(HDInsightRestConstants.XMsVersion);
            client.RequestHeaders.Add(HDInsightRestConstants.SchemaVersion2);

            IHttpResponseMessageAbstraction httpResponse = await client.SendAsync();
            return httpResponse;
        }

        // Method = "GET", UriTemplate = "/{subscriptionId}/cloudservices/{cloudServiceName}/resources/{deploymentNamespace}/~/containers/{containerName}/users/operations/{operationId}",
        public async Task<IHttpResponseMessageAbstraction> GetOperationStatus(string dnsName, string location, Guid operationId)
        {
            int i = 0;
            var start = DateTime.UtcNow;
            var timingManager = ServiceLocator.Instance.Locate<IHttpOperationManager>();
            var factory = ServiceLocator.Instance.Locate<IHDInsightHttpClientAbstractionFactory>();
            var result = await factory.Retry(this.credentials,
                                             this.context,
                                             (client) => this.ProcessGetOperationStatus(client, dnsName, location, operationId),
                                             r =>
                                             {
                                                 i++;
                                                 return r.StatusCode != HttpStatusCode.Accepted && r.StatusCode != HttpStatusCode.OK;
                                             },
                                             timingManager.RetryCount,
                                             timingManager.RetryInterval,
                                             this.ignoreSslErrors);

            if (result.StatusCode != HttpStatusCode.Accepted && result.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpLayerException(result.StatusCode, result.Content, i, DateTime.UtcNow - start);
            }
            return result;
        }

        public ILogger Logger { get; private set; }
    }
}
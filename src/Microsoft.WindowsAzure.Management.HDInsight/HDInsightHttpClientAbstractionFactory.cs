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
namespace Microsoft.WindowsAzure.Management.HDInsight
{
    using System;
    using System.Globalization;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;

    /// <summary>
    ///     Provides a factory for a class that Abstracts Http client requests.
    /// </summary>
    internal class HDInsightHttpClientAbstractionFactory : IHDInsightHttpClientAbstractionFactory
    {
        /// <summary>
        ///     Creates a new HttpClientAbstraction class.
        /// </summary>
        /// <param name="credentials">
        ///     The credentials to use.
        /// </param>
        /// <param name="context">
        ///     The context to use.
        /// </param>
        /// <returns>
        ///     A new instance of the HttpClientAbstraction.
        /// </returns>
        public IHttpClientAbstraction Create(IHDInsightSubscriptionCredentials credentials, HDInsight.IAbstractionContext context)
        {
            IHDInsightCertificateCredential certCreds = credentials as IHDInsightCertificateCredential;
            IHDInsightAccessTokenCredential tokenCreds = credentials as IHDInsightAccessTokenCredential;
            if (certCreds != null)
            {
                return
                    ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                  .Create(certCreds.Certificate, context);
            }
            if (tokenCreds != null)
            {
                return
                    ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                  .Create(tokenCreds.AccessToken, context);
            }
            throw new NotSupportedException("Credential Type is not supported");
        }

        /// <summary>
        ///     Creates a new HttpClientAbstraction class.
        /// </summary>
        /// <param name="credentials">
        ///     The credentials to use.
        /// </param>
        /// <returns>
        ///     A new instance of the HttpClientAbstraction.
        /// </returns>
        public IHttpClientAbstraction Create(IHDInsightSubscriptionCredentials credentials)
        {
            IHDInsightCertificateCredential certCreds = credentials as IHDInsightCertificateCredential;
            IHDInsightAccessTokenCredential tokenCreds = credentials as IHDInsightAccessTokenCredential;
            if (certCreds != null)
            {
                return
                    ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                  .Create(certCreds.Certificate);
            }
            if (tokenCreds != null)
            {
                return
                    ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                  .Create(tokenCreds.AccessToken);
            }
            throw new NotSupportedException("Credential Type is not supported");
        }

        /// <summary>
        ///     Creates a new HttpClientAbstraction class.
        /// </summary>
        /// <param name="context">
        ///     The context to use.
        /// </param>
        /// <returns>
        ///     A new instance of the HttpClientAbstraction.
        /// </returns>
        public IHttpClientAbstraction Create(HDInsight.IAbstractionContext context)
        {
            return ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                              .Create(context);
        }

        /// <summary>
        ///     Creates a new HttpClientAbstraction class.
        /// </summary>
        /// <returns>
        ///     A new instance of the HttpClientAbstraction.
        /// </returns>
        public IHttpClientAbstraction Create()
        {
            return ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                              .Create();
        }

        /// <inheritdoc />
        public async Task<IHttpResponseMessageAbstraction> Retry(IHDInsightSubscriptionCredentials credentials,
                                                                 IAbstractionContext context,
                                                                 Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation,
                                                                 Func<IHttpResponseMessageAbstraction, bool> shouldRetry,
                                                                 TimeSpan timeout,
                                                                 TimeSpan pollInterval)
        {
            IHDInsightCertificateCredential certCreds = credentials as IHDInsightCertificateCredential;
            IHDInsightAccessTokenCredential tokenCreds = credentials as IHDInsightAccessTokenCredential;
            if (certCreds != null)
            {
                return await ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                                    .Retry(certCreds.Certificate, context, operation, shouldRetry, timeout, pollInterval);
            }
            if (tokenCreds != null)
            {
                return await ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                                    .Retry(tokenCreds.AccessToken, context, operation, shouldRetry, timeout, pollInterval);
            }
            throw new NotSupportedException("Credential Type is not supported");
        }

        /// <inheritdoc />
        public async Task<IHttpResponseMessageAbstraction> Retry(IHDInsightSubscriptionCredentials credentials, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            IHDInsightCertificateCredential certCreds = credentials as IHDInsightCertificateCredential;
            IHDInsightAccessTokenCredential tokenCreds = credentials as IHDInsightAccessTokenCredential;
            if (certCreds != null)
            {
                return await ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                                    .Retry(certCreds.Certificate, operation, shouldRetry, timeout, pollInterval);
            }
            if (tokenCreds != null)
            {
                return await ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                                    .Retry(tokenCreds.AccessToken, operation, shouldRetry, timeout, pollInterval);
            }
            throw new NotSupportedException("Credential Type is not supported");
        }

        /// <inheritdoc />
        public async Task<IHttpResponseMessageAbstraction> Retry(IAbstractionContext context, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return await ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                                .Retry(context, operation, shouldRetry, timeout, pollInterval);
        }

        /// <inheritdoc />
        public async Task<IHttpResponseMessageAbstraction> Retry(Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return await ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>()
                                                .Retry(operation, shouldRetry, timeout, pollInterval);
        }
    }
}

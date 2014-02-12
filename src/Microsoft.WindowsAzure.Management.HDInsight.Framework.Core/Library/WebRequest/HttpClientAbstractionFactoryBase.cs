namespace Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    internal abstract class HttpClientAbstractionFactoryBase : IHttpClientAbstractionFactory
    {
        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(X509Certificate2 cert);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(X509Certificate2 cert, IAbstractionContext context);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(string token);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(string token, IAbstractionContext context);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create();

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(IAbstractionContext context);

        internal async Task<IHttpResponseMessageAbstraction> Retry(Func<IHttpClientAbstraction> create,
                                                                   CancellationToken cancellationToken,
                                                                   Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation,
                                                                   Func<IHttpResponseMessageAbstraction, bool> shouldRetry,
                                                                   TimeSpan timeout,
                                                                   TimeSpan pollInterval)
        {
            create.ArgumentNotNull("create");
            operation.ArgumentNotNull("operation");
            shouldRetry.ArgumentNotNull("shouldRetry");
            IHttpResponseMessageAbstraction response = null;
            bool shouldContinue = true;
            var start = DateTime.Now;
            Exception foundException = null;
            int attempts = 0;
            do
            {
                try
                {
                    foundException = null;
                    var client = create();
                    var currentAttempt = attempts++;
                    if (currentAttempt > 0)
                    {
                        client.Log(Severity.Warning,
                                   Verbosity.Detailed,
                                   string.Format(CultureInfo.InvariantCulture, "\r\nRetrying Operation because previous attempt resulted in error.  Current Retry attempt: {0}\r\n\r\n", currentAttempt));
                    }
                    response = await operation(client);
                    shouldContinue = response == null || shouldRetry(response);
                    if (shouldContinue)
                    {
                        cancellationToken.WaitForInterval(pollInterval);
                    }
                }
                catch (Exception ex)
                {
                    var hlex = ex as HttpLayerException;
                    if (hlex.IsNotNull())
                    {
                        foundException = ex;
                        cancellationToken.WaitForInterval(pollInterval);
                        continue;
                    }
                    var webex = ex as WebException;
                    if (webex.IsNotNull())
                    {
                        foundException = ex;
                        cancellationToken.WaitForInterval(pollInterval);
                        continue;
                    }
                        throw;
                }
            } 
            while (shouldContinue && DateTime.Now - start < timeout);
            
            if (foundException.IsNotNull())
            {
                foundException.Rethrow();
            }
            return response;
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(X509Certificate2 cert, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return this.Retry(() => this.Create(cert), CancellationToken.None, operation, shouldRetry, timeout, pollInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(X509Certificate2 cert, IAbstractionContext context, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return this.Retry(() => this.Create(cert, context), context.CancellationToken, operation, shouldRetry, timeout, pollInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(string token, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return this.Retry(() => this.Create(token), CancellationToken.None, operation, shouldRetry, timeout, pollInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(string token, IAbstractionContext context, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return this.Retry(() => this.Create(token, context), context.CancellationToken, operation, shouldRetry, timeout, pollInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return this.Retry(this.Create, CancellationToken.None, operation, shouldRetry, timeout, pollInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(IAbstractionContext context, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, TimeSpan timeout, TimeSpan pollInterval)
        {
            return this.Retry(() => this.Create(context), context.CancellationToken, operation, shouldRetry, timeout, pollInterval);
        }
    }
}

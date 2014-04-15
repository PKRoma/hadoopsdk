namespace Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    internal abstract class HttpClientAbstractionFactoryBase : IHttpClientAbstractionFactory
    {
        /// <inheritdoc />
        public IHttpClientAbstraction Create(X509Certificate2 cert)
        {
            return this.Create(cert, false);
        }

        /// <inheritdoc />
        public IHttpClientAbstraction Create(X509Certificate2 cert, IAbstractionContext context)
        {
            return this.Create(cert, context, false);
        }

        /// <inheritdoc />
        public IHttpClientAbstraction Create(string token)
        {
            return this.Create(token, false);
        }

        /// <inheritdoc />
        public IHttpClientAbstraction Create(string token, IAbstractionContext context)
        {
            return this.Create(token, context, false);
        }

        /// <inheritdoc />
        public IHttpClientAbstraction Create()
        {
            return this.Create(false);
        }

        /// <inheritdoc />
        public IHttpClientAbstraction Create(IAbstractionContext context)
        {
            return this.Create(context, false);
        }

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(X509Certificate2 cert, bool ignoreSslErrors);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(X509Certificate2 cert, IAbstractionContext context, bool ignoreSslErrors);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(string token, bool ignoreSslErrors);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(string token, IAbstractionContext context, bool ignoreSslErrors);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(bool ignoreSslErrors);

        /// <inheritdoc />
        public abstract IHttpClientAbstraction Create(IAbstractionContext context, bool ignoreSslErrors);

        internal async Task<IHttpResponseMessageAbstraction> Retry(Func<IHttpClientAbstraction> create,
                                                                   CancellationToken cancellationToken,
                                                                   Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation,
                                                                   Func<IHttpResponseMessageAbstraction, bool> shouldRetry,
                                                                   int retryCount,
                                                                   TimeSpan retryInterval)
        {
            create.ArgumentNotNull("create");
            operation.ArgumentNotNull("operation");
            shouldRetry.ArgumentNotNull("shouldRetry");
            IHttpResponseMessageAbstraction response = null;
            bool shouldContinue = true;
            var start = DateTime.Now;
            Exception foundException = null;
            int attempts = 0;
            int currentAttempt = 0;
            IHttpClientAbstraction client = null;
            do
            {
                try
                {
                    foundException = null;
                    client = create();
                    cancellationToken.ThrowIfCancellationRequested();
                    currentAttempt = attempts++;
                    if (currentAttempt > 0)
                    {
                        client.LogMessage(string.Format(CultureInfo.InvariantCulture, "\r\nRetrying Operation because previous attempt resulted in error.  Current Retry attempt: {0}\r\n\r\n", currentAttempt),
                                          Severity.Informational,
                                          Verbosity.Detailed);
                    }
                    response = await operation(client);
                    shouldContinue = response == null || shouldRetry(response);
                    if (shouldContinue)
                    {
                        cancellationToken.WaitForInterval(retryInterval);
                    }
                }
                catch (Exception ex)
                {
                    ex = ex.GetFirstException();
                    var hlex = ex as HttpLayerException;
                    var httpEx = ex as HttpRequestException;
                    var webex = ex as WebException;
                    var timeOut = ex as TimeoutException;
                    var taskCancled = ex as TaskCanceledException;
                    var operationCanceled = ex as OperationCanceledException;
                    if (taskCancled.IsNotNull() && taskCancled.CancellationToken.IsNotNull() && taskCancled.CancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    if (operationCanceled.IsNotNull() && operationCanceled.CancellationToken.IsNotNull() && operationCanceled.CancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    if (hlex.IsNotNull() || httpEx.IsNotNull() || webex.IsNotNull() || taskCancled.IsNotNull() || timeOut.IsNotNull() || operationCanceled.IsNotNull())
                    {
                        foundException = ex;
                        cancellationToken.WaitForInterval(retryInterval);
                        var msg = string.Format(CultureInfo.InvariantCulture,
                                                "\r\nRetrying Operation because previous attempt resulted in an exception that could be managed.  Current Retry attempt: {0}\r\n\r\n",
                                                currentAttempt);
                        client.LogMessage(msg,
                                          Severity.Informational, 
                                          Verbosity.Detailed);
                        client.LogMessage(ex.ToString(), Severity.Informational, Verbosity.Detailed);
                        continue;
                    }
                    client.LogMessage("Operation aborting because it received an exception that could not be managed.", Severity.Error, Verbosity.Normal);
                    client.LogException(ex);
                    throw;
                }
            } 
            while (shouldContinue && currentAttempt < retryCount);
            
            if (foundException.IsNotNull())
            {
                client.LogMessage("Operation aborting because it received an excessive count of errors.", Severity.Error, Verbosity.Normal);
                client.LogException(foundException);
                foundException.Rethrow();
            }
            return response;
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(X509Certificate2 cert, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, int retryCount, TimeSpan retryInterval, bool ignoreSslErrors)
        {
            return this.Retry(() => this.Create(cert, ignoreSslErrors), CancellationToken.None, operation, shouldRetry, retryCount, retryInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(X509Certificate2 cert, IAbstractionContext context, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, int retryCount, TimeSpan retryInterval, bool ignoreSslErrors)
        {
            return this.Retry(() => this.Create(cert, context, ignoreSslErrors), context.CancellationToken, operation, shouldRetry, retryCount, retryInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(string token, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, int retryCount, TimeSpan retryInterval, bool ignoreSslErrors)
        {
            return this.Retry(() => this.Create(token, ignoreSslErrors), CancellationToken.None, operation, shouldRetry, retryCount, retryInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(string token, IAbstractionContext context, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, int retryCount, TimeSpan retryInterval, bool ignoreSslErrors)
        {
            return this.Retry(() => this.Create(token, context, ignoreSslErrors), context.CancellationToken, operation, shouldRetry, retryCount, retryInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, int retryCount, TimeSpan retryInterval, bool ignoreSslErrors)
        {
            return this.Retry(() => this.Create(ignoreSslErrors), CancellationToken.None, operation, shouldRetry, retryCount, retryInterval);
        }

        /// <inheritdoc />
        public Task<IHttpResponseMessageAbstraction> Retry(IAbstractionContext context, Func<IHttpClientAbstraction, Task<IHttpResponseMessageAbstraction>> operation, Func<IHttpResponseMessageAbstraction, bool> shouldRetry, int retryCount, TimeSpan retryInterval, bool ignoreSslErrors)
        {
            return this.Retry(() => this.Create(context, ignoreSslErrors), context.CancellationToken, operation, shouldRetry, retryCount, retryInterval);
        }
    }
}

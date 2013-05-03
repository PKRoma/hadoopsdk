namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal class AsvClient : IAsvClient
    {
        internal AsvClient()
        {
        }

        public async Task ValidateAccount(string fullAccount, string key)
        {
            try
            {
                // Creates an HTTP client
                using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create())
                {
                    // Prepares the request
                    client.Method = HttpMethod.Get;
                    client.RequestUri = new Uri(string.Format(CultureInfo.InvariantCulture,
                                                              "http://{0}/?comp=list",
                                                              fullAccount));
                    client.RequestHeaders.Add(HDInsightRestHardcodes.XMsDate, DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture));
                    client.RequestHeaders.Add(HDInsightRestHardcodes.AsvXMsVersion);

                    string resourceString = string.Format(CultureInfo.InvariantCulture,
                                                          "/{0}/\ncomp:list",
                                                          ExtractAccount(fullAccount));
                    client.RequestHeaders.Add("Authorization",
                                              GenerateAuthenticator(
                                                  fullAccount,
                                                  key,
                                                  resourceString,
                                                  client.RequestHeaders));

                    // Sends, validates and parses the response
                    using (var httpResponse = await client.SendAsync())
                    {
                        if (httpResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw new HDInsightRestClientException(httpResponse.StatusCode,
                                                                   httpResponse.Content);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture,
                                                                     "Validating connection to '{0}' failed. Inner exception:{1}",
                                                                     fullAccount,
                                                                     e.Message),
                                                       e);
            }
        }
        
        public async Task ValidateContainer(string fullAccount, string key, string container)
        {
            try
            {
                 // Creates an HTTP client
                using (var client = ServiceLocator.Instance.Locate<IHttpClientAbstractionFactory>().Create())
                {
                    // Prepares the request
                    client.Method = HttpMethod.Get;
                    client.RequestUri = new Uri(string.Format("http://{0}/{1}?restype=container&comp=metadata",
                                                              fullAccount,
                                                              container));
                    client.RequestHeaders.Add(HDInsightRestHardcodes.XMsDate, DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture));
                    client.RequestHeaders.Add(HDInsightRestHardcodes.AsvXMsVersion);

                    string resourceString = string.Format("/{0}/{1}\ncomp:metadata\nrestype:container", ExtractAccount(fullAccount), container);
                    client.RequestHeaders.Add("Authorization",
                                              GenerateAuthenticator(
                                                  fullAccount,
                                                  key,
                                                  resourceString,
                                                  client.RequestHeaders));
            
                    // Sends, validates and parses the response
                    using (var httpResponse = await client.SendAsync())
                    {
                        if (httpResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw new HDInsightRestClientException(httpResponse.StatusCode, httpResponse.Content);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture,
                                                                     "Validating container '{0}' (storage '{1}') failed. Inner exception:{2}",
                                                                     container,
                                                                     fullAccount,
                                                                     e.Message),
                                                       e);
            }
        }

        private static string ExtractAccount(string fullAcount)
        {
            return fullAcount.Split(new char[] { '.' })[0];
        }

        private static string GenerateAuthenticator(string fullAccount, string key, string resourceString, IEnumerable<KeyValuePair<string, string>> headers)
        {
            // Generates a signature that matches the HTTP request
            string headerString = string.Join("\n", headers.Select(pair => string.Format(CultureInfo.InvariantCulture, "{0}:{1}", pair.Key, pair.Value)));
            string signature = string.Format(CultureInfo.InvariantCulture, "GET\n\n\n\n\n\n\n\n\n\n\n\n{0}\n{1}", headerString, resourceString);

            // Creates the Authenticator token
            using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(key)))
            {
                var hash = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(signature));
                return string.Format(CultureInfo.InvariantCulture,
                                     "SharedKey {0}:{1}",
                                     ExtractAccount(fullAccount),
                                     Convert.ToBase64String(hash));
            }
        }
    }
}

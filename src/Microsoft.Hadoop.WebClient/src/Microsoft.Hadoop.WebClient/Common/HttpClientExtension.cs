using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebClient.Common
{
    static class HttpClientExtension
    {
        internal static async Task<HttpResponseMessage> SendAsyncRequestForNewJob(this HttpClient client, HttpMethod method, string requestUri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                request.Content = new FormUrlEncodedContent(parameters.Where(kvp => kvp.Value != null));
            }
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            return response;
        }

    }
}

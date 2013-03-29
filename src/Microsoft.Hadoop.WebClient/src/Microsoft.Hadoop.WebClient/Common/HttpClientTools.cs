using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Hadoop.WebClient.Common
{
    public class HttpClientTools
    {
        public static void WaitForTaskCompletion(Task<HttpResponseMessage> t1)
        {
            t1.Wait();
            var response = t1.Result;
            var output = response.Content.ReadAsAsync<JObject>();
            output.Wait();
            response.EnsureSuccessStatusCode();
        }

        public static JObject GetTaskResults(Task<HttpResponseMessage> t1)
        {
            t1.Wait();
            var response = t1.Result;
            var output = response.Content.ReadAsAsync<JObject>();
            output.Wait();
            response.EnsureSuccessStatusCode();

            JObject res = output.Result;
            return res;
        }

    }
}

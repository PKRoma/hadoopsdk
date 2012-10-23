using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Hadoop.WebHDFS
{

    // based off of : http://hadoop.apache.org/docs/r1.0.0/webhdfs.html#OPEN
    //todo:
    //  Auth stuff
    //  Error handling & exception parsing... 
    // operations:
    //  Set Permission
    //  Set Owner
    //  Set Replication Factor 
    //  Set Access or Modification Time 
    //  Delegation Token's
    public class WebHDFSClient
    {
        #region "read"

        public async static Task<DirectoryListing> GetDirectoryStatus(string path)
        {
            JObject files = await GetWebHDFS(path, "LISTSTATUS");
            return new DirectoryListing(files);
        }

        public async static Task<DirectoryEntry> GetFileStatus(string path)
        {
            JObject file = await GetWebHDFS(path, "GETFILESTATUS");
            return new DirectoryEntry(file.Value<JObject>("FileStatus"));
        }

        public async static Task<ContentSummary> GetContentSummary(string path)
        {
            JObject contentSummary = await GetWebHDFS(path, "GETCONTENTSUMMARY");
            return new ContentSummary(contentSummary.Value<JObject>("ContentSummary"));
        }

        public async static Task<FileChecksum> GetFileChecksum(string path)
        {
            JObject fileChecksum = await GetWebHDFS(path, "GETFILECHECKSUM");
            return new FileChecksum(fileChecksum.Value<JObject>("FileChecksum"));
        }

        // todo, overloads with offset & length
        public async static Task<HttpResponseMessage> OpenFile(string path)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.GetAsync(GetRootUri() + path + "?op=OPEN");
            return resp;
        }

        #endregion

        #region "put"
        // todo: add permissions
        public async static Task<bool> CreateDirectory(string path)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=MKDIRS", null);
            var content = await resp.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");

        }

        public async static Task<bool> RenameDirectory(string path, string newPath)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=RENAME&destination=" + newPath, null);
            var content = await resp.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");

        }

        public async static Task<bool> DeleteDirectory(string path)
        {
            return await DeleteDirectory(path, false); 
        }

        public async static Task<bool> DeleteDirectory(string path, bool recursive)
        {
            // todo, need recursive option here.
            HttpClient hc = new HttpClient();
            var resp = await hc.DeleteAsync(GetRootUri() + path + "?op=DELETE&recursive="+recursive.ToString().ToLower());
            var content = await resp.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");
        }

        // todo: 
        public async static Task<string> CreateFile(string localFile, string remotePath)
        {
            WebRequestHandler wrc = new WebRequestHandler { AllowAutoRedirect = false };
            HttpClient hc = new HttpClient(wrc);
            var resp = await hc.PutAsync(GetRootUri() + remotePath + "?op=CREATE", null);
            var putLocation = resp.Headers.Location;
            StreamContent sc = new StreamContent(System.IO.File.OpenRead(localFile));
            var resp2 = await hc.PutAsync(putLocation, sc);
            return resp2.Headers.Location.ToString();
        }

        public async static Task<string> AppendFile(string localFile, string remotePath)
        {
            WebRequestHandler wrc = new WebRequestHandler { AllowAutoRedirect = false };
            HttpClient hc = new HttpClient(wrc);
            var resp = await hc.PostAsync(GetRootUri() + remotePath + "?op=APPEND", null);
            var postLocation = resp.Headers.Location;
            StreamContent sc = new StreamContent(System.IO.File.OpenRead(localFile));
            var resp2 = await hc.PostAsync(postLocation, sc);
            // oddly, this is returning a 403 forbidden 
            // due to: "IOException","javaClassName":"java.io.IOException","message":"java.io.IOException: 
            // Append to hdfs not supported. Please refer to dfs.support.append configuration parameter.
            return resp2.Headers.Location.ToString();
        }

        #endregion


        private static string GetRootUri()
        {
            // todo: move to some config based mechanism
            return "http://localhost:50070/webhdfs/v1";
        }

        private static async Task<JObject> GetWebHDFS(string path, string operation)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.GetAsync(GetRootUri() + path + "?op=" + operation);
            resp.EnsureSuccessStatusCode();
            JObject files = await resp.Content.ReadAsAsync<JObject>();
            return files;
        }
    }

}

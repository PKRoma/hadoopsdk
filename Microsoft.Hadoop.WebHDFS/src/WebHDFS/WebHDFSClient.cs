using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS
{

    // based off of : http://hadoop.apache.org/docs/r1.0.0/webhdfs.html
    //todo:
    //  Auth stuff
    //  X Error handling & exception parsing... => not processing exceptoin at this point... 
    // operations:
    //  X Set Permission
    //  X Set Owner
    //  X Set Replication Factor 
    //  X Set Access or Modification Time 
    //  Delegation Token's
    //  X == Move off of static to ctor
    //  X add documentation
    public class WebHDFSClient
    {
        //  "http://localhost:50070/webhdfs/v1";
        private string clusterHost = "localhost";
        private string clusterPort = "50070";

        public WebHDFSClient() { }
        
        public WebHDFSClient(string clusterHost)
        {
            this.clusterHost = clusterHost;
        }

        public WebHDFSClient(string clusterHost, string clusterPort)
        {
            this.clusterHost = clusterHost;
            this.clusterPort = clusterPort;
        }
        
        #region "read"

        /// <summary>
        /// List the statuses of the files/directories in the given path if the path is a directory. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async  Task<DirectoryListing> GetDirectoryStatus(string path)
        {
            JObject files = await GetWebHDFS(path, "LISTSTATUS");
            return new DirectoryListing(files);
        }

        /// <summary>
        /// Return a file status object that represents the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async  Task<DirectoryEntry> GetFileStatus(string path)
        {
            JObject file = await GetWebHDFS(path, "GETFILESTATUS");
            return new DirectoryEntry(file.Value<JObject>("FileStatus"));
        }

        /// <summary>
        /// Return the current user's home directory in this filesystem. 
        /// The default implementation returns "/user/$USER/". 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetHomeDirectory()
        {
            JObject path = await GetWebHDFS("/", "GETHOMEDIRECTORY");
            return path.Value<string>("Path");
        }

        /// <summary>
        /// Return the ContentSummary of a given Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async  Task<ContentSummary> GetContentSummary(string path)
        {
            JObject contentSummary = await GetWebHDFS(path, "GETCONTENTSUMMARY");
            return new ContentSummary(contentSummary.Value<JObject>("ContentSummary"));
        }

        /// <summary>
        /// Get the checksum of a file
        /// </summary>
        /// <param name="path">The file checksum. The default return value is null, which 
        /// indicates that no checksum algorithm is implemented in the corresponding FileSystem. </param>
        /// <returns></returns>
        public async  Task<FileChecksum> GetFileChecksum(string path)
        {
            JObject fileChecksum = await GetWebHDFS(path, "GETFILECHECKSUM");
            return new FileChecksum(fileChecksum.Value<JObject>("FileChecksum"));
        }

        // todo, overloads with offset & length
        /// <summary>
        /// Opens an FSDataInputStream at the indicated Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async  Task<HttpResponseMessage> OpenFile(string path)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.GetAsync(GetRootUri() + path + "?op=OPEN");
            resp.EnsureSuccessStatusCode();
            return resp;
        }

        /// <summary>
        /// Opens an FSDataInputStream at the indicated Path.  The offset and length will allow 
        /// you to get a subset of the file.  
        /// </summary>
        /// <param name="path"></param>
        /// <param name="offset">This includes any header bytes</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> OpenFile(string path, int offset, int length)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.GetAsync(GetRootUri() + path + "?op=OPEN&offset="+offset.ToString() + "&length="+length.ToString());
            resp.EnsureSuccessStatusCode();
            return resp;
        }

        #endregion

        #region "put"
        // todo: add permissions
        /// <summary>
        /// Make the given file and all non-existent parents into directories. 
        /// Has the semantics of Unix 'mkdir -p'. Existence of the directory hierarchy is not an error. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async  Task<bool> CreateDirectory(string path)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=MKDIRS", null);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");

        }

        /// <summary>
        /// Renames Path src to Path dst.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public async  Task<bool> RenameDirectory(string path, string newPath)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=RENAME&destination=" + newPath, null);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");

        }

        /// <summary>
        /// Delete a file.  Note, this will not recursively delete and will
        /// not delete if directory is not empty
        /// </summary>
        /// <param name="path">the path to delete</param>
        /// <returns>true if delete is successful else false. </returns>
        public Task<bool> DeleteDirectory(string path)
        {
            return DeleteDirectory(path, false); 
        }


        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="path">the path to delete</param>
        /// <param name="recursive">if path is a directory and set to true, the directory is deleted else throws an exception.
        /// In case of a file the recursive can be set to either true or false. </param>
        /// <returns>true if delete is successful else false. </returns>
        public async  Task<bool> DeleteDirectory(string path, bool recursive)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.DeleteAsync(GetRootUri() + path + "?op=DELETE&recursive="+recursive.ToString().ToLower());
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");
        }

        /// <summary>
        /// Set permission of a path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="permissions"></param>
        public async Task<bool> SetPermissions(string path, string permissions)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=SETPERMISSION&permission=" + permissions, null);
            resp.EnsureSuccessStatusCode();
            return true;
        }

        /// <summary>
        /// Sets the owner for the file 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="owner">If it is null, the original username remains unchanged</param>
        public async Task<bool> SetOwner(string path, string owner)
        {
            // todo, add group
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=SETOWNER&owner=" + owner,null);
            resp.EnsureSuccessStatusCode();
            return true;
        }

        /// <summary>
        /// Sets the group for the file 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="group">If it is null, the original groupname remains unchanged</param>
        public async Task<bool> SetGroup(string path, string group)
        {
            // todo, add group
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=SETOWNER&group=" + group, null);
            resp.EnsureSuccessStatusCode();
            return true;
        }

        /// <summary>
        /// Set replication for an existing file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="replicationFactor"></param>
        /// <returns></returns>
        public async Task<bool> SetReplicationFactor(string path, int replicationFactor)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=SETREPLICATION&replication="+ replicationFactor.ToString(),null);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsAsync<JObject>();
            return content.Value<bool>("boolean");
        }

        /// <summary>
        /// Set access time of a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="accessTime">Set the access time of this file. The number of milliseconds since Jan 1, 1970. 
        /// A value of -1 means that this call should not set access time</param>
        public async Task<bool> SetAccessTime(string path, string accessTime)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=SETTIMES&accesstime=" + accessTime, null);
            resp.EnsureSuccessStatusCode();
            return true;
        }

        /// <summary>
        /// Set modification time of a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="modificationTime">Set the modification time of this file. The number of milliseconds since Jan 1, 1970.
        /// A value of -1 means that this call should not set modification time</param>
        public async Task<bool> SetModificationTime(string path, string modificationTime)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.PutAsync(GetRootUri() + path + "?op=SETTIMES&modificationtime=" + modificationTime, null);
            resp.EnsureSuccessStatusCode();
            return true; 
        }

        /// <summary>
        /// Opens an FSDataOutputStream at the indicated Path. Files are overwritten by default.
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async  Task<string> CreateFile(string localFile, string remotePath)
        {
            WebRequestHandler wrc = new WebRequestHandler { AllowAutoRedirect = false };
            HttpClient hc = new HttpClient(wrc);
            var resp = await hc.PutAsync(GetRootUri() + remotePath + "?op=CREATE", null);
            var putLocation = resp.Headers.Location;
            StreamContent sc = new StreamContent(System.IO.File.OpenRead(localFile));
            var resp2 = await hc.PutAsync(putLocation, sc);
            resp2.EnsureSuccessStatusCode();
            return resp2.Headers.Location.ToString();
        }

        /// <summary>
        /// Append to an existing file (optional operation).
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async  Task<string> AppendFile(string localFile, string remotePath)
        {
            WebRequestHandler wrc = new WebRequestHandler { AllowAutoRedirect = false };
            HttpClient hc = new HttpClient(wrc);
            var resp = await hc.PostAsync(GetRootUri() + remotePath + "?op=APPEND", null);
            resp.EnsureSuccessStatusCode();
            var postLocation = resp.Headers.Location;
            StreamContent sc = new StreamContent(System.IO.File.OpenRead(localFile));
            var resp2 = await hc.PostAsync(postLocation, sc);
            resp2.EnsureSuccessStatusCode();
            // oddly, this is returning a 403 forbidden 
            // due to: "IOException","javaClassName":"java.io.IOException","message":"java.io.IOException: 
            // Append to hdfs not supported. Please refer to dfs.support.append configuration parameter.
            return resp2.Headers.Location.ToString();
        }

        #endregion


        private  string GetRootUri()
        {
            // todo: move to some config based mechanism
            //  "http://localhost:50070/webhdfs/v1";
            return "http://" + clusterHost + ":" + clusterPort + "/webhdfs/v1";
        }

        private  async Task<JObject> GetWebHDFS(string path, string operation)
        {
            HttpClient hc = new HttpClient();
            var resp = await hc.GetAsync(GetRootUri() + path + "?op=" + operation);
            resp.EnsureSuccessStatusCode();
            JObject files = await resp.Content.ReadAsAsync<JObject>();
            return files;
        }


    }

}

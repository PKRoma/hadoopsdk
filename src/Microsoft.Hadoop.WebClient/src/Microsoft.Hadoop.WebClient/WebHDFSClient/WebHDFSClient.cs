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


using System.IO;
using Microsoft.Hadoop.WebHDFS.Adapters;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS
{
   using System;
   using System.Globalization;
   using System.Net;

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

    // todo - should split this into two classes - low level HTTP client and high level object model.
    public class WebHDFSClient
    {
        // todo - need to add ability to pass in cancellation token
        
        //  "http://localhost:50070/webhdfs/v1";

        public Uri BaseUri { get; private set; }
        private string homeDirectory = string.Empty;
        private string userName;

        private HttpMessageHandler handler;

        private bool IsAzureConnected()
        {
            return this.handler.GetType() == typeof(BlobStorageAdapter);
        }

        public string GetAbsolutePath(string hdfsPath)
        {
            if (string.IsNullOrEmpty(hdfsPath))
            {
                return "/";
            }
            else if (hdfsPath[0] == '/')
            {
                return hdfsPath;
            }
            else if (hdfsPath.Contains(":"))
            {
                Uri uri = new Uri(hdfsPath);
                return uri.AbsolutePath;
            }
            else
            {
                return this.homeDirectory + "/" + hdfsPath;
            }
        }

        public string GetFullyQualifiedPath(string path)
        {
            path = this.GetAbsolutePath(path);
            if (this.handler == null)
            {
                return "hdfs://" + this.BaseUri.Host + path;
            }
            else
            {
                var asAdapter = (BlobStorageAdapter)this.handler;
                return "asv://" + asAdapter.ContainerName + "@" + asAdapter.Account + path;
            }
        }

        public WebHDFSClient(Uri baseUri, string userName)
        {
            this.userName = userName;
            this.BaseUri = baseUri;
            var homeDirectoryTask = this.GetHomeDirectory();
            homeDirectoryTask.Wait();
            this.homeDirectory = homeDirectoryTask.Result;
        }

        public WebHDFSClient(string hadoopUser, WebHDFSStorageAdapter handler)
        {
            this.homeDirectory = "/user/" + hadoopUser;
            this.BaseUri = handler.BaseUri;
            this.handler = handler;            
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
            if (!string.IsNullOrEmpty(this.homeDirectory))
            {
                JObject path = await GetWebHDFS("/", "GETHOMEDIRECTORY");
                this.homeDirectory = path.Value<string>("Path");
                return this.homeDirectory;
            }
            return this.homeDirectory;
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.GetAsync(this.GetUriForOperation(path) + "op=OPEN");
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.GetAsync(this.GetUriForOperation(path) + "op=OPEN&offset="+offset.ToString() + "&length="+length.ToString());
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=MKDIRS", null);
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=RENAME&destination=" + newPath, null);
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
            HttpClient hc = this.CreateHTTPClient();
            string uri = this.GetUriForOperation(path) + "op=DELETE&recursive=" + recursive.ToString().ToLower();
            var resp = await hc.DeleteAsync(uri);
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=SETPERMISSION&permission=" + permissions, null);
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=SETOWNER&owner=" + owner,null);
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=SETOWNER&group=" + group, null);
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=SETREPLICATION&replication="+ replicationFactor.ToString(),null);
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=SETTIMES&accesstime=" + accessTime, null);
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
            HttpClient hc = this.CreateHTTPClient();
            var resp = await hc.PutAsync(this.GetUriForOperation(path) + "op=SETTIMES&modificationtime=" + modificationTime, null);
            resp.EnsureSuccessStatusCode();
            return true; 
        }

        //private async Task ProcessPutAsync(HttpClient hc, string putLocation, StreamContent sc)
        //{
        //    var response = await hc.PutAsync(putLocation, sc);
        //    if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
        //    {
        //        var uri = response.Content.Headers.ContentLocation;

        //    }
        //    else
        //    {
        //        response.EnsureSuccessStatusCode();
        //    }
        //}

        /// <summary>
        /// Opens an FSDataOutputStream at the indicated Path. Files are overwritten by default.
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async  Task<string> CreateFile(string localFile, string remotePath)
        {  
            HttpClient hc = this.CreateHTTPClient(true);
            var uri = this.GetUriForOperation(remotePath) + "op=CREATE&overwrite=true";
//            var resp = await hc.PutAsync(uri, null);
//            var putLocation = resp.Headers.Location;
            var putLocation = uri;
            StreamContent sc = new StreamContent(System.IO.File.OpenRead(localFile));
            var resp2 = await hc.PutAsync(putLocation, sc);
            resp2.EnsureSuccessStatusCode();
            return resp2.Headers.Location.ToString();
        }

        /// <summary>
        /// Opens an FSDataOutputStream at the indicated Path. Files are overwritten by default.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async Task<string> CreateFile(Stream content, string remotePath)
        {
            HttpClient hc = this.CreateHTTPClient(false);
            var resp = await hc.PutAsync(this.GetUriForOperation(remotePath) + "op=CREATE", null);
            var putLocation = resp.Headers.Location;
            StreamContent sc = new StreamContent(content);
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
            HttpClient hc = this.CreateHTTPClient(false);
            var resp = await hc.PostAsync(this.GetUriForOperation(remotePath) + "op=APPEND", null);
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

        /// <summary>
        /// Append to an existing file (optional operation).
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public async Task<string> AppendFile(Stream content, string remotePath)
        {
            HttpClient hc = this.CreateHTTPClient(false);
            var resp = await hc.PostAsync(this.GetUriForOperation(remotePath) + "op=APPEND", null);
            resp.EnsureSuccessStatusCode();
            var postLocation = resp.Headers.Location;
            StreamContent sc = new StreamContent(content);
            var resp2 = await hc.PostAsync(postLocation, sc);
            resp2.EnsureSuccessStatusCode();
            return resp2.Headers.Location.ToString();
        }

        #endregion

        private HttpClient CreateHTTPClient(bool allowAutoRedirect = true)
        {
            // todo - should probably not create these each time.
            return this.handler != null ? new HttpClient(this.handler) : new HttpClient(new WebRequestHandler { AllowAutoRedirect = allowAutoRedirect });
        }

        private string GetRootUri()
        {
            // todo: move to some config based mechanism
            //  "http://localhost:50070/webhdfs/v1";
            return this.BaseUri + "webhdfs/v1";
        }

        private string GetUriForOperation(string path)
        {
            string uri = this.GetRootUri();
            if (!string.IsNullOrEmpty(path))
            {
                if (path[0] == '/')
                {
                    uri += path;
                }
                else
                {
                    uri += this.homeDirectory + "/" + path;
                }
            }
            uri += "?";
            if (!string.IsNullOrEmpty(this.userName))
            {
                uri += string.Format(CultureInfo.InvariantCulture, "user.name={0}&", this.userName);
            }
            return uri;
        }

        private async Task<JObject> GetWebHDFS(string path, string operation)
        {
            HttpClient hc = this.CreateHTTPClient();
            string uri = this.GetUriForOperation(path);
            uri += "op=" + operation;
            var resp = await hc.GetAsync(uri);
            resp.EnsureSuccessStatusCode();
            JObject files = await resp.Content.ReadAsAsync<JObject>();
            return files;
        }
    }

}

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

namespace Microsoft.Hadoop.WebClient.Storage
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.WebHDFS;

    /// <summary>
    /// Interface that represents a webHdfs http (rest) client
    /// </summary>
    public interface IAzureHdfsHttpClient
    {
        /// <summary>
        /// Method that opens a file using the WebHdfs Restful interface.
        /// </summary>
        /// <param name="path">The path to the file in Hdfs.</param>
        /// <returns>The contents of the file.</returns>
        Task<HttpContent> OpenFile(string path);

        /// <summary>
        /// Method that creates a file using the WebHdfs REST interface.
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        /// <param name="content">The contents of the file.</param>
        /// /// <param name="overwrite">True or False value that determines if the file is overwriten if it exists.</param>
        /// <returns>The location of the new file.</returns>
        Task<string> CreateFile(string path, Stream content, bool overwrite);

        /// <summary>
        /// Method that deletes a file or directory using the WebHdfs REST interface.
        /// </summary>
        /// <param name="path">The path of the item to delete.</param>
        /// <param name="recursive">A True or False value that determines if the item and all of its children should be deleted.</param>
        /// <returns></returns>
        Task<bool> Delete(string path, bool recursive);

        /// <summary>
        /// Method that gets the status of a file using the WebHdfs REST interface.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>Status information for the file.</returns>
        Task<DirectoryEntry> GetFileStatus(string path);
    }
}

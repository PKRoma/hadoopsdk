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
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    interface IHdfsClientAdapter
    {
        Task<bool> CreateDirectory(string path);

        Task<string> DeleteDirectory(string path);

        Task<IEnumerable<object>> GetDirectory(string path);

        Task<bool> DirectoryExists(string path);

        Task<bool> CreateFile(string path, Stream content);

        Task<Stream> OpenFile(string path);

        Task<bool> DeleteFile(string path);

        Task<bool> AppendToFile(string path, Stream content);

        Task<bool> FileExists(string path);

        Task<IEnumerable<object>> GetFiles(string path);

    }
}

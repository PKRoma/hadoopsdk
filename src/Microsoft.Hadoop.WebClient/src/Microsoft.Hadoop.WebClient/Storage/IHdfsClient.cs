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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IHdfsClient
    {
        Task<bool> CreateFolder(string path);
        Task<bool> CreateFile(string path);
        Task<Stream> OpenFile(string path);
        Task<bool> Delete(string path, bool recursive);

        //void WriteAllLines(string hdfsPath, IEnumerable<string> lines);
        //void WriteAllText(string hdfsPath, string text);
        //void WriteAllBytes(string hdfsPath, byte[] bytes);
        //string[] ReadAllLines(string hdfsPath);
        //string ReadAllText(string hdfsPath);
        //byte[] ReadAllBytes(string hdfsPath);
        //bool Exists(string hdfsPath);
        //void MakeDirectory(string hdfsPath);
        //void CopyToLocal(string hdfsPath, string localPath);
        //void CopyFromLocal(string localPath, string hdfsPath);
        //void Delete(string hdfsPath);
        //string[] LsFiles(string hdfsPath);
        //string GetAbsolutePath(string hdfsPath);
        //string GetFullyQualifiedPath(string hdfsPath);
        //IEnumerable<string> EnumerateDataInFolder(string folderName);
        //IEnumerable<string> EnumerateDataInFolder(string folderName, int maxLines, Func<string, bool> fileNamePredicate = null);
    }
}

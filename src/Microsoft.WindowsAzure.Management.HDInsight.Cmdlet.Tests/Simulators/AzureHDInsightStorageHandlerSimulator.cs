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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Simulators
{
    using System;
    using System.IO;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;

    internal class AzureHDInsightStorageHandlerSimulator : IAzureHDInsightStorageHandler
    {
        internal Uri Path { get; private set; }
        internal Stream UploadedStream { get; private set; }

        public Uri GetStoragePath(Uri httpPath)
        {
            return AzureHDInsightStorageHandler.GetWasbStoragePath(httpPath);
        }

        public void UploadFile(Uri path, Stream contents)
        {
            this.Path = path;
            this.UploadedStream = new MemoryStream();
            contents.CopyTo(this.UploadedStream);
        }
    }
}

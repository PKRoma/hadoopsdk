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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;

    internal class NewAzureHDInsightSqoopJobDefinitionCommand
        : AzureHDInsightNewJobDefinitionCommand<AzureHDInsightSqoopJobDefinition>, INewAzureHDInsightSqoopJobDefinitionCommand
    {
        private readonly SqoopJobCreateParameters sqoopJobDefinition = new SqoopJobCreateParameters();
        private string[] resources = new string[] { };

        public string Command
        {
            get { return this.sqoopJobDefinition.Command; }
            set { this.sqoopJobDefinition.Command = value; }
        }

        public override Hashtable Defines { get; set; }

        public string File
        {
            get { return this.sqoopJobDefinition.File; }
            set { this.sqoopJobDefinition.File = value; }
        }

        public override string[] Files
        {
            get { return this.resources; }
            set { this.resources = value; }
        }

        public override string StatusFolder
        {
            get { return this.sqoopJobDefinition.StatusFolder; }
            set { this.sqoopJobDefinition.StatusFolder = value; }
        }

        public override Task EndProcessing()
        {
            if (this.Command.IsNotNullOrEmpty() && this.File.IsNotNullOrEmpty())
            {
                throw new ArgumentException("Only Query or File can be specified, not both.");
            }

            var sqoopJob = new AzureHDInsightSqoopJobDefinition();
            sqoopJob.Command = this.Command;
            sqoopJob.File = this.File;
            sqoopJob.StatusFolder = this.StatusFolder;

            if (sqoopJob.Command.IsNullOrEmpty())
            {
                sqoopJob.File.ArgumentNotNullOrEmpty("File");
            }

            if (this.Files.IsNotNull())
            {
                sqoopJob.Files.AddRange(this.Files);
            }

            this.Output.Add(sqoopJob);
            return TaskEx.GetCompletedTask();
        }
    }
}

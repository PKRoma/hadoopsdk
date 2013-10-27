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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    internal class NewAzureHDInsightPigJobDefinitionCommand : AzureHDInsightNewJobDefinitionCommand<AzureHDInsightPigJobDefinition>, INewAzureHDInsightPigJobDefinitionCommand
    {
        private readonly PigJobCreateParameters pigJobDefinition = new PigJobCreateParameters();
        private Hashtable defines = new Hashtable();
        private string[] resources = new string[] { };

        public override Hashtable Defines
        {
            get { return this.defines; }
            set { this.defines = value; }
        }

        public override string[] Files
        {
            get { return this.resources; }
            set { this.resources = value; }
        }

        public override string StatusFolder
        {
            get { return this.pigJobDefinition.StatusFolder; }
            set { this.pigJobDefinition.StatusFolder = value; }
        }

        public string Query
        {
            get { return this.pigJobDefinition.Query; }
            set { this.pigJobDefinition.Query = value; }
        }

        public string File
        {
            get { return this.pigJobDefinition.File; }
            set { this.pigJobDefinition.File = value; }
        }

        public string[] Arguments { get; set; }

        public override Task EndProcessing()
        {
            if (this.Query.IsNotNullOrEmpty() && this.File.IsNotNullOrEmpty())
            {
                throw new ArgumentException("Only Query or File can be specified, not both.");
            }

            var pigJob = new AzureHDInsightPigJobDefinition();
            pigJob.Query = this.Query;
            pigJob.File = this.File;
            pigJob.StatusFolder = this.StatusFolder;

            if (this.Arguments.IsNotNull())
            {
                pigJob.Arguments.AddRange(this.Arguments);
            }

            if (this.Files.IsNotNull())
            {
                pigJob.Files.AddRange(this.Files);
            }

            this.Output.Add(pigJob);
            return TaskEx.GetCompletedTask();
        }
    }
}

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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;

    /// <summary>
    /// Represents the New-AzureHDInsightSqoopJobDefinition Power Shell Cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.New, AzureHdInsightPowerShellConstants.AzureHDInsightSqoopJobDefinition)]
    public class NewAzureHDInsightSqoopJobDefinitionCmdlet : AzureHDInsightCmdlet, INewAzureHDInsightSqoopJobDefinitionBase
    {
        private readonly INewAzureHDInsightSqoopJobDefinitionCommand command;

        /// <summary>
        /// Initializes a new instance of the NewAzureHDInsightSqoopJobDefinitionCmdlet class.
        /// </summary>
        public NewAzureHDInsightSqoopJobDefinitionCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateNewSqoopDefinition();
        }

        /// <inheritdoc />
        protected override void StopProcessing()
        {
            this.command.Cancel();
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
                   HelpMessage = "The command to run in the sqoop job.")]
        public string Command
        {
            get { return this.command.Command; }
            set { this.command.Command = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
                   HelpMessage = "The query file to run in the sqoop job.")]
        [Alias(AzureHdInsightPowerShellConstants.AliasQueryFile)]
        public string File
        {
            get { return this.command.File; }
            set { this.command.File = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
                   HelpMessage = "The files for the sqoop job.")]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need collections for input parameters")]
        public string[] Files
        {
            get { return this.command.Files; }
            set { this.command.Files = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
                   HelpMessage = "The output location to use for the job.")]
        public string StatusFolder
        {
            get { return this.command.StatusFolder; }
            set { this.command.StatusFolder = value; }
        }

        /// <summary>
        /// Finishes the execution of the cmdlet by writing out the config object.
        /// </summary>
        protected override void EndProcessing()
        {
            if (this.File.IsNullOrEmpty() && this.Command.IsNullOrEmpty())
            {
                throw new PSArgumentException("Either File or Command should be specified for Sqoop jobs.");
            }

            this.command.EndProcessing().Wait();
            foreach (var output in this.command.Output)
            {
                this.WriteObject(output);
            }
            this.WriteDebugLog();
        }
    }
}

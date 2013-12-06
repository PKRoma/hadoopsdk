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
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    /// <summary>
    ///     Cmdlet that lists all the properties of a subscription registered with the HDInsight service.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, AzureHdInsightPowerShellConstants.AzureHDInsightProperties)]
    public class GetAzureHDInsightPropertiesCmdlet : AzureHDInsightCmdlet, IGetAzureHDInsightPropertiesBase
    {
        private readonly IGetAzureHDInsightPropertiesCommand command;

        /// <summary>
        ///     Initializes a new instance of the GetAzureHDInsightPropertiesCmdlet class.
        /// </summary>
        public GetAzureHDInsightPropertiesCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetProperties();
        }

        /// <inheritdoc />
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The management certificate used to manage the Azure subscription.")]
        [Alias(AzureHdInsightPowerShellConstants.AliasCert)]
        public X509Certificate2 Certificate
        {
            get { return this.command.Certificate; }
            set { this.command.Certificate = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "The CloudServiceName to use when managing the HDInsight cluster.")]
        public string CloudServiceName
        {
            get { return this.command.CloudServiceName; }
            set { this.command.CloudServiceName = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The Endpoint to use when connecting to Azure.")]
        public Uri EndPoint
        {
            get { return this.command.EndPoint; }
            set { this.command.EndPoint = value; }
        }

        /// <summary>
        ///     Gets or sets a flag to only show Azure regions available to the subscription.
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Flag to only show Azure regions available to the subscription.")]
        public SwitchParameter Locations { get; set; }

        /// <inheritdoc />
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The subscription id for the Azure subscription.")]
        [Alias(AzureHdInsightPowerShellConstants.AliasSub)]
        public string Subscription
        {
            get { return this.command.Subscription; }
            set { this.command.Subscription = value; }
        }

        /// <summary>
        ///     Gets or sets a flag to only show HDInsight versions available to the subscription.
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Flag to only show HDInsight versions available to the subscription")]
        public SwitchParameter Versions { get; set; }

        /// <summary>
        ///     Finishes the execution of the cmdlet by listing the clusters.
        /// </summary>
        protected override void EndProcessing()
        {
            this.command.Logger = this.Logger;
            try
            {
                this.command.Logger = this.Logger;
                Task task = this.command.EndProcessing();
                CancellationToken token = this.command.CancellationToken;
                while (!task.IsCompleted)
                {
                    this.WriteDebugLog();
                    task.Wait(1000, token);
                }
                if (task.IsFaulted)
                {
                    throw new AggregateException(task.Exception);
                }
                if (this.MyInvocation.BoundParameters.ContainsKey("Debug"))
                {
                    this.WriteObject(this.command.Output.SelectMany(output => output.Capabilities));
                }
                else
                {
                    if (this.Versions.IsPresent)
                    {
                        foreach (AzureHDInsightCapabilities output in this.command.Output)
                        {
                            this.WriteObject(output.Versions);
                        }
                    }
                    else if (this.Locations.IsPresent)
                    {
                        foreach (AzureHDInsightCapabilities output in this.command.Output)
                        {
                            this.WriteObject(output.Locations);
                        }
                    }
                    else
                    {
                        foreach (AzureHDInsightCapabilities output in this.command.Output)
                        {
                            this.WriteObject(output);
                        }
                    }
                }
                this.WriteDebugLog();
            }
            catch (Exception ex)
            {
                Type type = ex.GetType();
                this.Logger.Log(Severity.Error, Verbosity.Normal, this.FormatException(ex));
                this.WriteDebugLog();
                if (type == typeof(AggregateException) || type == typeof(TargetInvocationException) || type == typeof(TaskCanceledException))
                {
                    ex.Rethrow();
                }
                else
                {
                    throw;
                }
            }
            this.WriteDebugLog();
        }

        /// <inheritdoc />
        protected override void StopProcessing()
        {
            this.command.Cancel();
        }
    }
}

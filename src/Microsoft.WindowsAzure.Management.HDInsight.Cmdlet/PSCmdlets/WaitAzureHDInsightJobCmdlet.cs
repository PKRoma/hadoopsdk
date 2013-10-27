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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Management.Automation;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    /// <summary>
    /// Cmdlet that lists all the Jobs running on a HDInsight cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Wait, AzureHdInsightPowerShellConstants.AzureHDInsightJobs, DefaultParameterSetName = AzureHdInsightPowerShellConstants.ParameterSetJobHistoryByName)]
    public class WaitAzureHDInsightJobCmdlet : AzureHDInsightCmdlet, IWaitAzureHDInsightJobBase
    {
        private readonly IWaitAzureHDInsightJobCommand command;

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            DateTime start = DateTime.Now;
            var msg = string.Format(CultureInfo.CurrentCulture, "Waiting for jobDetails Started : {0}", start.ToString(CultureInfo.CurrentCulture));
            this.Logger.Log(Severity.Informational, Verbosity.Detailed, msg);
            try
            {
                this.command.Logger = this.Logger;
                var task = this.command.ProcessRecord();
                var token = this.command.CancellationToken;
                while (!task.IsCompleted)
                {
                    this.WriteDebugLog();
                    if (this.command.JobDetailsStatus.IsNotNull())
                    {
                        msg = string.Format(CultureInfo.CurrentCulture, "Waiting for jobDetails : {0}", this.Job.JobId);
                        var record = new ProgressRecord(0, msg, this.command.JobDetailsStatus.StatusCode.ToString() + " : " + this.command.JobDetailsStatus.PercentComplete);
                        this.WriteProgress(record);
                    }
                    task.Wait(1000, token);
                }
                this.WriteDebugLog();
            }
            catch (Exception ex)
            {
                var type = ex.GetType();
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
            msg = string.Format(CultureInfo.CurrentCulture, "Waiting for jobDetails Stopped : {0}", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            this.Logger.Log(Severity.Informational, Verbosity.Detailed, msg);
            msg = string.Format(CultureInfo.CurrentCulture, "Waiting for jobDetails Executed for {0} minutes", (DateTime.Now - start).TotalMinutes.ToString(CultureInfo.CurrentCulture));
            this.Logger.Log(Severity.Informational, Verbosity.Detailed, msg);
            this.WriteDebugLog();
        }

        /// <inheritdoc />
        protected override void StopProcessing()
        {
            this.command.Cancel();
        }

        /// <summary>
        /// Initializes a new instance of the WaitAzureHDInsightJobCmdlet class.
        /// </summary>
        public WaitAzureHDInsightJobCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateWaitJobs();
        }

        /// <inheritdoc />
        [Parameter(Mandatory = true,
                  HelpMessage = "The Jobs to wait for.",
                  ValueFromPipeline = true)]
        public AzureHDInsightJob Job
        {
            get { return this.command.Job; }
            set { this.command.Job = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
            HelpMessage = "The number of seconds to wait for completion, before cancelling waiting.",
            ValueFromPipeline = true)]
        public double WaitTimeoutInSeconds
        {
            get { return this.command.WaitTimeoutInSeconds; }
            set { this.command.WaitTimeoutInSeconds = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = true,
                  Position = 1,
                  HelpMessage = "The credentials to connect to Azure HDInsight cluster.",
                  ValueFromPipeline = true,
                  ParameterSetName = AzureHdInsightPowerShellConstants.ParameterSetJobHistoryByName)]
        [Alias(AzureHdInsightPowerShellConstants.AliasCredentials)]
        public PSCredential Credential
        {
            get { return this.command.Credential; }
            set { this.command.Credential = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 1, Mandatory = true,
                   HelpMessage = "The subscription id for the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellConstants.ParameterSetJobHistoryByNameWithSpecificSubscriptionCredentials)]
        [Alias(AzureHdInsightPowerShellConstants.AliasSub)]
        public string Subscription
        {
            get { return this.command.Subscription; }
            set { this.command.Subscription = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
                   HelpMessage = "The management certificate used to manage the Azure subscription.",
                   ParameterSetName = AzureHdInsightPowerShellConstants.ParameterSetJobHistoryByNameWithSpecificSubscriptionCredentials)]
        [Alias(AzureHdInsightPowerShellConstants.AliasCert)]
        public X509Certificate2 Certificate
        {
            get { return this.command.Certificate; }
            set { this.command.Certificate = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
                   HelpMessage = "The Endpoint to use when connecting to Azure.",
                   ParameterSetName = AzureHdInsightPowerShellConstants.ParameterSetJobHistoryByNameWithSpecificSubscriptionCredentials)]
        public Uri EndPoint
        {
            get { return this.command.EndPoint; }
            set { this.command.EndPoint = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false,
                   HelpMessage = "The CloudServiceName to use when managing the HDInsight cluster.",
                   ParameterSetName = AzureHdInsightPowerShellConstants.ParameterSetJobHistoryByNameWithSpecificSubscriptionCredentials)]
        public string CloudServiceName
        {
            get { return this.command.CloudServiceName; }
            set { this.command.CloudServiceName = value; }
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            try
            {
                this.command.Logger = this.Logger;
                var task = this.command.EndProcessing();
                var token = this.command.CancellationToken;
                while (!task.IsCompleted)
                {
                    this.WriteDebugLog();
                    task.Wait(1000, token);
                }
                if (task.IsFaulted)
                {
                    throw new AggregateException(task.Exception);
                }
                foreach (var output in this.command.Output)
                {
                    this.WriteObject(output);
                }
                this.WriteDebugLog();
            }
            catch (AggregateException ex)
            {
                this.WriteObject(this.FormatException(ex));
                this.Logger.Log(Severity.Error, Verbosity.Normal, this.FormatException(ex));
                throw ex.InnerException;
            }
            this.WriteDebugLog();
        }
    }
}
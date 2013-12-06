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
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    /// <summary>
    ///     Sets the Default Storage Container for the HDInsight cluster configuration.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, AzureHdInsightPowerShellConstants.AzureHDInsightConfigValues)]
    public class AddAzureHDInsightConfigValuesCmdlet : AzureHDInsightCmdlet, IAddAzureHDInsightConfigValuesBase
    {
        private readonly IAddAzureHDInsightConfigValuesCommand command;

        /// <summary>
        ///     Initializes a new instance of the AddAzureHDInsightConfigValuesCmdlet class.
        /// </summary>
        public AddAzureHDInsightConfigValuesCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateAddConfig();
        }

        /// <summary>
        ///     Gets or sets the Azure HDInsight Configuration for the Azure HDInsight cluster being constructed.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true,
            HelpMessage = "The HDInsight cluster configuration to use when creating the new cluster (created by New-AzureHDInsightConfig).",
            ValueFromPipeline = true, ParameterSetName = AzureHdInsightPowerShellConstants.ParameterSetDefaultStorageAccount)]
        public AzureHDInsightConfig Config
        {
            get { return this.command.Config; }
            set
            {
                if (value.IsNull())
                {
                    throw new ArgumentNullException("value", "The value for the configuration can not be null.");
                }

                this.command.Config.ClusterSizeInNodes = value.ClusterSizeInNodes;
                this.command.Config.DefaultStorageAccount = value.DefaultStorageAccount;
                this.command.Config.AdditionalStorageAccounts.AddRange(value.AdditionalStorageAccounts);
                this.command.Config.HiveMetastore = value.HiveMetastore ?? this.command.Config.HiveMetastore;
                this.command.Config.OozieMetastore = value.OozieMetastore ?? this.command.Config.OozieMetastore;
            }
        }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Core Hadoop service.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Ease of use in Powershell")]
        [Parameter(Mandatory = false, HelpMessage = "a collection of configuration properties to customize the Core Hadoop service.")]
        public Hashtable Core
        {
            get { return this.command.Core; }
            set { this.command.Core = value; }
        }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Hdfs Hadoop service.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Ease of use in Powershell")]
        [Parameter(Mandatory = false, HelpMessage = "a collection of configuration properties to customize Hdfs Core Hadoop service.")]
        public Hashtable Hdfs
        {
            get { return this.command.Hdfs; }
            set { this.command.Hdfs = value; }
        }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Hive Hadoop service.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Ease of use in Powershell")]
        [Parameter(Mandatory = false, HelpMessage = "a collection of configuration properties to customize the Hive Hadoop service.")]
        public AzureHDInsightHiveConfiguration Hive
        {
            get { return this.command.Hive; }
            set { this.command.Hive = value; }
        }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the MapReduce Hadoop service.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Ease of use in Powershell")]
        [Parameter(Mandatory = false, HelpMessage = "a collection of configuration properties to customize the MapReduce Hadoop service.")]
        public AzureHDInsightMapReduceConfiguration MapReduce
        {
            get { return this.command.MapReduce; }
            set { this.command.MapReduce = value; }
        }

        /// <summary>
        ///     Gets or sets a collection of configuration properties to customize the Oozie Hadoop service.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Ease of use in Powershell")]
        [Parameter(Mandatory = false, HelpMessage = "a collection of configuration properties to customize the Oozie Hadoop service.")]
        public AzureHDInsightOozieConfiguration Oozie
        {
            get { return this.command.Oozie; }
            set { this.command.Oozie = value; }
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            try
            {
                this.command.EndProcessing().Wait();
                foreach (AzureHDInsightConfig output in this.command.Output)
                {
                    this.WriteObject(output);
                }
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

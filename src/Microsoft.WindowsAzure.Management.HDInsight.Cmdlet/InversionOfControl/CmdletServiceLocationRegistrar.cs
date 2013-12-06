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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet
{
    using System;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    /// <summary>
    ///     Registers Cmdlet services with the IoC system.
    /// </summary>
    internal class CmdletServiceLocationRegistrar : IServiceLocationRegistrar
    {
        /// <inheritdoc />
        public void Register(IServiceLocationRuntimeManager manager, IServiceLocator locator)
        {
            if (manager.IsNull())
            {
                throw new ArgumentNullException("manager");
            }

            manager.RegisterType<IAzureHDInsightCommandFactory, AzureHDInsightCommandFactory>();
            manager.RegisterType<IAzureHDInsightSubscriptionsFileManagerFactory, AzureHDInsightSubscriptionsFileManagerFactory>();
            manager.RegisterType<IAzureHDInsightSubscriptionsManagerFactory, AzureHDInsightSubscriptionsManagerFactory>();
            manager.RegisterType<IAzureHDInsightConnectionSessionManagerFactory, AzureHDInsightConnectionSessionManagerFactory>();
            manager.RegisterType<IBufferingLogWriterFactory, PowershellLogWriterFactory>();
            manager.RegisterType<IAzureHDInsightStorageHandlerFactory, AzureHDInsightStorageHandlerFactory>();
            manager.RegisterType<IAzureHDInsightClusterManagementClientFactory, AzureHDInsightClusterManagementClientFactory>();
            manager.RegisterType<IAzureHDInsightJobSubmissionClientFactory, AzureHDInsightJobSubmissionClientFactory>();
        }
    }
}

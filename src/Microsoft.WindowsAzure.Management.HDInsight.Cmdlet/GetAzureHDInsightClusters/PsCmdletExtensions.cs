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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;

    internal static class PsCmdletExtensions
    {
        public static AzureHDInsightClusterConnection AssertValidConnection(this PSCmdlet cmdlet)
        {
            var sessionManager = ServiceLocator.Instance.Locate<IAzureHDInsightConnectionSessionManagerFactory>().Create(cmdlet.SessionState);
            var currentConnection = sessionManager.GetCurrentCluster();
            if (currentConnection == null)
            {
                cmdlet.ThrowTerminatingError(
                    new ErrorRecord(
                        new NotSupportedException("Please connect to a valid Azure HDInsight cluster before calling this cmdlet."),
                        "1024",
                        ErrorCategory.ConnectionError,
                        cmdlet));
            }

            return currentConnection;
        }
    }
}

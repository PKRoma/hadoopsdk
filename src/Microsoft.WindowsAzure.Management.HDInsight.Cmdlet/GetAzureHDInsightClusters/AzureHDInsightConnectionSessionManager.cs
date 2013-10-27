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
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;

    internal class AzureHDInsightConnectionSessionManager : IAzureHDInsightConnectionSessionManager
    {
        private const string CurrentClusterVariableName = "_hdinsightCurrentCluster";

        private readonly SessionState sessionState;

        public AzureHDInsightConnectionSessionManager(SessionState sessionState)
        {
            this.sessionState = sessionState;
        }

        public void SetCurrentCluster(AzureHDInsightClusterConnection cluster)
        {
            this.sessionState.PSVariable.Set(CurrentClusterVariableName, cluster);
        }

        public AzureHDInsightClusterConnection GetCurrentCluster()
        {
            var currentClusterVariable = this.sessionState.PSVariable.Get(CurrentClusterVariableName);
            if (currentClusterVariable == null)
            {
                return null;
            }

            return (AzureHDInsightClusterConnection)currentClusterVariable.Value;
        }
    }
}

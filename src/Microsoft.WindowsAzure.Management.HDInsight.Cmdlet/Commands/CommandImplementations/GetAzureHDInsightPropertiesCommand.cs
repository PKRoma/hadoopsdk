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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;

    internal class GetAzureHDInsightPropertiesCommand : AzureHDInsightClusterCommand<AzureHDInsightCapabilities>, IGetAzureHDInsightPropertiesCommand
    {
        private const string ContainersCountKey = "CONTAINERS_Count";
        private const string CoresUsedKey = "CONTAINERS_CoresUsed";
        private const string MaxCoresAllowedKey = "CONTAINERS_MaxCoresAllowed";

        public override async Task EndProcessing()
        {
            IHDInsightClient client = this.GetClient();
            var capabilities = await client.ListResourceProviderPropertiesAsync();
            capabilities = capabilities.ToList();
            var azureCapabilities = new AzureHDInsightCapabilities(capabilities);
            azureCapabilities.Versions = await client.ListAvailableVersionsAsync();
            azureCapabilities.Locations = await client.ListAvailableLocationsAsync();
            azureCapabilities.ClusterCount = this.GetIntCapability(capabilities, ContainersCountKey);
            azureCapabilities.CoresUsed = this.GetIntCapability(capabilities, CoresUsedKey);
            azureCapabilities.MaxCoresAllowed = this.GetIntCapability(capabilities, MaxCoresAllowedKey);
            this.Output.Add(azureCapabilities);
        }

        private int GetIntCapability(IEnumerable<KeyValuePair<string, string>> capabilities, string capabilityName)
        {
            int capabilityValue = 0;
            KeyValuePair<string, string> capablity = capabilities.FirstOrDefault(cap => cap.Key == capabilityName);
            if (int.TryParse(capablity.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out capabilityValue))
            {
                return capabilityValue;
            }

            return 0;
        }
    }
}

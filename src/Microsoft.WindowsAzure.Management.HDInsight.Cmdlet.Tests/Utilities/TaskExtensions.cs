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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Tests.Utilities
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;

    internal static class TaskEx2
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Required for functionality.")]
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            var task = new Task<TResult>(() => result);
            task.RunSynchronously();
            return task;
        }

        public static IEnumerable<TEntity> ToEnumerable<TEntity>(this ICollection<PSObject> powerShellObjects) where TEntity : class
        {
            powerShellObjects.ArgumentNotNull("psObjects");
            var enumerableEntities = new List<TEntity>();
            foreach (PSObject psObject in powerShellObjects)
            {
                var entity = psObject.ImmediateBaseObject as TEntity;
                if (entity != null)
                {
                    enumerableEntities.Add(entity);
                }
            }

            return enumerableEntities;
        }
    }
}

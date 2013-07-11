//  Copyright (c) Microsoft Corporation
//  All rights reserved.
// 
//  Licensed under the Apache License, Version 2.0 (the "License"); you may not
//  use this file except in compliance with the License.  You may obtain a copy
//  of the License at http://www.apache.org/licenses/LICENSE-2.0   
// 
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
//  WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
//  MERCHANTABLITY OR NON-INFRINGEMENT.  
// 
//  See the Apache Version 2.0 License for specific language governing 
//  permissions and limitations under the License. 


using IQToolkit.Data.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using IQToolkit;

namespace Microsoft.Hadoop.Hive
{
    /// <summary>
    /// returns the list of file resources in all map/reduce expressions accessible from the source expression
    /// </summary>
    public class AddFileGatherer : DbExpressionVisitor
    {
        List<string> fileResources = new List<string>();

        public static ReadOnlyCollection<string> Gather(Expression expression)
        {
            var gatherer = new AddFileGatherer();
            gatherer.Visit(expression);

            var initialList = gatherer.fileResources.Distinct().ToList();
            var finalList = initialList.SelectMany(f =>
            {
                return
                    new DirectoryInfo(Path.GetDirectoryName(f)).GetFiles("*.dll", SearchOption.AllDirectories).Union(
                    new DirectoryInfo(Path.GetDirectoryName(f)).GetFiles("*.exe", SearchOption.AllDirectories))
                    .Select(fi => fi.FullName);
            })
            .Distinct();

            return finalList.ToReadOnly();
        }

        protected override MapExpression VisitMap(MapExpression map)
        {
            if (map == null)
                return null;

            this.fileResources.Add(map.DriverPath);
            this.fileResources.Add(Path.GetDirectoryName(map.DriverPath).Replace('\\', '/') + "/Microsoft.Hadoop.MapReduce.dll");
            this.fileResources.Add(map.AssemblyFullPath);
            return map;
        }
    }
}



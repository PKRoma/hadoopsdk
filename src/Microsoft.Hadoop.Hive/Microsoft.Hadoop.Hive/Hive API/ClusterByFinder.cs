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
    /// Finds first ClusterBy operator in the expression tree
    /// </summary>
    public class ClusterByFinder : DbExpressionVisitor
    {
        SelectExpression firstSelectWithClusterBy = null;

        public static ReadOnlyCollection<string> FindClusterByKeyColumns(Expression expression)
        {
            var finder = new ClusterByFinder();
            finder.Visit(expression);

            if (finder.firstSelectWithClusterBy != null)
            {
                return finder.firstSelectWithClusterBy.ClusterBy.Cast<ColumnExpression>().Select(item => item.Name).ToReadOnly();
            }

            throw new Exception("ClusterBy statement not found before Reduce.");
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            Expression result = base.VisitSelect(select);

            if (select != null && firstSelectWithClusterBy == null && select.ClusterBy != null && select.ClusterBy.Count != 0)
            {
                firstSelectWithClusterBy = select;
            }
            return result;
        }
    }
}



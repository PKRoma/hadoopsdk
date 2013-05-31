using IQToolkit.Data.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
            return gatherer.fileResources.AsReadOnly();
        }

        protected override MapExpression VisitMap(MapExpression map)
        {
            if (map == null)
                return null;

            this.fileResources.Add(map.DriverPath);
            this.fileResources.Add(Path.GetDirectoryName(map.DriverPath).Replace('\\', '/') + "/Microsoft.Hadoop.MapReduce.dll");
            return map;
        }
    }
}

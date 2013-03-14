using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Hadoop.Hive
{
    internal class HiveMaterializer
    {
        public static IEnumerable<T> Materialize<T>(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            return (from DataRow row in table.Rows select CreateObject<T>(row)).ToList();
        }

        internal static T CreateObject<T>(DataRow row)
        {
            var target = default(T);
            if (row == null)
            {
                return target;
            }

            target = Activator.CreateInstance<T>();

            foreach (DataColumn col in row.Table.Columns)
            {
                var prop = target.GetType().GetProperty(col.ColumnName);
                if (prop == null)
                {
                    throw new MissingMemberException(string.Format(@"Property '{0}' is not defined for this object",col.ColumnName));
                }

                try
                {
                    var value = row[col.ColumnName];
                    prop.SetValue(target, value, null);
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("Object could not be materialized from hive results.");
                }
            }

            return target;
        }
    }
}

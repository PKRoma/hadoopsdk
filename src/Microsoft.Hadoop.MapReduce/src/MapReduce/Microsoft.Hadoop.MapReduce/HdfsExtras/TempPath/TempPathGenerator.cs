using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.HdfsExtras
{
    public class TempPathGenerator : ITempPathGenerator
    {
        public string GetTempPath()
        {
            return System.IO.Path.GetTempFileName();
        }
    }
}

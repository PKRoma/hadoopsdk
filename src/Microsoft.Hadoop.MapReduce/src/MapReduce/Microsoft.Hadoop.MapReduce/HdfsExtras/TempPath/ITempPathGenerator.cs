using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.HdfsExtras
{
    public interface ITempPathGenerator
    {
        string GetTempPath();
    }
}

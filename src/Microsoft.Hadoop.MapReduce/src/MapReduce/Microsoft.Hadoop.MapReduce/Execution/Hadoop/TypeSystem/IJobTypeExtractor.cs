using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    public interface IJobTypeExtractor
    {
        void ExtractTypes(Type jobType, out Type mapperType, out Type combinerType, out Type reducerType);
    }
}

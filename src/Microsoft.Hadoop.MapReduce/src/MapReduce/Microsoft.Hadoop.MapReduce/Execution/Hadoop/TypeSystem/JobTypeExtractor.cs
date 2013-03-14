using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using System.Globalization;

    internal class JobTypeExtractor : IJobTypeExtractor
    {
        public void ExtractTypes(Type jobType, out Type mapperType, out Type combinerType, out Type reducerType)
        {
            mapperType = null;
            combinerType = null;
            reducerType = null;

            Type jobTypeBase = jobType;
            while (!jobTypeBase.Name.StartsWith("HadoopJob`"))
            {
                jobTypeBase = jobTypeBase.BaseType;

                if (jobTypeBase == null)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture,
                                               "JobType should derive from HadoopJob<>,HadoopJob<,> or HadoopJob<,,>");
                    throw new StreamingException(msg);
                }
            }

            mapperType = jobTypeBase.GetGenericArguments()[0];

            if (jobTypeBase.GetGenericTypeDefinition() == typeof(HadoopJob<,>))
            {
                reducerType = jobTypeBase.GetGenericArguments()[1];
            }

            if (jobTypeBase.GetGenericTypeDefinition() == typeof(HadoopJob<,,>))
            {
                combinerType = jobTypeBase.GetGenericArguments()[1];
                reducerType = jobTypeBase.GetGenericArguments()[2];
            }
        }
    }
}

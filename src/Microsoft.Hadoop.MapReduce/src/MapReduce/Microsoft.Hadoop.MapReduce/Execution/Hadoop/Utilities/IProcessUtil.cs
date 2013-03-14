using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using System.Threading.Tasks;

    public interface IProcessUtil
    {
        Task<int> RunAndThrowOnError(IProcessExecutor executor);
        Task<int> RunCommand(IProcessExecutor executor); 
    }
}

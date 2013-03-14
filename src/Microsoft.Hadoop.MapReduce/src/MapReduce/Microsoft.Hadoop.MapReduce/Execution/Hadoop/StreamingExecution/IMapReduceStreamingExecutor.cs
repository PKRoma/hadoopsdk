using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using Microsoft.Hadoop.WebClient.WebHCatClient;

    public interface IMapReduceStreamingExecutor 
    {
        /// <summary>
        /// Gets or sets a flag indicating that verbose output should be used.
        /// </summary>
        bool Verbose { get; set; }

        /// <summary>
        /// Provides a guid used to store job information.
        /// </summary>
        Guid JobGuid { get; }

        /// <summary>
        /// The inputs to supply to the execution.
        /// </summary>
        ICollection<string> Inputs { get; }

        /// <summary>
        /// The location into which all outputs should be stored.
        /// </summary>
        string OutputLocation { get; set; }
        
        /// <summary>
        /// The Mapper to use during the execution.
        /// </summary>
        string Mapper { get; set; }

        /// <summary>
        /// The Reducer to use during the execution.
        /// </summary>
        string Reducer { get; set; }

        /// <summary>
        /// The Combiner to use during the execution.
        /// </summary>
        string Combiner { get; set; }

        /// <summary>
        /// Files to include with the execution.
        /// </summary>
        ICollection<string> File { get; }

        /// <summary>
        /// Hadoop Property Defines to include with the execution.
        /// </summary>
        IDictionary<string, string> Defines { get; }

        /// <summary>
        /// Command Environment Variables to define with the execution.
        /// </summary>
        IDictionary<string, string> CmdEnv { get; }

        /// <summary>
        /// Additional Hadoop streaming command line args to add to the end of the 
        /// execution command line string.
        /// </summary>
        ICollection<string> Args { get; }

        /// <summary>
        /// Execute the request.
        /// </summary>
        /// <param name="throwOnError">Throw if there is an error.</param>
        /// <returns>
        /// The result of the execution as a <see cref="MapReduceResult"/> object.
        /// </returns>
        MapReduceResult Execute(bool throwOnError);
    }
}

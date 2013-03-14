// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public enum StandardLineType
    {
        StdErr,
        StdOut
    }

    /// <summary>
    /// A delegate to be used to process output lines coming from the command execution.
    /// </summary>
    /// <param name="lineType">
    /// The type of line to process.
    /// </param>
    /// <param name="output">
    /// The line to process.
    /// </param>
    public delegate void ProcessOutputLine(StandardLineType lineType, string output);

    /// <summary>
    ///     Provides an abstraction to process execution so that various components can
    ///     be placed under unitized testing.
    /// </summary>
    public interface IProcessExecutor
    {
        /// <summary>
        ///     Represents a cached result of the Standard Output, so long
        ///     as CacheOutputs is true.
        /// </summary>
        IEnumerable<string> StandardOut { get; }

        /// <summary>
        ///     Represents a cached result of the Standard Error, so long
        ///     as ChacheOutputs is true.
        /// </summary>
        IEnumerable<string> StandardError { get; }

        /// <summary>
        ///     Represents the arguments to be supplied to the command line.
        /// </summary>
        ICollection<string> Arguments { get; }

        /// <summary>
        ///     Represents the environment variables to be used when creating the process.
        /// </summary>
        IDictionary<string, string> EnvironemntVariables { get; }

        /// <summary>
        /// </summary>
        bool WriteOutputAndErrorToConsole { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if standard output and error lines should
        /// be cached in the <see cref="StandardError"/> or <see cref="StandardOut"/> properties.
        /// </summary>
        bool CacheOutputs { get; set; }

        /// <summary>
        /// Creates the command line argument string as it will be formatted for <see cref="ProcessStartInfo.Arguments"/>.
        /// </summary>
        /// <returns>
        /// A command line argument string.
        /// </returns>
        string CreateArgumentString();

        /// <summary>
        /// Gets or sets a delegate function to be used to process output.
        /// </summary>
        ProcessOutputLine ProcessOutput { get; set; }

        /// <summary>
        /// The command to be executed.  This may include the full path to the command.
        /// </summary>
        string Command { get; set; }

        /// <summary>
        /// The exit code returned from process execution.
        /// </summary>
        int ExitCode { get; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>
        /// A command that represents the execution.
        /// </returns>
        Task<int> Execute();
    }
}

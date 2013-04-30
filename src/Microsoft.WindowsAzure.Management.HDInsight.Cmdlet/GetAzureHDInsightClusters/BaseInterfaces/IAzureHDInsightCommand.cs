namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents an underlying command for an AzureHDInsight Cmdlet.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values that would be returned by the Cmdlet.
    /// </typeparam>
    internal interface IAzureHDInsightCommand<T> : IAzureHDInsightCommandBase
    {
        ICollection<T> Output { get; }
    }
}
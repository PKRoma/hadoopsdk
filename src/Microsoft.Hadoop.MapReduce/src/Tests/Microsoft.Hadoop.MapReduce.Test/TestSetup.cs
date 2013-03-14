using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Test
{
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.HdInsight.CombineDriver;
    using Microsoft.HdInsight.MRRunner;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public static class TestSetup
    {
        internal static LocalHadoopConstructor makeLocalHadoop;
        internal static OneBoxHadoopConstructor makeOneBoxHadoop;
        internal static AzureHadoopConstructor makeAzureHadoop;

        internal static IHdfsFile OriginalHdfsFile;

        private static List<Type> types;
            
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            makeLocalHadoop = Hadoop.makeLocal;
            makeOneBoxHadoop = Hadoop.makeOneBox;
            makeAzureHadoop = Hadoop.makeAzure;
            OriginalHdfsFile = HdfsFile.InternalHdfsFile;

            types = new List<Type>();
            types.Add(typeof(HdInsight.MapDriver.Program));
            types.Add(typeof(HdInsigt.ReduceDriver.Program));
            types.Add(typeof(Runner));
            types.Add(typeof(Program));
        }
    }
}

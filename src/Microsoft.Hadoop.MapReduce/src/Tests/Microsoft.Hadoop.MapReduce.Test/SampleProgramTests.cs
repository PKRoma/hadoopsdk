using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Hadoop.MapReduce.Test
{
    using Microsoft.Hadoop.MapReduce.HadoopImplementations;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.WebClient.WebHCatClient;
    using Microsoft.Hadoop.WebHDFS;
    using Microsoft.Hadoop.WebHDFS.Adapters;
    using WordCountSampleApplication;

    [TestClass]
    public class SampleProgramTests : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        public void IntegrationRun_WordCount_SampleProgramLocal()
        {
            PrepareForLocalRun();

            Driver.Main(new string[0]);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        public void IntegrationRunParameterSweepSampleProgramLocal()
        {
            PrepareForLocalRun();

            ParameterSweep.Program.Main(new string[0]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void IntegrationRun_WordCount_SampleProgramWebHcat()
        {
            PrepareForWebRun();

            Driver.Main(new string[0]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ClusterRun_WordCount_SampleProgramWebHcat()
        {
            PrepareForClusterRun("default");

            Driver.Main(new string[0]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void IntengrationRun_ParameterSweep_SampleProgramWebHcat()
        {
            PrepareForWebRun();

            ParameterSweep.Program.Main(new string[0]);
        }
    }
}

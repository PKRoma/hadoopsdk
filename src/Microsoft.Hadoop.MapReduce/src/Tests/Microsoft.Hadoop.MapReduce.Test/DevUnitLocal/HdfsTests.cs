using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Microsoft.Hadoop.MapReduce.Test
{
    using System.Linq;
    using System.Text;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.MapReduce.Test.ExecutorTests;

    [TestClass]
    public class HdfsTests
    {
        private static Guid testRun = Guid.NewGuid();
        private IHadoop hadoop = Hadoop.Connect();

        public string GetAnHdfsPath()
        {
            return string.Format("/test/{0}/{1}", testRun, Guid.NewGuid());
        }

        private string hdfsFolderPath = "test/unittest/HdfsFileTests";
        private string hdfsFilePath = "test/unittest/HdfsFileTests/file0.txt";
        private void EnsureTestFileExists()
        {
            if (!hadoop.StorageSystem.Exists(hdfsFilePath))
            {
                hadoop.StorageSystem.WriteAllLines(hdfsFilePath, new[] { "hello", "world" });
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Exists()
        {
            EnsureTestFileExists();
            Assert.IsTrue(hadoop.StorageSystem.Exists(hdfsFilePath));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Ls()
        {
            var hdfsFolder = this.GetAnHdfsPath();
            IHdfsFile hdfsFile = Hadoop.Connect().StorageSystem;
            hdfsFile.WriteAllText(hdfsFolder + "/one.txt", "one");
            hdfsFile.WriteAllText(hdfsFolder + "/two.txt", "two");
            hdfsFile.WriteAllText(hdfsFolder + "/three.txt", "three");
            var data = hdfsFile.LsFiles(hdfsFolder);
            Assert.IsTrue(data.Contains(hdfsFolder + "/one.txt"));
            Assert.IsTrue(data.Contains(hdfsFolder + "/two.txt"));
            Assert.IsTrue(data.Contains(hdfsFolder + "/three.txt"));
            hadoop.StorageSystem.Delete(hdfsFolder);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void IntegrationTestEnumerateDataInFolder()
        {
            var hdfsFolder = this.GetAnHdfsPath();
            IHdfsFile hdfsFile = Hadoop.Connect().StorageSystem;
            hdfsFile.WriteAllText(hdfsFolder + "/one.txt", "one");
            hdfsFile.WriteAllText(hdfsFolder + "/two.txt", "two");
            hdfsFile.WriteAllText(hdfsFolder + "/three.txt", "three");
            var data = hdfsFile.EnumerateDataInFolder(hdfsFolder).ToArray();
            hdfsFile.Delete(hdfsFolder);
            Assert.IsTrue(data.Contains("one"));
            Assert.IsTrue(data.Contains("two"));
            Assert.IsTrue(data.Contains("three"));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EnumerateDataInFolder()
        {
            EnsureTestFileExists();
            foreach (string line in hadoop.StorageSystem.EnumerateDataInFolder(hdfsFolderPath))
            {
                Console.WriteLine(line);
            }
        }

        [TestMethod]
        public void CopyToLocal()
        {
            string tempPath = Path.GetTempFileName();
            hadoop.StorageSystem.CopyToLocal("/user/mikelid/input/Textsort/input0.bin", tempPath);
            Assert.IsTrue(File.Exists(tempPath));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void ReadWriteAllLinesIntegrationTest()
        {
            string path = this.GetAnHdfsPath();
            string[] writen = { "one", "two", "three" };
            hadoop.StorageSystem.WriteAllLines(path, writen);
            string[] read = hadoop.StorageSystem.ReadAllLines(path);
            Assert.IsTrue(read.SequenceEqual(writen));
            hadoop.StorageSystem.Delete(path);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void ReadWriteAllTextIntegrationTest()
        {
            string path = this.GetAnHdfsPath();
            string writen = "one\r\ntwo\tthree";
            hadoop.StorageSystem.WriteAllText(path, writen);
            string read = hadoop.StorageSystem.ReadAllText(path);
            Assert.AreEqual(writen, read);
            hadoop.StorageSystem.Delete(path);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void ReadWriteAllBytesIntegrationTest()
        {
            string path = this.GetAnHdfsPath();
            byte[] writen = Encoding.UTF8.GetBytes("one\r\ntwo\tthree");
            hadoop.StorageSystem.WriteAllBytes(path, writen);
            byte[] read = hadoop.StorageSystem.ReadAllBytes(path);
            Assert.IsTrue(writen.SequenceEqual(read));
            hadoop.StorageSystem.Delete(path);
        }

        public void AssertFsTransferCommand(string fsCommand, string source, string destination, MockProcessExecutor executor)
        {
            Assert.AreEqual("fs", executor.Arguments.ElementAt(0));
            Assert.AreEqual(fsCommand, executor.Arguments.ElementAt(1));
            Assert.AreEqual(source, executor.Arguments.ElementAt(2));
            Assert.AreEqual(destination, executor.Arguments.ElementAt(3));
        }

        private class MockGenerator : ITempPathGenerator
        {
            private readonly string path = System.IO.Path.GetTempFileName();

            public string GetTempPath()
            {
                return this.path;
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void WriteAllLinesCallsMoveFromLocalWithTempFile()
        {
            var gen = new MockGenerator();
            var executor = new MockProcessExecutor();
            IHdfsFile hdfsFile = new LocalHdfsFile(() => executor, gen);
            string[] lines = { "one", "two", "three" };
            string hdfsPath = this.GetAnHdfsPath();
            hdfsFile.WriteAllLines(hdfsPath, lines);
            executor.WaitForExecution();
            this.AssertFsTransferCommand(FsCommands.MoveFromLocal, gen.GetTempPath(), hdfsPath, executor);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void WriteAllTextCallsMoveFromLocalWithTempFile()
        {
            var gen = new MockGenerator();
            var executor = new MockProcessExecutor();
            IHdfsFile hdfsFile = new LocalHdfsFile(() => executor, gen);
            string text = "one\ttwo\r\nthree";
            string hdfsPath = this.GetAnHdfsPath();
            hdfsFile.WriteAllText(hdfsPath, text);
            executor.WaitForExecution();
            this.AssertFsTransferCommand(FsCommands.MoveFromLocal, gen.GetTempPath(), hdfsPath, executor);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void WriteAllBytesCallsMoveFromLocalWithTempFile()
        {
            var gen = new MockGenerator();
            var executor = new MockProcessExecutor();
            IHdfsFile hdfsFile = new LocalHdfsFile(() => executor, gen);
            byte[] writen = Encoding.UTF8.GetBytes("one\r\ntwo\tthree");
            string hdfsPath = this.GetAnHdfsPath();
            hdfsFile.WriteAllBytes(hdfsPath, writen);
            executor.WaitForExecution();
            this.AssertFsTransferCommand(FsCommands.MoveFromLocal, gen.GetTempPath(), hdfsPath, executor);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void ReadAllBytesCallsCopyToLocalWithTempFile()
        {
            var gen = new MockGenerator();
            var executor = new MockProcessExecutor();
            string hdfsPath = this.GetAnHdfsPath();
            IHdfsFile hdfsFile = new LocalHdfsFile(() => executor, gen);
            byte[] read = hdfsFile.ReadAllBytes(hdfsPath);
            executor.WaitForExecution();
            this.AssertFsTransferCommand(FsCommands.CopyToLocal, hdfsPath, gen.GetTempPath(), executor);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void ReadAllLinesCallsCopyToLocalWithTempFile()
        {
            var gen = new MockGenerator();
            var executor = new MockProcessExecutor();
            string hdfsPath = this.GetAnHdfsPath();
            IHdfsFile hdfsFile = new LocalHdfsFile(() => executor, gen);
            string[] lines = hdfsFile.ReadAllLines(hdfsPath);
            executor.WaitForExecution();
            this.AssertFsTransferCommand(FsCommands.CopyToLocal, hdfsPath, gen.GetTempPath(), executor);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void ReadAllTextCallsCopyToLocalWithTempFile()
        {
            var gen = new MockGenerator();
            var executor = new MockProcessExecutor();
            string hdfsPath = this.GetAnHdfsPath();
            IHdfsFile hdfsFile = new LocalHdfsFile(() => executor, gen);
            string text = hdfsFile.ReadAllText(hdfsPath);
            executor.WaitForExecution();
            this.AssertFsTransferCommand(FsCommands.CopyToLocal, hdfsPath, gen.GetTempPath(), executor);
        }
    }
}

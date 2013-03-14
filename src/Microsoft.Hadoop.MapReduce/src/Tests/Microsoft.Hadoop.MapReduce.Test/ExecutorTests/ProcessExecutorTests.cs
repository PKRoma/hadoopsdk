using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Hadoop.MapReduce.Test.ExecutorTests
{
   using System.Diagnostics;
   using System.IO;
   using System.Linq;
   using Microsoft.Hadoop.MapReduce.Execution.Hadoop;

   [TestClass]
   public class ProcessExecutorTests
   {
      // These tests may seem as though they are testing a test class.
      // However because it is derived for the "class under test".  They are 
      // really directed at functionality in the base.
      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanExecuteAJob()
      {
         var executor = new ProcessExecutorTester();
         executor.Command = "foo.exe";
         executor.Arguments.Add("1");
         executor.Arguments.Add("2");
         executor.Arguments.Add("3");

         executor.Execute();

         executor.ProcessResult();

         Assert.AreEqual(0, executor.ExitCode);

         Assert.AreEqual(3, executor.ProcessDetails.Arguments.Count());
      }

      private void ExecuteCommandWithArgsAndValidate(params string[] args)
      {
         var executor = new ProcessExecutorTester();
         executor.Command = "foo.exe";
         foreach (var arg in args)
         {
            executor.Arguments.Add(arg);
         }
         executor.Execute();
         executor.ProcessResult();
         executor.AssertArgsAndVariables();
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanHandleArgumentsWithSpaces()
      {
         this.ExecuteCommandWithArgsAndValidate(@"C:\My File.txt");
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanHandleArgumentsWithCr()
      {
         this.ExecuteCommandWithArgsAndValidate(@"C:\My\r""File.txt");
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanHandleArgumentsWithLf()
      {
         this.ExecuteCommandWithArgsAndValidate(@"C:\My\nFile.txt");
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanHandleArgumentsWithCrLf()
      {
         this.ExecuteCommandWithArgsAndValidate(@"C:\My\r\nFile.txt");
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanHandleArgumentsWithTabs()
      {
         this.ExecuteCommandWithArgsAndValidate("C:\\My\tFile.txt");
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanHandleArgumentsWithQuotes()
      {
         this.ExecuteCommandWithArgsAndValidate("C:\\My\"File.txt");
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanHandleArgumentsWithPipes()
      {
         this.ExecuteCommandWithArgsAndValidate("C:\\My|File.txt");
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanChangeEnvironmentVariables()
      {
         var executor = new ProcessExecutorTester();
         executor.Command = "foo.exe";
         executor.EnvironemntVariables["Path"] = "foo";
         executor.Execute();
         executor.ProcessResult();
         executor.AssertArgsAndVariables();
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanAddEnvironmentVariables()
      {
         var executor = new ProcessExecutorTester();
         executor.Command = "foo.exe";
         executor.EnvironemntVariables.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
         executor.Execute();
         executor.ProcessResult();
         executor.AssertArgsAndVariables();
      }

      [TestMethod]
      [TestCategory("CheckIn")]
      public void ProcessExecutorCanRemoveEnvironmentVariables()
      {
         var executor = new ProcessExecutorTester();
         executor.Command = "foo.exe";
         executor.EnvironemntVariables.Remove(executor.EnvironemntVariables.Keys.First(str => string.Equals(str, "path", StringComparison.OrdinalIgnoreCase)));
         executor.Execute();
         executor.ProcessResult();
         executor.AssertArgsAndVariables();
         Assert.IsFalse(executor.EnvironemntVariables.Keys.Any(str => string.Equals(str, "path", StringComparison.OrdinalIgnoreCase)));
      }
   }
}

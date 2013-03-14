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

namespace Microsoft.Hadoop.MapReduce
{
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Linq;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Microsoft.Hadoop.WebHCat.Protocol;
   using Microsoft.Hadoop.WebHDFS;
   using Microsoft.Hadoop.WebHDFS.Adapters;
   using Newtonsoft.Json.Linq;

   /// <summary>
   ///    Executes Hadoop jobs on Azure Hadoop clusters.
   /// </summary>
   public static class WebHCatJobExecutor
   {
      /// <summary>
      ///    Executes the specified job.
      /// </summary>
      /// <typeparam name="TJobType">The job type.</typeparam>
      public static void ExecuteJob<TJobType>(string clusterName, 
                                             string userName, 
                                             string password, 
                                             string storageAccount, 
                                             string storageAccountKey, 
                                             string asvContainer) 
                                       where TJobType : HadoopJob, new()
      {
          var whchc = new WebHCatHttpClient(new Uri(clusterName), userName, password);
          
          //todo: Add combiner to underlying API 
         /*
          * curl -s 
          *      -d user.name=hdinsightuser 
          *      -d cmdenv="MSFT_HADOOP_MAPPER_DLL=ConsoleApplication1.exe"  
          *      -d cmdenv="MSFT_HADOOP_COMBINER_DLL=ConsoleApplication1.exe" 
          *      -d cmdenv="MSFT_HADOOP_COMBINER_TYPE=WordCount.WordCountReducer"  
          *      -d cmdenv="MSFT_HADOOP_MAPPER_TYPE=WordCount.WordCountMapper" 
          *      -d cmdenv="MSFT_HADOOP_REDUCER_DLL=ConsoleApplication1.exe"  
          *      -d cmdenv="MSFT_HADOOP_REDUCER_TYPE=WordCount.WordCountReducer" 
          *      -d input=mydata -d output=mycounts18 
          *      -d mapper=Microsoft.Hadoop.MapDriver.exe 
          *      -d reducer=Microsoft.Hadoop.ReduceDriver.exe 
          *      -d combiner=Microsoft.Hadoop.CombineDriver.exe 
          *      -d files=asv:///in/ConsoleApplication1.exe,
          *               asv:///in/Microsoft.Hadoop.CombineDriver.exe,
          *               asv:///in/Microsoft.Hadoop.MapDriver.exe,
          *               asv:///in/Microsoft.Hadoop.MapReduce.dll,
          *               asv:///in/Microsoft.Hadoop.ReduceDriver.exe,
          *               asv:///in/MRRunner.exe,
          *               asv:///in/Newtonsoft.Json.dll 
          */

         var job = new TJobType();
         var context = new ExecutorContext();
         HadoopJobConfiguration config = job.Configure(context);
         Type mapperType, reducerType, combinerType;
         HadoopJobExecutor_Old.ExtractTypes<TJobType>(out mapperType, out combinerType, out reducerType);
         ExecutorUtils.CheckUserTypes(mapperType, combinerType, reducerType);

         var cmdenvs = new Dictionary<string, string>();
         cmdenvs[EnvironmentUtils.EnvVarName_User_MapperDLLName] = Path.GetFileName(mapperType.Assembly.Location);
         cmdenvs[EnvironmentUtils.EnvVarName_User_MapperTypeName] = mapperType.FullName.Replace(" ", "");
         if (combinerType != null)
         {
            cmdenvs[EnvironmentUtils.EnvVarName_User_CombinerDLLName] = Path.GetFileName(combinerType.Assembly.Location);
            cmdenvs[EnvironmentUtils.EnvVarName_User_CombinerTypeName] = combinerType.FullName.Replace(" ", "");
         }
         if (reducerType != null)
         {
            cmdenvs[EnvironmentUtils.EnvVarName_User_ReducerDLLName] = Path.GetFileName(reducerType.Assembly.Location);
            cmdenvs[EnvironmentUtils.EnvVarName_User_ReducerTypeName] = reducerType.FullName.Replace(" ", "");
         }

         // use for status dir & output directory.. 
         Guid jobGuid = Guid.NewGuid();
         string dirName = "dotnetcli/" + jobGuid.ToString();
         string statusDirectory = dirName + "/status";
         string appDirectory = dirName + "/app";

         var defines = new Dictionary<string, string>();
         var args = new List<string>();

         ApplyCompression(config, defines);
         ApplySortingArguments(config, defines, args);

         var files =
            new List<string>(
               new[] { EnvironmentUtils.PathToMapDriverExe, EnvironmentUtils.PathToReduceDriverExe, EnvironmentUtils.PathToCombineDriverExe }.Concat(
                  EnvironmentUtils.BuildFileIncludeList(config.FilesToInclude, config.FilesToExclude)));

         var blobAdapter = new BlobStorageAdapter(storageAccount, storageAccountKey);
         blobAdapter.Connect(asvContainer, false);
         var fsClient = new WebHDFSClient(blobAdapter);

         files.AsParallel().ForAll(file => { fsClient.CreateFile(file, appDirectory + "/" + Path.GetFileName(file)); });
         IEnumerable<string> remoteFileList = files.Select(file => appDirectory + "/" + Path.GetFileName(file));

         var jobTask = whchc.CreateMapReduceStreamingJob(
            config.InputPath,
            config.OutputFolder,
            EnvironmentUtils.MapDriverExeName,
            EnvironmentUtils.ReduceDriverExeName,
            null,
            defines,
            remoteFileList,
            cmdenvs,
            args,
            statusDirectory,
            "");
         jobTask.ContinueWith(
            jobComplete =>
            jobComplete.Result.Content.ReadAsAsync<JObject>()
                       .ContinueWith(jobJson => whchc.WaitForJobToCompleteAsync(jobJson.Result.Value<string>("id")))
                       .ContinueWith(x => Console.WriteLine("All Done"))
                       .Wait());
      }

      private static void ApplyCompression(HadoopJobConfiguration config, Dictionary<string, string> defines)
      {
         if (config.CompressOutput)
         {
            defines.Add("mapred.output.compress", "true");
            defines.Add("mapred.output.compression.codec", "org.apache.hadoop.io.compress.GzipCodec");
         }
      }

      private static void ApplySortingArguments(HadoopJobConfiguration config, Dictionary<string, string> defines, List<string> args)
      {
         List<string> shuffleSortStreamingArguments;
         Dictionary<string, string> shuffleSortDefines;
         ExecutorUtils.ExtractShuffleSortParameters(config, out shuffleSortStreamingArguments, out shuffleSortDefines);
         args.Concat(shuffleSortStreamingArguments);
         defines.Concat(shuffleSortDefines);
      }
   }
}

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

module WordCount

open System
open System.Net
open Microsoft.Hadoop.MapReduce


type WordCountMapper() =
    inherit MapperBase()

    override self.Map (inputLine, context) = 
        inputLine.Split(' ') |> Array.iter(fun s -> context.EmitKeyValue(s, "1"))
        
type WordCountReducer() =
    inherit ReducerCombinerBase()

    override self.Reduce(key, values, context) =
        let total = values |> Seq.sumBy(fun s -> Int64.Parse(s))
        context.EmitKeyValue(key, total.ToString())

type WordCountFSharp() =
    inherit HadoopJob<WordCountMapper,WordCountReducer>()

    let input = "input/wordcount/test.txt"
    let output = "output/fsharp/wordcount"
    let webInput = "http://www.gutenberg.org/cache/epub/11/pg11.txt"

    override self.Initialize(context) =
        let wc = new WebClient()
        if HdfsFile.Exists(input) = false then
            let data = wc.DownloadString(webInput)
            HdfsFile.WriteAllText(input, data) 
        


    override self.Configure(context) =
        let config = new HadoopJobConfiguration()
        config.InputPath <- input 
        config.OutputFolder <- output
        config



[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    Console.WriteLine("Beginning job")
    HadoopJobExecutor.ExecuteJob<WordCountFSharp>()
    Console.WriteLine("job complete") 
    let x = Console.ReadLine()
    0 // return an integer exit code

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

module FSharpJsonEncodedValues

open System
open System.Collections.Generic
open System.Net
open System.Linq
open Microsoft.Hadoop.MapReduce
open Microsoft.Hadoop.MapReduce.Json
open Newtonsoft.Json
open Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs

// this sample shows how complex types can be leveraged from fsharp
// using TimeZoneInfo as the token type.
// This will be serialized to json using json.net in order to be emitted
// in temporary files, and ultimately will be surfaced up to the programming
// model as a TimeZoneInfo.
// One could do a similar thing with any type that will serialize


// this reducer will emit a key value pair 
// to group the time zone if in daylight savings time, as well
// as the TimeZoneInfo object itself
type CustomTypeMapper() =
    inherit JsonOutMapperBase<TimeZoneInfo>()

    override self.Map (inputLine : string, context: JsonMapperContext<TimeZoneInfo>) = 
        let zone = inputLine.Trim()
        let info = TimeZoneInfo.FindSystemTimeZoneById(zone)
        context.EmitKeyValue(info.IsDaylightSavingTime(DateTime.Now).ToString(), info)
        context.EmitKeyValue(info.IsDaylightSavingTime(DateTime.Now.AddMonths(6)).ToString(), info)

// this reducer will simply concat all of the display names of the timezones handed to it      
type CustomTypeReducer() =
    inherit JsonInReducerCombinerBase<TimeZoneInfo>()

    override self.Reduce(key : string, values : IEnumerable<TimeZoneInfo>, context: ReducerCombinerContext ) =
        let result = values |> Seq.map(fun res -> res.DisplayName) |>   String.concat "\t"
        context.EmitKeyValue(key, result)

type FSharpJsonEncodedValues() =
    inherit HadoopJob<CustomTypeMapper,CustomTypeReducer>()

    let input = "input/fsharpJsonEncodedValues"
    let inputFile = "input/fsharpJsonEncodedValues/input0.txt"
    let output = "output/fsharpJsonEncodedValues"
    

    override self.Initialize(context) =
        if HdfsFile.Exists(input) then HdfsFile.Delete(input)
        HdfsFile.MakeDirectory(input)
        HdfsFile.WriteAllLines(inputFile, ["Eastern Standard Time"; "Pacific Standard Time"])
        


    override self.Configure(context) =
        let config = new HadoopJobConfiguration()
        config.InputPath <- input 
        config.OutputFolder <- output
        config

    override self.Cleanup(context) = 
        HdfsFile.EnumerateDataInFolder(output, 10 )  |> Console.WriteLine

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    Console.WriteLine("Beginning job")
    let hadoop = Hadoop.Connect();
    let results = hadoop.MapReduceJob.ExecuteJob<FSharpJsonEncodedValues>()
    Console.WriteLine("job complete") 
    
    Console.WriteLine("data written")
    let x = Console.ReadLine()
    0 // return an integer exit code


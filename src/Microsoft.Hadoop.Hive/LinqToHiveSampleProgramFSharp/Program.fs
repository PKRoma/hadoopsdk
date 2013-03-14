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

open Microsoft.Hadoop.Hive
open HiveSample
open Microsoft.FSharp
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open System
open System.Linq

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let db = new MyHiveDatabase(new Uri("http://localhost:50111"), "UserName", "Password", "AzureStorageAccount", "AzureStorageKey" )

    Console.WriteLine("Any records for Matt Damon?") 

    let filteredQuery = query {
                            for actor in db.Actors do
                            where (actor.Name = "Matt Damon")
                            select actor
    }
    

    filteredQuery |> Seq.iter(fun actor -> System.Console.WriteLine( "Actor: {0}, MovieId {1}", actor.Name, actor.MovieId)) 
    Console.WriteLine("Done With Damon")
    
    let groupQuery = query { 
                            for actor in db.Actors do
                            groupBy actor.Name into g
                            select (g.Key, g.Average(fun (acts : ActorsRow) -> acts.AwardsCount ))
    }

    let printResult (x,y) = Console.WriteLine("Actor {0}, Average {1}", x,y)
    
    
    // note, this will not ship the sort / take to the server, it will do them 
    // client side.
    groupQuery |> Seq.sortBy (fun result -> -1.0 *   (snd result)) |> Seq.take(10) |>  Seq.iter(fun result -> printResult(result) )

    System.Console.WriteLine("done with grouping")

    let joinQuery = query {
                        for actor in db.Actors do
                        join movie in db.Titles on 
                            (actor.MovieId = movie.MovieId)
                        select (actor.Name, movie.Name, movie.Rating) 
                    }

    
    joinQuery |> Seq.take(20) |> Seq.iter(fun result ->  
                        let (actor,movie,rating) = result
                        System.Console.WriteLine("Actor: {0}, Movie: {1}, Rating: {2}", actor, movie, rating)
                        )

    System.Console.WriteLine("done, press <ENTER> to exit...")
    let done1 = System.Console.ReadLine()
    


    0 // return an integer exit code






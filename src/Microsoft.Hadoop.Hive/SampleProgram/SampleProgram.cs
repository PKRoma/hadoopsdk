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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.Hive;
using System.Data;
using HiveSample;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MyHiveDatabase(new Uri("http://localhost:50111"), "UserName", "Password", "AzureStorageAccount", "AzureStorageKey");

            var q = from x in
                        (from a in db.Actors
                         select new { a.ActorId, foo = a.AwardsCount})
                    group x by x.ActorId into g
                    select new { ActorId = g.Key, bar = g.Average(z => z.foo) };

            var results1 = q.ToList();


            var projectionQuery = from aw in db.Awards
                                  join t in db.Titles
                                      on aw.MovieId equals t.MovieId
                                  where t.Year == 1994 && aw.Won == "True"
                                  select new { MovieId = t.MovieId, Name = t.Name, Type = aw.Type, Category = aw.Category, Year = t.Year };


            var newTable = projectionQuery.CreateTable("AwardsIn1994");  // no rows come to client

            // get the count of awards for each movie in 1994
            var query2 = from x in newTable
                         group x by x.MovieId into g
                         select new { MovieId = g.Key, Count = g.Count() };

            var results2 = query2.ToList();

            newTable.Drop();

            var query3 = from a in db.Actors
                         join t in db.Titles
                             on a.MovieId equals t.MovieId
                         where t.Year == 1994
                         select new { t.Name, a.AwardsCount };

            var newTable2 = query3.CreateTable("MoviesIn1994");

            // get the sum of lifetime award counts for movies in 19994
            var query4 = from x in newTable2
                         group x by x.Name into g
                         select new { Name = g.Key, Sum = g.Sum(p => p.AwardsCount) };

            var results4 = query4.ToList();

            newTable2.Drop();
        }
    }

}

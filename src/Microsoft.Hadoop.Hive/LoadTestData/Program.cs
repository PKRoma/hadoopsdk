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

using Microsoft.Hadoop.Hive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTestData
{
    class TableLoader
    {
        static async void LoadNetflixTables()
        {
            var db = new HiveConnection(new Uri("http://localhost:50111"), "UserName", "Password","AzureStorageAccount", "AzureStorageKey");

           db.GetTable<HiveRow>("Awards").Drop();

           string command = @"CREATE TABLE Awards(MovieId string, AwardId string, Year int, Won string, Type string, Category string) 
                                            row format delimited 
                                            fields terminated by ',';";

           await db.ExecuteHiveQuery(command);

           command = @"LOAD DATA LOCAL INPATH 'C:/code/MovieData/Awards.txt' OVERWRITE INTO TABLE Awards";

           await db.ExecuteHiveQuery(command);

           db.GetTable<HiveRow>("Titles").Drop();

           command = @"CREATE TABLE Titles(MovieId string, Name string, Rating string, Year int) 
                                row format delimited 
                                fields terminated by ',';";

           await db.ExecuteHiveQuery(command);

           command = "LOAD DATA INPATH '/user/hadoop/Moviedata/Titles.txt' OVERWRITE INTO TABLE Titles";

           await db.ExecuteHiveQuery(command);

           db.GetTable<HiveRow>("Actors").Drop();

           command = @"CREATE TABLE Actors(MovieId string, ActorId string, Name string, AwardsCount int) 
                                row format delimited 
                                fields terminated by ',';";

           await db.ExecuteHiveQuery(command);

           command = "LOAD DATA INPATH '/user/hadoop/Moviedata/Actors.txt' OVERWRITE INTO TABLE Actors";

           await db.ExecuteHiveQuery(command);
        }

    }
}

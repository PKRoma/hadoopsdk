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
using Microsoft.Hadoop.MapReduce;


//This project is for code-snippets that go in CHM documentation
//  -> this code is compilable and reference-able, but doesn't necessarily do anything useful.
//
// See : http://www.simple-talk.com/dotnet/.net-tools/taming-sandcastle-a-.net-programmers-guide-to-documenting-your-code/

namespace DocCodeSnippets
{
    public class Snippets1 : MapperBase
    {
        #region Snippet.IMapper.Map
        public override void Map(string inputLine, MapperContext context)
        {
            string key = inputLine.Trim();
            context.EmitKeyValue(key, "1");
        }
        #endregion

        public void InputPartitionId()
        {
            HadoopMapperContext mapperContext = new HadoopMapperContext();
            #region Snippet.MapperContext.InputPartitionId
            string targetFile = mapperContext.InputFilename + "_" +
                                mapperContext.InputPartitionId + ".out.txt";
            #endregion
        }

        public void SettingGenericArg()
        {
            HadoopJobConfiguration config = new HadoopJobConfiguration();
            #region Snippet.StreamingJobConfiguration.SettingGenericArg
            config.AdditionalGenericArguments.Add("-D \"mapred.reduce.tasks.speculative.execution=true\"");
            #endregion
        }
    }
}

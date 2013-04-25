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
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Hadoop.WebClient.AmbariClient.Contracts;
using Microsoft.Hadoop.WebClient.Common;
using Newtonsoft.Json.Linq;

namespace Microsoft.Hadoop.WebClient.AmbariClient
{
    public class AmbariClient
    {
        private const string HeadnodeName = "IsotopeHeadNode_IN_0";
        private const string WorkerNodePrefix = "IsotopeWorkerNode_IN_";

        private readonly AmbariHttpClient client;

        public AmbariClient(Uri baseUri, string username, string password)
        {
            client = new AmbariHttpClient(baseUri, username, password);
        }

        public AmbariClient(Uri baseUri, string username, string password, string hadoopUserName)
        {
            client = new AmbariHttpClient(baseUri, username, password, hadoopUserName);
        }

        public AmbariClient(Uri baseUri, string username, string password, string hadoopUserName, HttpMessageHandler handler)
        {
            client = new AmbariHttpClient(baseUri, username, password, hadoopUserName, handler);
        }

        public IList<ClusterInfo> GetClusters()
        {
            var clusterInfos = new List<ClusterInfo>();
            Task<HttpResponseMessage> clusters = client.GetClusters();
            JObject results =  HttpClientTools.GetTaskResults(clusters);
            JToken items = results["items"];
            foreach (JToken cluster in items)
            {
                string href = cluster["href"].Value<string>();
                clusterInfos.Add(new ClusterInfo(href));
            }
  
            return clusterInfos;
        }

        public HostComponentMetric GetHostComponentMetric(string clusterName)
        {
            Task<HttpResponseMessage> clusters = client.GetHostComponentMetric(clusterName, HeadnodeName);
            JObject results = HttpClientTools.GetTaskResults(clusters);
            int mapsLaunched = results["metrics"]["mapred.JobTracker"]["maps_launched"].Value<int>();

            //Get maps_completed so far
            int mapsCompleted = results["metrics"]["mapred.JobTracker"]["maps_completed"].Value<int>();

            //Get maps_failed so far
            int mapsFailed = results["metrics"]["mapred.JobTracker"]["maps_failed"].Value<int>();

            //Get maps_killed so far
            int mapsKilled = results["metrics"]["mapred.JobTracker"]["maps_killed"].Value<int>();

            //Get maps waiting 
            int mapsWaiting = results["metrics"]["mapred.JobTracker"]["waiting_maps"].Value<int>();

            //Get maps running
            int mapsRunning = results["metrics"]["mapred.JobTracker"]["running_maps"].Value<int>();
            return new HostComponentMetric(mapsLaunched, mapsCompleted, mapsFailed, mapsKilled, mapsWaiting, mapsRunning);
        }

        public IEnumerable<double> GetAsvMetrics(string storageAccount, DateTime start, DateTime end)
        {
            Task<HttpResponseMessage> t1 = client.GetAsvMetrics(storageAccount, start, end);
            JObject results = HttpClientTools.GetTaskResults(t1);

            JArray array = results["azureFileSystem.azureFileSystem"]["asv_raw_bytes_uploaded"].Value<JArray>();
            return array.Select(token => token.Value<double>()).ToList();
        }
    }
}

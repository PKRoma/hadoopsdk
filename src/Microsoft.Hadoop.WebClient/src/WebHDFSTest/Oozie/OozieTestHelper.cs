using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Hadoop.WebClient.Common;
using Microsoft.Hadoop.WebClient.OozieClient;
using Microsoft.Hadoop.WebClient.OozieClient.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace WebClientTests.Oozie
{
    class OozieTestHelper
    {
        private readonly OozieHttpClient client;
        private readonly TestContext testContext;
        readonly TimeSpan waitForRunningStatusTimeout = TimeSpan.FromMinutes(2);

        public OozieTestHelper(OozieHttpClient client, TestContext testContext)
        {
            this.client = client;
            this.testContext = testContext;
        }

        public void StartJob(string id)
        {
            Task<HttpResponseMessage> t1 = client.StartJob(id);
            HttpClientTools.WaitForTaskCompletion(t1);
            
            WaitForStatus(id, OozieJobStatus.Running, waitForRunningStatusTimeout);
        }

        public string GetSystemMode()
        {
            Task<HttpResponseMessage> t1 = client.GetStatus();
            var res = HttpClientTools.GetTaskResults(t1);

            string systemMode = res["systemMode"].Value<string>();
            return systemMode;
        }

        public string SubmitJob(Dictionary<string, string> properties)
        {
            Task<HttpResponseMessage> t1 = client.SubmitJob(properties);
            var res = HttpClientTools.GetTaskResults(t1);
            string systemMode = res["id"].Value<string>();
            return systemMode;
        }

        public void WaitForStatus(string jobId, string expectedStatus, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;
            while (true)
            {
                string status = GetJobStatus(jobId);
                testContext.WriteLine("Waiting for job '{0}' to change status from {1} to {2}", jobId, status, expectedStatus);                if (status.ToLower() == expectedStatus.ToLower())
                {
                    break;
                }

                if (DateTime.Now - startTime > timeout)
                {
                    string errMgs = string.Format("Job '{0}' didn't change status to {1} after {2} minutes", jobId,
                                                  expectedStatus, timeout.TotalMinutes);
                    throw new Exception(errMgs);
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        public void WaitForStatusChange(string jobId, string expectedStatus, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;
            while (true)
            {
                string status = GetJobStatus(jobId);
                testContext.WriteLine("Waiting for job '{0}' to change status to something other than {1}, current status is {2}", jobId, expectedStatus, status);
                if (status.ToLower() != expectedStatus.ToLower())
                {
                    break;
                }

                if (DateTime.Now - startTime > timeout)
                {
                    string errMgs = string.Format("Job '{0}' didn't change status from {1} after {2} minutes", jobId,
                                                  expectedStatus, timeout.TotalMinutes);
                    throw new Exception(errMgs);
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        public string GetJobStatus(string jobId)
        {
            Task<HttpResponseMessage> t1 = client.GetJobInfo(jobId);
            var res = HttpClientTools.GetTaskResults(t1);

            string systemMode = res["status"].Value<string>();
            return systemMode;
        }

        public void KillJob(string id)
        {
            Task<HttpResponseMessage> t1 = client.KillJob(id);
            HttpClientTools.WaitForTaskCompletion(t1);
            WaitForStatus(id, OozieJobStatus.Killed, TimeSpan.FromMinutes(5));
        }

        public void SuspendJob(string id)
        {
            Task<HttpResponseMessage> t1 = client.SuspendJob(id);
            HttpClientTools.WaitForTaskCompletion(t1);
            WaitForStatus(id, OozieJobStatus.Suspended, TimeSpan.FromMinutes(5));
        }

        public void ResumeJob(string id)
        {
            Task<HttpResponseMessage> t2 = client.ResumeJob(id);
            HttpClientTools.WaitForTaskCompletion(t2);
            WaitForStatus(id, OozieJobStatus.Running, TimeSpan.FromMinutes(5));
        }
    }
}

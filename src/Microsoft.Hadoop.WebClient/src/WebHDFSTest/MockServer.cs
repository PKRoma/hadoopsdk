using Microsoft.Hadoop.WebHCat;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Hadoop;
using System.IO;
using Newtonsoft.Json;

namespace WebClientTests
{
    internal class MockServer : HttpMessageHandler
    {
        //private ConcurrentDictionary<string, Job> activeJobs;

        //private ConcurrentDictionary<string, Job> completedJobs;

        private readonly Microsoft.Hadoop.Version version;

        private readonly int jobDuration;

        private Func<object, HttpResponseMessage> func;

        // TODO - add delay for jobs
        internal MockServer(Microsoft.Hadoop.Version version, int jobDuration)
        {
            this.version = version;
            //this.activeJobs = new ConcurrentDictionary<string, Job>();
            //this.completedJobs = new ConcurrentDictionary<string, Job>();
            this.jobDuration = jobDuration;
            this.func = RunJob;
            //Task.Run((Action)this.EndJobs);
        }

        internal void Reset()
        {
            //this.activeJobs.Clear();
            //this.completedJobs.Clear();
        }

        internal bool CompleteJob(string jobID)
        {
            // todo
            return true;
        }

        private void  EndJobs()
        {
            while (true)
            {
                Thread.Sleep(this.jobDuration * 1000);
                // todo - add job completion code
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post)
                return this.HandlePost(request);
            else
                throw new NotImplementedException();
        }

        private Task<HttpResponseMessage> HandlePost(HttpRequestMessage request)
        {
            if (string.Equals(request.RequestUri.AbsolutePath, "/templeton/v1/hive", StringComparison.OrdinalIgnoreCase))
            {
                // Hive job
                //HiveJob job = (HiveJob) JobFactory.NewJob(Microsoft.Hadoop.JobType.Hive, Microsoft.Hadoop.Version.V1);
                string jobID = GenerateJobID();
                //this.activeJobs.TryAdd(jobID, job);

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Created);
                message.Content = new StringContent(GenerateJobCreationInfo(jobID), Encoding.Unicode, "application/json");
                var task = Task.Factory.StartNew<HttpResponseMessage>(this.func, message);

                return task;
            }
            else if (string.Equals(request.RequestUri.AbsolutePath, "/templeton/v1/pig", StringComparison.OrdinalIgnoreCase))
            {
                // Hive job
                //PigJob job = (PigJob)JobFactory.NewJob(Microsoft.Hadoop.JobType.Pig, Microsoft.Hadoop.Version.V1);
                string jobID = GenerateJobID();
                //this.activeJobs.TryAdd(jobID, job);

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Created);
                message.Content = new StringContent(GenerateJobCreationInfo(jobID), Encoding.Unicode, "application/json");
                var task = Task.Factory.StartNew<HttpResponseMessage>(this.func, message);

                return task;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private HttpResponseMessage RunJob(object message)
        {
            return (HttpResponseMessage) message;
        }

        private string GenerateJobID()
        {
            DateTimeOffset dto = DateTimeOffset.UtcNow.LocalDateTime;
            string dt = dto.ToString("yyyyMMddHHMM");
            string counter = dto.Millisecond.ToString();  // kind of hacky, but don't need to lock a counter
            return "job_" + dt + "_0" + counter;
        }

        private string GenerateJobCreationInfo(string jobID)
        {
            var sw = new StringWriter(new StringBuilder());

            var jw = new JsonTextWriter(sw);
            jw.WriteStartObject();
            jw.WritePropertyName("id");
            jw.WriteValue(jobID);
            jw.WritePropertyName("info");
            jw.WriteStartObject();
            jw.WritePropertyName("stdout");
            jw.WriteValue("templeton-job-id:" + jobID);
            jw.WritePropertyName("stderr");
            jw.WriteValue("");
            jw.WritePropertyName("exitcode");
            jw.WriteValue(0);
            jw.WriteEndObject();
            jw.WriteEndObject();

            return sw.ToString();
        }
    }
}

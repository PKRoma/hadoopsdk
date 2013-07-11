namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.ServerDataObjects
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.Common.Models;
    using Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data;

    internal class JobPayloadServerConverter
    {
        internal string SerializeJobList(HDInsightJobList jobs)
        {
            var result = new PassthroughResponse();
            if (jobs.ErrorCode.IsNotNullOrEmpty() || jobs.HttpStatusCode != HttpStatusCode.Accepted)
            {
                result.Error = new PassthroughErrorResponse { StatusCode = jobs.HttpStatusCode, ErrorId = jobs.ErrorCode };
            }
            result.Data = jobs.JobIds;
            return this.SerializeJobDetails(result);
        }

        internal string SerializeJobCreationResults(HDInsightJobCreationResults jobResults)
        {
            var result = new PassthroughResponse();
            if (jobResults.ErrorCode.IsNotNullOrEmpty() || jobResults.HttpStatusCode != HttpStatusCode.Accepted)
            {
                result.Error = new PassthroughErrorResponse { StatusCode = jobResults.HttpStatusCode, ErrorId = jobResults.ErrorCode };
            }
            result.Data = jobResults.JobId;
            return this.SerializeJobDetails(result);
        }

        private ClientJobRequest DeserializePayload(string payload)
        {
            ClientJobRequest request;
            var knowTypes = new Type[]
            {
                typeof(JobRequest), 
                typeof(HiveJobRequest), 
                typeof(MapReduceJobRequest)
            };
            DataContractSerializer ser = new DataContractSerializer(typeof(ClientJobRequest), knowTypes);
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(payload);
                writer.Flush();
                stream.Flush();
                stream.Position = 0;
                request = (ClientJobRequest)ser.ReadObject(stream);
            }
            return request;
        }

        private void SetStandardProperties(ClientJobRequest request, HDInsightJobCreationDetails details)
        {
            foreach (var jobRequestParameter in request.Parameters)
            {
                details.Parameters.Add(jobRequestParameter.Key, jobRequestParameter.Value.ToString());
            }
            foreach (var jobRequestParameter in request.Resources)
            {
                details.Resources.Add(jobRequestParameter.Key, jobRequestParameter.Value.ToString());
            }
        }

        internal HDInsightHiveJobCreationDetails DeserializeHiveJobCreationDetails(string content)
        {
            var request = this.DeserializePayload(content);
            var result = new HDInsightHiveJobCreationDetails()
            {
                JobName = request.JobName,
                OutputStorageLocation = request.OutputStorageLocation,
                Query = request.Query
            };
            this.SetStandardProperties(request, result);
            return result;
        }

        internal HDInsightMapReduceJobCreationDetails DeserializeMapReduceJobCreationDetails(string content)
        {
            var request = this.DeserializePayload(content);
            var result = new HDInsightMapReduceJobCreationDetails()
            {
                ApplicationName = request.ApplicationName,
                JarFile = request.JarFile,
                JobName = request.JobName,
                OutputStorageLocation = request.OutputStorageLocation,
            };
            result.Arguments.AddRange(request.Arguments);
            this.SetStandardProperties(request, result);
            return result;
        }

        internal string SerializeJobDetails(HDInsightJob job)
        {
            var result = new PassthroughResponse();
            if (job.ErrorCode.IsNotNullOrEmpty() || job.HttpStatusCode != HttpStatusCode.Accepted)
            {
                result.Error = new PassthroughErrorResponse { StatusCode = job.HttpStatusCode, ErrorId = job.ErrorCode };
            }
            JobDetails details = new JobDetails()
            {
                ErrorOutputPath = job.ErrorOutputPath,
                ExitCode = job.ExitCode,
                LogicalOutputPath = job.LogicalOutputPath,
                Name = job.Name,
                PhysicalOutputPath = new Uri(job.PhysicalOutputPath),
                Query = job.Query,
                SubmissionTime = job.SubmissionTime.Ticks.ToString()
            };
            JobStatusCode statusCode;
            Assert.IsTrue(JobStatusCode.TryParse(job.StatusCode, out statusCode));
            details.StatusCode = statusCode;
            result.Data = details;
            return this.SerializeJobDetails(result);
        }

        private string SerializeJobDetails(PassthroughResponse result)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(PassthroughResponse));
            using (var stream = new MemoryStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    ser.WriteObject(stream, result);
                    stream.Flush();
                    stream.Position = 0;
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

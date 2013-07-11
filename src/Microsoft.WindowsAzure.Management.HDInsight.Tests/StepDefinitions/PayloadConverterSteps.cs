namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.StepDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.ServerDataObjects;
    using TechTalk.SpecFlow;

    [Binding]
    public class PayloadConverterSteps
    {
        private object transferObject;
        private string serializedForm;

        public void BeforeScenario()
        {
            
        }

        public void AfterScenario()
        {
            
        }
        
        [Given(@"I have a job list object")]
        public void GivenIHaveAListOfJobIds()
        {
            var list = new HDInsightJobList();
            list.HttpStatusCode = HttpStatusCode.Accepted;
            list.ErrorCode = string.Empty;
            this.transferObject = list;
        }

        [Given(@"I have a job details object")]
        public void GivenIHaveAJobDetailsObject()
        {
            var job = new HDInsightJob();
            job.ErrorCode = string.Empty;
            job.ErrorOutputPath = string.Empty;
            job.ExitCode = 0;
            job.HttpStatusCode = HttpStatusCode.Accepted;
            job.JobId = string.Empty;
            job.LogicalOutputPath = string.Empty;
            job.Name = string.Empty;
            job.PhysicalOutputPath = "http://output";
            job.Query = string.Empty;
            job.StatusCode = JobStatusCode.Unknown.ToString();
            job.SubmissionTime = DateTime.UtcNow;
            this.transferObject = job;
        }

        [Given(@"I have a map reduce job request object")]
        public void GivenIHaveAMapReduceJobRequestObject()
        {
            var request = new HDInsightMapReduceJobCreationDetails();
            request.ApplicationName = string.Empty;
            request.JarFile = string.Empty;
            request.JobName = string.Empty;
            request.OutputStorageLocation = string.Empty;
            this.transferObject = request;
        }

        [Given(@"I have a hive job request object")]
        public void GivenIHaveAHiveJobRequestObject()
        {
            var request = new HDInsightHiveJobCreationDetails();
            request.JobName = string.Empty;
            request.Query = string.Empty;
            request.OutputStorageLocation = string.Empty;
            this.transferObject = request;
        }

        [Given(@"I add the following argument ""(.*)""")]
        public void GivenIAddTheFollowingArgument(string argument)
        {
            ((HDInsightMapReduceJobCreationDetails)this.transferObject).Arguments.Add(argument);
        }

        [Given(@"I add the following parameter ""(.*)"" with a value of ""(.*)""")]
        public void IAddTheFollowingParameter_key_WithAValueOf_value(string key, string value)
        {
            ((HDInsightJobCreationDetails)this.transferObject).Parameters.Add(key, value);
        }

        [Given(@"I add the following resource ""(.*)"" with a value of ""(.*)""")]
        public void IAddTheFollowingResource_key_WithAValueOf_value(string key, string value)
        {
            ((HDInsightJobCreationDetails)this.transferObject).Resources.Add(key, value);
        }

        [Given(@"I set the job name as ""(.*)""")]
        public void GivenISetTheJobNameAs(string name)
        {
            ((HDInsightJobCreationDetails)this.transferObject).JobName = name;
        }

        [Given(@"I set the application name as ""(.*)""")]
        public void GivenISetTheApplicationNameAs_name(string name)
        {
            ((HDInsightMapReduceJobCreationDetails)this.transferObject).ApplicationName = name;
        }

        [Given(@"I set the Jar file as ""(.*)""")]
        public void GivenISetTheJarFileAs_jarFile(string jarFile)
        {
            ((HDInsightMapReduceJobCreationDetails)this.transferObject).JarFile = jarFile;
        }

        [Given(@"I set the Output Storage Location as ""(.*)""")]
        public void GivenISetTheOutputStorageLocationAs_outputStorageLocation(string outputStorageLocation)
        {
            ((HDInsightJobCreationDetails)this.transferObject).OutputStorageLocation = outputStorageLocation;
        }

        [Given(@"I set the Query to the following:")]
        public void GivenISetTheQueryToTheFollowing_query(string query)
        {
            ((HDInsightHiveJobCreationDetails)this.transferObject).Query = query;
        }

        [Given(@"the request will return the http status code ""(.*)""")]
        public void GivenTheJobRequestWillReturnTheStatusCode_httpStatusCode(HttpStatusCode code)
        {
            this.transferObject.GetType().GetProperty("HttpStatusCode").SetValue(this.transferObject, code);
        }

        [Given(@"the jobId ""(.*)"" is added to the list of jobIds")]
        public void GivenTheJobId_jobId_IsAddedToTheListOfJobIds(string jobId)
        {
            ((HDInsightJobList)this.transferObject).JobIds.Add(jobId);
        }

        [Given(@"the request will return the error id ""(.*)""")]
        public void GivenIHaveTheError_error(string error)
        {
            this.transferObject.GetType().GetProperty("ErrorCode").SetValue(this.transferObject, error);
        }

        [Given(@"the job has the name ""(.*)""")]
        public void GivenTheJobHasTheName_name(string name)
        {
            ((HDInsightJob)this.transferObject).Name = name;
        }

        [When(@"I serialize the object")]
        public void WhenISerializeTheListOfJobIds()
        {
            var serverConverter = new JobPayloadServerConverter();
            var clientConverter = new JobPayloadConverter();
            var asList = this.transferObject.As<HDInsightJobList>();
            if (asList.IsNotNull())
            {
                this.serializedForm = serverConverter.SerializeJobList(asList);
                return;
            }
            var asDetail = this.transferObject.As<HDInsightJob>();
            if (asDetail.IsNotNull())
            {
                this.serializedForm = serverConverter.SerializeJobDetails(asDetail);
                return;
            }
            var asRequest = this.transferObject.As<HDInsightJobCreationDetails>();
            if (asRequest.IsNotNull())
            {
                this.serializedForm = clientConverter.SerializeJobCreationDetails(asRequest);
                return;
            }
            Assert.Fail("Unable to recognize the object type.");
        }

        [Then(@"the value of the serialized output should be equivalent with the original")]
        public void TheValueOfTheSerializedOutputShouldBeEquivalentWithTheOrignal()
        {
            var clientConverter = new JobPayloadConverter();
            var serverConverter = new JobPayloadServerConverter();
            HDInsightJobList asList = this.transferObject.As<HDInsightJobList>();
            if (asList.IsNotNull())
            {
                HDInsightJobList actual = clientConverter.DeserializeJobList(this.serializedForm);
                Assert.AreEqual(asList.ErrorCode, actual.ErrorCode);
                Assert.AreEqual(asList.HttpStatusCode, actual.HttpStatusCode);
                Assert.IsTrue(asList.JobIds.SequenceEqual(actual.JobIds));
                return;
            }
            HDInsightJob asJob = this.transferObject.As<HDInsightJob>();
            if (asJob.IsNotNull())
            {
                HDInsightJob actual = clientConverter.DeserializeJobDetails(this.serializedForm, asJob.JobId);
                Assert.AreEqual(asJob.ErrorCode, actual.ErrorCode);
                Assert.AreEqual(asJob.HttpStatusCode, actual.HttpStatusCode);
                Assert.AreEqual(asJob.ErrorOutputPath, actual.ErrorOutputPath);
                Assert.AreEqual(asJob.ExitCode, actual.ExitCode);
                Assert.AreEqual(asJob.JobId, actual.JobId);
                Assert.AreEqual(asJob.LogicalOutputPath, actual.LogicalOutputPath);
                Assert.AreEqual(asJob.Name, actual.Name);
                Assert.AreEqual(new Uri(asJob.PhysicalOutputPath), new Uri(actual.PhysicalOutputPath));
                Assert.AreEqual(asJob.Query, actual.Query);
                Assert.AreEqual(asJob.StatusCode, actual.StatusCode);
                Assert.AreEqual(asJob.SubmissionTime, actual.SubmissionTime);
                return;
            }
            var asMapReduce = this.transferObject.As<HDInsightMapReduceJobCreationDetails>();
            if (asMapReduce.IsNotNull())
            {
                HDInsightMapReduceJobCreationDetails actual = serverConverter.DeserializeMapReduceJobCreationDetails(this.serializedForm);
                Assert.AreEqual(asMapReduce.ApplicationName, actual.ApplicationName);
                Assert.IsTrue(asMapReduce.Arguments.SequenceEqual(actual.Arguments));
                Assert.AreEqual(asMapReduce.JarFile, actual.JarFile);
                Assert.AreEqual(asMapReduce.JobName, actual.JobName);
                Assert.AreEqual(asMapReduce.OutputStorageLocation, actual.OutputStorageLocation);
                Assert.IsTrue(asMapReduce.Parameters.SequenceEqual(actual.Parameters));
                Assert.IsTrue(asMapReduce.Resources.SequenceEqual(actual.Resources));
                return;
            }
            var asHive = this.transferObject.As<HDInsightHiveJobCreationDetails>();
            if (asHive.IsNotNull())
            {
                HDInsightHiveJobCreationDetails actual = serverConverter.DeserializeHiveJobCreationDetails(this.serializedForm);
                Assert.AreEqual(asHive.JobName, actual.JobName);
                Assert.AreEqual(asHive.OutputStorageLocation, actual.OutputStorageLocation);
                Assert.IsTrue(asHive.Parameters.SequenceEqual(actual.Parameters));
                Assert.AreEqual(asHive.Query, actual.Query);
                Assert.IsTrue(asHive.Resources.SequenceEqual(actual.Resources));
                return;
            }
            Assert.Fail("Unable to recognize the object type.");
        }

        [Given(@"the job has the Error Output Path ""(.*)""")]
        public void GivenTheJobHasTheErrorOutputPath_path(string path)
        {
            ((HDInsightJob)this.transferObject).ErrorOutputPath = path;
        }

        [Given(@"the job has an Exit Code of (\d+)")]
        public void GivenTheJobHasTheExitCode_exitCode(int exitCode)
        {
            ((HDInsightJob)this.transferObject).ExitCode = exitCode;
        }

        [Given(@"the job has a Job Id of ""(.*)""")]
        public void GivenTheJobHasTheJobId_jobId(string jobId)
        {
            ((HDInsightJob)this.transferObject).JobId = jobId;
        }

        [Given(@"the job has the Logical Output Path ""(.*)""")]
        public void GivenTheJobHasTheLogicalOutputPath_logicalOutputPath(string logicalOutputPath)
        {
            ((HDInsightJob)this.transferObject).LogicalOutputPath = logicalOutputPath;
        }

        [Given(@"the job has the Physical Output Path ""(.*)""")]
        public void GivenTheJobHasThePhysicalOutputPath_physicalOutputPath(string physicalOutputPath)
        {
            ((HDInsightJob)this.transferObject).PhysicalOutputPath = physicalOutputPath;
        }

        [Given(@"the job has the following query:")]
        public void GivenTheJobHasTheFollowingQuery_query(string query)
        {
            ((HDInsightJob)this.transferObject).Query = query;
        }

        [Given(@"the job has the Status Code ""(.*)""")]
        public void GivenTheJobHasTheStatusCode(string statusCode)
        {
            ((HDInsightJob)this.transferObject).StatusCode = statusCode;
        }

        [Given(@"the job has was submitted on ""(.*)""")]
        public void GivenTheJobWasSubmitedOnt_date(DateTime date)
        {
            ((HDInsightJob)this.transferObject).SubmissionTime = date;
        }

    }
}

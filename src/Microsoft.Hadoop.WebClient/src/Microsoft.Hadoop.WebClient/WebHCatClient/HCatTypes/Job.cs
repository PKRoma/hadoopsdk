using Microsoft.Hadoop.WebHDFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHCat
{
    //public abstract class Job : Resource
    //{
    //    public string Id { get; internal set; }
    //    public string StatusDirectory { get; internal set; }   // todo - need to switch to real directory
    //    public Uri CallBack { get; internal set; }  // todo - need to switch to real file object

    //    public JobStatus GetStatus() { throw new NotImplementedException(); } // TODO - provide implementation

    //    public IEnumerable<T> GetResults<T>() { throw new NotImplementedException(); }
    //}

    //public abstract class JobStatus : Resource
    //{
    //    public abstract bool Completed { get; }
    //}

    //public static class JobFactory
    //{
    //    public static Job NewJob(JobType type, Version version)
    //    {
    //        switch (version)
    //        {
    //            case Version.V1:
    //            {
    //                switch (type)
    //                {
    //                    case JobType.Hive:
    //                        return new HiveJob();
    //                    case JobType.Pig:
    //                        return new PigJob();
    //                    default:
    //                        throw new NotImplementedException();
    //                }
    //                //break;
    //            }
    //            default:
    //                throw new NotImplementedException();
    //        }
    //    }

    //}
}

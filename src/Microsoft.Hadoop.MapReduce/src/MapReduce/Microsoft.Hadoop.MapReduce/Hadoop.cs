using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce
{
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using Microsoft.Hadoop.MapReduce.HadoopImplementations;

    public delegate IHadoop LocalHadoopConstructor();

    public delegate IHadoop OneBoxHadoopConstructor(Uri clusterName, string userName, string password);

    public delegate IHadoop AzureHadoopConstructor(Uri clusterName, 
                                                     string userName, 
                                                     string hadoopUser,
                                                     string password, 
                                                     string storageAccount, 
                                                     string storageKey,
                                                     string container,
                                                     bool createContainerIfMissing);

    public static class Hadoop
    {
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible",
            Justification = "Suppressing for now until I can bring in IOC refactor.")]
        public static LocalHadoopConstructor MakeLocal = () => LocalHadoop.Create();
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible",
            Justification = "Suppressing for now until I can bring in IOC refactor.")]
        public static OneBoxHadoopConstructor MakeOneBox = (cluster, user, password) => WebHadoop.Create(cluster, user, password);
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible",
            Justification = "Suppressing for now until I can bring in IOC refactor.")]
        public static AzureHadoopConstructor MakeAzure = (cluster, user, hadoopUser, password, account, key, container, create) =>  
                                                           HadoopOnAzure.Create(cluster, user, hadoopUser, password, account, key, container, create);

        public static IHadoop Connect()
        {
            return MakeLocal();
        }

        public static IHadoop Connect(Uri clusterName, string userName, string password)
        {
            return MakeOneBox(clusterName, userName, password);
        }

        public static IHadoop Connect(Uri clusterName,
                                      string userName,
                                      string hadoopUser,
                                      string password,
                                      string storageAccount,
                                      string storageKey,
                                      string container,
                                      bool createContainerIfMissing)
        {
            return MakeAzure(clusterName, userName, hadoopUser, password, storageAccount, storageKey, container, createContainerIfMissing);
        }
    }
}

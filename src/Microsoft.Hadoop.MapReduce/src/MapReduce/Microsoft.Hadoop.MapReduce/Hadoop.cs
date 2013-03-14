using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce
{
    using System.Security;
    using Microsoft.Hadoop.MapReduce.HadoopImplementations;

    internal delegate IHadoop LocalHadoopConstructor();

    internal delegate IHadoop OneBoxHadoopConstructor(Uri clusterName, string userName, string password);

    internal delegate IHadoop AzureHadoopConstructor(Uri clusterName, 
                                                     string userName, 
                                                     string hadoopUser,
                                                     string password, 
                                                     string storageAccount, 
                                                     string storageKey,
                                                     string container,
                                                     bool createContainerIfMissing);

    public static class Hadoop
    {
        internal static LocalHadoopConstructor makeLocal = () => LocalHadoop.Create();
        internal static OneBoxHadoopConstructor makeOneBox = (cluster, user, password) => WebHadoop.Create(cluster, user, password);
        internal static AzureHadoopConstructor makeAzure = (cluster, user, hadoopUser, password, account, key, container, create) =>  
                                                           HadoopOnAzure.Create(cluster, user, hadoopUser, password, account, key, container, create);

        public static IHadoop Connect()
        {
            return makeLocal();
        }

        public static IHadoop Connect(Uri clusterName, string userName, string password)
        {
            return makeOneBox(clusterName, userName, password);
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
            return makeAzure(clusterName, userName, hadoopUser, password, storageAccount, storageKey, container, createContainerIfMissing);
        }
    }
}

using System;
using System.Configuration;

namespace WebClientTests
{
    internal static class TestConfig
    {
        public static string ClusterUrl
        {
            get
            {
                return GetValue("ClusterUrl");
            }
        }

        public static string ClusterUser
        {
            get
            {
                return GetValue("ClusterUser");
            }
        }

        public static string ClusterPassword
        {
            get
            {
                return GetValue("ClusterPassword");
            }
        }

        public static string StorageAccount
        {
            get
            {
                return GetValue("StorageAccount");
            }
        }

        public static string StoragePassword
        {
            get
            {
                return GetValue("StoragePassword");
            }
        }

        public static string ContainerName
        {
            get
            {
                return GetValue("ContainerName");
            }
        }

        private static string GetValue(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception(string.Format("app.config is not fully updated. Key '{0}' has no value", key));
            }

            return value;
        }

    }
}

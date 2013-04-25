namespace Microsoft.Hadoop.WebClient.AmbariClient.Contracts
{
    public class ClusterInfo
    {
        private readonly string href;

        public ClusterInfo(string href)
        {
            this.href = href;
        }

        public string Href
        {
            get { return href; }
        }
    }
}
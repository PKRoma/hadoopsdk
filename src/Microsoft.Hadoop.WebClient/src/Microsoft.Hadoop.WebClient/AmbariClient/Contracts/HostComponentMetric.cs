namespace Microsoft.Hadoop.WebClient.AmbariClient.Contracts
{
    public class HostComponentMetric
    {
        private readonly int mapsLaunched;
        private readonly int mapsCompleted;
        private readonly int mapsFailed;
        private readonly int mapsKilled;
        private readonly int mapsWaiting;
        private readonly int mapsRunning;

        public HostComponentMetric(int mapsLaunched, int mapsCompleted, int mapsFailed, int mapsKilled, 
                                   int mapsWaiting, int mapsRunning)
        {
            this.mapsLaunched = mapsLaunched;
            this.mapsCompleted = mapsCompleted;
            this.mapsFailed = mapsFailed;
            this.mapsKilled = mapsKilled;
            this.mapsWaiting = mapsWaiting;
            this.mapsRunning = mapsRunning;
        }

        public int MapsLaunched
        {
            get { return mapsLaunched; }
        }

        public int MapsCompleted
        {
            get { return mapsCompleted; }
        }

        public int MapsFailed
        {
            get { return mapsFailed; }
        }

        public int MapsKilled
        {
            get { return mapsKilled; }
        }

        public int MapsWaiting
        {
            get { return mapsWaiting; }
        }

        public int MapsRunning
        {
            get { return mapsRunning; }
        }
    }
}
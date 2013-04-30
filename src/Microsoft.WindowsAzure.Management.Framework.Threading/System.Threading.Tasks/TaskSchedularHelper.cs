namespace System.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class TaskSchedularHelper
    {
        /// <summary>
        /// Gets a task scheduler.
        /// </summary>
        public static TaskScheduler TaskScheduler
        {
            get
            {
                if (SynchronizationContext.Current == null)
                {
                    return TaskScheduler.Default;
                }
                return TaskScheduler.FromCurrentSynchronizationContext();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Common
{
    public static class JobScheduler
    {
        private static List<IJob> Jobs { get; } = new List<IJob>();

        public static void Start()
        {
            Jobs.ForEach(p => p.Run());
        }

        public static void Add(IJob job)
        {
            Jobs.Add(job);
        }
    }
}
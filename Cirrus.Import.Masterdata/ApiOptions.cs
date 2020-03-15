using System.Collections.Generic;

namespace Cirrus.Import.Masterdata
{
    class ApiOptions
    {
        public string Endpoint { get; set; }

        public string Token { get; set; }

        public bool CheckAfterUpdate { get; set; }

        public int TimeoutInSeconds { get; set; }

        public List<int> RetryIntervalsInSeconds { get; set; } = new List<int>();

        public int MaxDegreeOfParallelism { get; set; }
    }
}

using System.Collections.Concurrent;
using System.Diagnostics;

namespace GrpcClient.Models
{
    public class RequestHistory
    {
        public static ConcurrentDictionary<long, RequestInfo> Requests { get; set; } = new();
    }

    public class RequestInfo
    {
        public RequestInfo(PrimeNumberRequest primeNumberRequest)
        {
            PrimeNumberRequest = primeNumberRequest;
            Stopwatch.Start();
        }

        public long RetryIntervalInMilliseconds { get; set; } = 15 * 1000;
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        public PrimeNumberRequest PrimeNumberRequest { get; set; }

    }
}

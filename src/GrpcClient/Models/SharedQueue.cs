using System.Collections.Concurrent;

namespace GrpcClient.Models
{
    public class SharedQueue
    {
        public static ConcurrentQueue<PrimeNumberRequest> Requests = new();
    }
}

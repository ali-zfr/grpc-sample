using System.Collections.Concurrent;
namespace GrpcServer
{

    public static class SharedQueue
    {
        public static ConcurrentQueue<EventPayload> NumbersQueue = new();
    }

    public record EventPayload(long Id, long Number, bool IsPrime);
}

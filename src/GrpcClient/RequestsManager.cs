using GrpcClient.Models;

namespace GrpcClient
{
    public class RequestsManager
    {
        public void QueueRequests()
        {
            var random = new Random();
            long count = 0;
            try
            {
                while (true)
                {
                    SharedQueue.Requests.Enqueue(new PrimeNumberRequest()
                    {
                        Id = ++count,
                        Number = random.Next(1, 1001),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in QueueRequests: {ex.Message}");
            }
        }


        public void ProcessRetries()
        {
            try
            {
                while (true)
                {
                    var requestsToRetry = RequestHistory.Requests.Where(
                        x => x.Value.Stopwatch.ElapsedMilliseconds > x.Value.RetryIntervalInMilliseconds);

                    foreach (var request in requestsToRetry)
                    {
                        request.Value.RetryIntervalInMilliseconds = request.Value.RetryIntervalInMilliseconds * request.Value.RetryIntervalInMilliseconds;

                        SharedQueue.Requests.Enqueue(request.Value.PrimeNumberRequest);
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in ProcessRetries: {ex.Message}");
            }
        }

    }
}

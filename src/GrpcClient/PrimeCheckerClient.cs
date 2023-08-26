using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient.Models;
using System.Runtime.CompilerServices;

namespace GrpcClient
{
    public class PrimeCheckerClient
    {

        private AsyncDuplexStreamingCall<PrimeNumberRequest, PrimeNumberResponse> call;

        public PrimeCheckerClient() 
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5015");
            var service = new PrimeChecker.PrimeCheckerClient(channel);
            call = service.IsPrime();

            ArgumentNullException.ThrowIfNull(call);
        }

        public async Task SendRequests()
        {
            try
            {
                while (true)
                {
                    var isDequed = SharedQueue.Requests.TryDequeue(out PrimeNumberRequest? request);
                    if (isDequed && request != null)
                    {
                        RequestInfo requestInfo = new(request);
                        var isNewRequest = RequestHistory.Requests.TryAdd(requestInfo.PrimeNumberRequest.Id, requestInfo);

                        if (isNewRequest)
                            requestInfo.Stopwatch.Start();
                        await call.RequestStream.WriteAsync(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in SendRequests: {ex.Message}");
            }
            finally
            {
                await call.RequestStream.CompleteAsync();
            }
        }

        public async Task RecieveResponse()
        {
            try
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    var removed = RequestHistory.Requests.TryRemove(message.Id, out RequestInfo? requestInfo);
                    Console.Write(message.ToString());

                    if (removed && requestInfo != null)
                    {
                        requestInfo.Stopwatch.Stop();
                        Console.WriteLine($" | Round Trip Time: {requestInfo.Stopwatch.Elapsed.TotalMilliseconds} ms");
                    }
                    else
                    {
                        Console.WriteLine($" | Round Trip Time: NOT_FOUND");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in ReceiveResponse: {ex.Message}");
            }
        }
    }
}

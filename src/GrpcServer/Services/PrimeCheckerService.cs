using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services
{
    public class PrimeCheckerService : PrimeChecker.PrimeCheckerBase
    {
        private readonly ILogger<PrimeCheckerService> _logger;
        public PrimeCheckerService(ILogger<PrimeCheckerService> logger)
        {
            _logger = logger;
        }

        public override Task<PrimeNumberResponse> IsPrime(PrimeNumberRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Request Recieved: {request.Number} | {request.Timestamp}");
            return Task.FromResult(new PrimeNumberResponse() { IsPrime = true });
        }
    }
}
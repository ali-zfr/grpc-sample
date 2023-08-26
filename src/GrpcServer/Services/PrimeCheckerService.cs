using Grpc.Core;

namespace GrpcServer.Services
{
    public class PrimeCheckerService : PrimeChecker.PrimeCheckerBase
    {
        private readonly ILogger<PrimeCheckerService> _logger;
        public PrimeCheckerService(ILogger<PrimeCheckerService> logger)
        {
            _logger = logger;
        }

        public override async Task IsPrime(IAsyncStreamReader<PrimeNumberRequest> requestStream, IServerStreamWriter<PrimeNumberResponse> responseStream, ServerCallContext context)
        {
            /*
            * 2- verify if the sent number (which will be a random number between 1 and 1000) is prime or not
            * 3- respond client 
            * 6- Display every second the top 10 highest requested/validated prime numbers and total number of messages received 
            */
            await foreach (var message in requestStream.ReadAllAsync())
            {

                if (message.Number < 1 || message.Number > 1000)
                    throw new Exception("Number must be within the range (1-1000)");

                var primeNumberResponse = new PrimeNumberResponse()
                {
                    Id = message.Id
                };

                if (IsPrime(message.Number))
                {
                    primeNumberResponse.IsPrime = true;
                    primeNumberResponse.Message = $"{message.Number} is a prime number";
                }
                else
                {
                    primeNumberResponse.IsPrime = false;
                    primeNumberResponse.Message = $"{message.Number} is not a prime number";
                }

                SharedQueue.NumbersQueue.Enqueue(new EventPayload(message.Id, message.Number, primeNumberResponse.IsPrime));

                await responseStream.WriteAsync(primeNumberResponse);
            }
            
        }

        /// <summary>
        /// natural number n>1 is called a prime number if its only positive divisors are 1 and n
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsPrime(long n)
        {
            if (n < 2 || n % 2 == 0) return false;
            if (n == 2) return true;

            // check whether it is divisible by any prime number less than or equal to the square root of n
            for (int i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0) return false;
            }

            return true;
        }
    }
}
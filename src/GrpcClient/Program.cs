// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using System.Threading.Channels;


var channel = GrpcChannel.ForAddress("http://localhost:5015");
var service = new PrimeChecker.PrimeCheckerClient(channel);

var response = service.IsPrime(new PrimeNumberRequest()
{
    Id = 1,
    Number = 1,
    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
});

Console.WriteLine(response.IsPrime);

// See https://aka.ms/new-console-template for more information
using GrpcClient;

/* 
1- consistently send 10,000 requests/second
4- verify 10,000 responses if response is missing, take appropriate action.(probably resend)
5- display the Round Trip Time (RTT) for each sent message on the client side.
 */

PrimeCheckerClient client = new();
RequestsManager requestManager = new();

try
{
    _ = Task.Run(() => requestManager.QueueRequests());
    _ = Task.Run(async () => await client.SendRequests()); 
    _ = Task.Run(async() => await client.RecieveResponse());

    requestManager.ProcessRetries();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.ReadLine();


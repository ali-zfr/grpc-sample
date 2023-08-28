# grpc-sample
This is a client server application in .net that uses grpc for communication.

## Steps of the development process, from client to server including thinking process and code evolution during the development.
1. Created a POC of grpc client server application, where client sends name to server and server responds back with Wellcome message
2. During my R&D, with the goal in mind to continuously send & recieve messages between client and server I found out that grpc supports bidirection streaming that would help me accomplish continuous communication
3. Created repo on Git and setting up project structure adding git ignore and makefile
4. Created proto file for bidirectional streaming
5. Implemented server side
   - Verify if the sent number (which will be a random number between 1 and 1000) is prime or not
   - respond client with the bool flag if the number is prime or not
6. Implemented client side
   - Send requests in a nonstop loop to server and recieve server response
   - It worked but since sending and recieving response from server was executing in one thread it didn't work as expected
   - So I created 2 thread, one with the responsibility of sending requests and second one to recieve reponse from server
   - After that client was sending messages and response as expected so, I move to next task
7. Now I had to show messages count and top 10 prime numbers
   - So I create a thread safe queue where I queued a custom messages(request id, isPrime flag and the number request by client)
   - Then I created a PrimeStatsService class where I will process those queued messages and it implements IHostedService execute long-running background services
   - There I placed a method that processed messages and displayed stats in a infinite loop
   - On executing I found out I wasn't working as expected, because I was dequeuing messages after 1 second as I had display stats after one second so I moved process numbers and process stats in saperate methods and then assigned both of them saperate threads and it worked out exceptionally well
8. Now I had to implement retries and round trip time on client side
   - In order to check RTT I needed to keep track of messages sent to server for that I created a RequestInfo class that contains request and stopwatch that would start when sending the request and I stored its instances to a thread safe dictionary
   - On recieving response I got RequestInfo back from server I removed its object from the dictionary, stoped the stopwatch and displayed the RTT in milliseconds
   - But the issue was I had only one method that was sending requests how would I resend requests?
   - In order to address that issue, rather than sending directly messages inside send message method I created a thread safe queue in which I will add new messages to send and messages to retry
   - Now send message method deques messages from queue and sends requests to the server
   - Then I created a saperate thread where I will check for messages in dictionary where message retry interval i.e 15secs have been passed, exponentially grow their retry interval and queue them again
  

## Scaling application to hundreds of servers
1. Create another microservice for PrimeStatsService
2. Horizontal Autoscaling Primechecker service
3. Using Load balancers: Place a load balancer in front of your gRPC servers to distribute incoming requests across multiple instances of your service.
4. Using Azure CDN: Using Azure CDN clients can request servers that are located nearest to them

## Testing server-side code with integration testing
1. Basic Functionality: Test the basic "happy path" where everything is supposed to work.
2. Edge Cases: Test with invalid input, extremely large/small numbers, or other unusual situations to ensure your server can handle them like stop client forcefully.
3. Concurrency: Test with multiple clients making simultaneous requests to make sure the server can handle it.

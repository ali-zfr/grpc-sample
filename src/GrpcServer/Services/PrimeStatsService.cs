namespace GrpcServer.Services
{
    public class PrimeStatsService : IHostedService
    {
        private List<long> _maxValidPrimes = new List<long>(10);
        private long _totalMessagesReceived = 0;
        private long _lastTotalMessagesRecieved = 0;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(() => ProcessNumbers());

            _ = Task.Run(() => DisplayStatistics());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("PrimeStatisticsManager is stopping.");

            return Task.CompletedTask;
        }

        public void ProcessNumbers()
        {
            try
            {
                while (true)
                {
                    if (SharedQueue.NumbersQueue.TryDequeue(out EventPayload? result))
                    {
                        ArgumentNullException.ThrowIfNull(result);

                        _totalMessagesReceived++;

                        // maintain top 10 highest requested/validated prime numbers
                        if (result.IsPrime)
                        {
                            if (_maxValidPrimes.Count < 10) _maxValidPrimes.Add(result.Number);
                            else if (!_maxValidPrimes.Contains(result.Number))
                            {
                                long minNumber = _maxValidPrimes.Min();

                                // If the new number is larger, replace the smallest number with the new number
                                if (result.Number > minNumber)
                                {
                                    int index = _maxValidPrimes.IndexOf(minNumber);
                                    _maxValidPrimes[index] = result.Number;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in ProcessNumbers: {ex.Message}");
            }
        }


        private void DisplayStatistics()
        {
            while (true)
            {
                try
                {
                    _maxValidPrimes.Sort();
                    Console.WriteLine($"Total Messages Received: {_totalMessagesReceived}");
                    Console.WriteLine($"Delta: {_totalMessagesReceived-_lastTotalMessagesRecieved}");
                    Console.WriteLine($"Max Valid Primes: {string.Join(", ", _maxValidPrimes)}\n");

                    _lastTotalMessagesRecieved = _totalMessagesReceived;
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred in DisplayStatistics: {ex.Message}");
                }
            }
        }
    }
}

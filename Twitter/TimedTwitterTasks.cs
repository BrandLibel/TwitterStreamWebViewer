using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace TwitterStreamWebViewer.Twitter
{
    public class TimedTwitterTasks : IHostedService, IDisposable
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private int executionCount = 0;
        private readonly ILogger<TimedTwitterTasks> _logger;
        private Timer _timer;
        private static bool IsRunning = false;
        public TimedTwitterTasks(ILogger<TimedTwitterTasks> logger)
        {
            _logger = logger;
            var c = new CancellationToken();
            this.StartAsync(c);
        }

        private class NewFollower
        {
            public long followID { get; set; }
            public bool? result { get; set; }
        }

         static List<NewFollower> newfollwersList = new List<NewFollower>();
            public Task StartAsync(CancellationToken stoppingToken)
            {


                _logger.LogInformation("Timed Hosted Service running.");

                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(10));

                return Task.CompletedTask;
            }

            private async void DoWork(object state)
            {
                var count = Interlocked.Increment(ref executionCount);
                if (IsRunning == true) return;
                IsRunning = true;
                
                var t = new Task(() =>
                {
                    var twitterFriends = new TwitterFriends();
                    
                    if(newfollwersList.Where(f => f.result == null).Count() == 0)
                    {
                        var newFriends = twitterFriends.GetNewFriends();
                        var existingFriends = newfollwersList.Select(f => f.followID).ToList();

                        foreach (var newFriend in newFriends.Except(existingFriends))
                        {
                            newfollwersList.Add(new NewFollower { followID = newFriend });

                        }
                    }
                    else
                    {
                        _logger.LogInformation("Need to Add " + newfollwersList.Where(f => f.result == null).Count());

                        NewFollower addFollwer = newfollwersList.Where(f => f.result == null).First();

                        try
                        {
                            twitterFriends.AddFollow(addFollwer.followID);
                            addFollwer.result = true;
                            _logger.LogInformation($"Added {addFollwer.followID}");
                        }
                        catch
                        {
                            _logger.LogInformation($"ERROR {addFollwer.followID}");
                            addFollwer.result = false;
                        }
                    }

                    IsRunning = false;
                    
                    


                });
                t.Start();

                await t;

                _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
            }

            public Task StopAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");

                _timer?.Change(Timeout.Infinite, 0);

                return Task.CompletedTask;
            }

            public void Dispose()
            {
                _timer?.Dispose();
            }

            
        }
}

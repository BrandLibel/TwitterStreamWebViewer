using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterStreamWebViewer.Twitter
{
    public class TwitterFriends
    {
        public List<long> GetNewFriends()
        {
            Tweetinvi.ExceptionHandler.SwallowWebExceptions = false;
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            var credentials = Tweetinvi.Auth.SetUserCredentials(
                    "PJp6NbNEQEjzMvGgoKST1gF9A",// "YOUR CONSUMER KEY",
                    "CPRfUjwUHvlxJFAc7JysFda9JfutcwuPWry6OyeQndGrRD0Myr",//"YOUR CONSUMER SECRET",
                    "1171874133094936576-VTrhR9sqfwE3eYlEo3RWO4tz2Yi8ej",//YOUR USER ACCESS TOKEN",
                    "WK3naR5sOC6sHhZ01NwDpf1PTsVUckv2LWH2bLKYVE5I5"// "YOUR USER ACCESS SECRET");
            );

            var user = Tweetinvi.User.GetAuthenticatedUser();
            var followerIDs = user.GetFollowerIds();
            var friendsIDs = user.GetFriendIds();
            // .
            //var nonfriendIDs = friendsIDs.Except(followerIDs).ToList();
            var newFriendIDs = followerIDs.Except(friendsIDs).ToList();
            return newFriendIDs;
        }

        public bool AddFollow(long userToFollowID)
        {           

            try
            {
                Tweetinvi.ExceptionHandler.SwallowWebExceptions = false;
                RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
                var credentials = Tweetinvi.Auth.SetUserCredentials(
                        "PJp6NbNEQEjzMvGgoKST1gF9A",// "YOUR CONSUMER KEY",
                        "CPRfUjwUHvlxJFAc7JysFda9JfutcwuPWry6OyeQndGrRD0Myr",//"YOUR CONSUMER SECRET",
                        "1171874133094936576-VTrhR9sqfwE3eYlEo3RWO4tz2Yi8ej",//YOUR USER ACCESS TOKEN",
                        "WK3naR5sOC6sHhZ01NwDpf1PTsVUckv2LWH2bLKYVE5I5"// "YOUR USER ACCESS SECRET");
                );
                var user = Tweetinvi.User.GetAuthenticatedUser();
                var ret = user.FollowUser(userToFollowID);
                return ret;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void RemoveFollowOnlyFriends()
        {
            Tweetinvi.ExceptionHandler.SwallowWebExceptions = false;
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            var credentials = Tweetinvi.Auth.SetUserCredentials(
                    "PJp6NbNEQEjzMvGgoKST1gF9A",// "YOUR CONSUMER KEY",
                    "CPRfUjwUHvlxJFAc7JysFda9JfutcwuPWry6OyeQndGrRD0Myr",//"YOUR CONSUMER SECRET",
                    "1171874133094936576-VTrhR9sqfwE3eYlEo3RWO4tz2Yi8ej",//YOUR USER ACCESS TOKEN",
                    "WK3naR5sOC6sHhZ01NwDpf1PTsVUckv2LWH2bLKYVE5I5"// "YOUR USER ACCESS SECRET");
            );

            var user = Tweetinvi.User.GetAuthenticatedUser();
            var followerIDs = user.GetFollowerIds();
            var friendsIDs = user.GetFriendIds();
            // .
            //var nonfriendIDs = friendsIDs.Except(followerIDs).ToList();
            var newFriendIDs = followerIDs.Except(friendsIDs).ToList();

            // var x = 0;
            //Console.WriteLine($"Removing {nonfriendIDs.Count()}");
            //foreach(var nonfriendID in nonfriendIDs)
            //{
            //    Task.Delay(500).Wait();
            //    x++;
            //    user.UnFollowUser(nonfriendID);
            //    Console.WriteLine($"Removed {x}");


            //}

            var x = 0;
          
            foreach (var newFriend in newFriendIDs)
            {
                //if (x >= 5) return;
                x++;
                try
                {
                    //Thread.Sleep(250);
                    //var limits = RateLimit.GetCurrentCredentialsRateLimits();
                    //Console.WriteLine(limits.)
                    
                    user.FollowUser(newFriend);
                    Console.WriteLine($"Added {x}");

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                


            }

        }
    }
}

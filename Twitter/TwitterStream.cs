using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using Tweetinvi.Streams;
using TwitterStreamWebViewer.SignalR;
using TwitterStreamWebViewer.Twitter;

namespace TwitterStreamWebViewer
{
    public class TwitterStream
    {

        private readonly IHubContext<SignalRTweetHub> _tweetHub;
        private IFilteredStream _filteredStream;
        public TwitterStream(IHubContext<SignalRTweetHub> tweetHub)
        {
            _tweetHub = tweetHub;
        }
        public void StartStream()
        {
            Tweetinvi.ExceptionHandler.SwallowWebExceptions = false;
            var credentials = Tweetinvi.Auth.SetUserCredentials(
                    TwitterAppCredentials.ConsumerKey,
                    TwitterAppCredentials.ConsumerSecret,
                    TwitterAppCredentials.AccessToken,
                    TwitterAppCredentials.AccessSecret
            );

            var user = Tweetinvi.User.GetUserFromScreenName("AndrewYang");
            _filteredStream = Tweetinvi.Stream.CreateFilteredStream();
            _filteredStream.FilterLevel = Tweetinvi.Streaming.Parameters.StreamFilterLevel.None;
            _filteredStream.TweetMode = Tweetinvi.TweetMode.Extended;
            //_filteredStream.AddLocation(new Location(30.6266, 81.4609, 30.6319, 81.6065));
            _filteredStream.AddFollow(user.Id);
            _filteredStream.AddTrack("YangGang");
            //_filteredStream.AddTrack("DemDebate");
            _filteredStream.AddTrack("StillVotingYang");
            //stream.AddTrack("FreedomDividend");
            //stream.AddTrack("SecureTheBag");
            //stream.AddTrack("YangMediaBlackout");
            //stream.AddTrack("YangMediaShoutout");
            //stream.AddTrack("YangWillWin");
            //stream.AddTrack("YangOrBust");
            //stream.AddTrack("YangBeatsTrump");
            //stream.AddTrack("NewHampshireForYang");
            //stream.AddTrack("AmericaNeedsYang");
            //stream.AddTrack("LetYangSpeak");
            //stream.AddTrack("VoteYang");

            _filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                try
                {


                    var tweet = args.Tweet;
                    if (tweet.IsRetweet) return;

                    using (var db = new TwitterSearchModel())
                    {
                        var savedUser = db.TwitterUsers.Where(u => u.TwitterUserID == tweet.CreatedBy.IdStr).FirstOrDefault();
                        if (savedUser == null)
                        {
                            var user = Tweetinvi.User.GetUserFromId(tweet.CreatedBy.Id);
                            var userJson = Tweetinvi.JsonSerializer.ToJson(user.UserDTO);

                            db.TwitterUsers.Add(new TwitterUser
                            {
                                TwitterUserID = user.IdStr,
                                TwitterUserJson = userJson
                            });
                        }

                        var savedTweet = db.TweetRecords.Where(t => t.TweetID == tweet.IdStr).FirstOrDefault();
                        if (savedTweet == null)
                        {
                            var fullTweet = Tweetinvi.Tweet.GetTweet(tweet.Id);
                            _tweetHub.Clients.All.SendAsync("ReceiveTweet", fullTweet.TweetDTO.ToJson());
                            //var voteYang = Tweetinvi.Timeline.GetUserTimeline(.GetExistingList()
                            var tweetJson = Tweetinvi.JsonSerializer.ToJson(fullTweet.TweetDTO);

                            db.TweetRecords.Add(new TweetRecord
                            {
                                TweetID = tweet.IdStr,
                                TweetJson = tweetJson
                            });

                            Console.WriteLine($"User: {fullTweet.CreatedBy.Name}");
                            Console.WriteLine($"Tweet: {fullTweet.FullText ?? fullTweet.Text}");
                            Console.WriteLine($"{fullTweet.Url}");
                            Console.WriteLine();
                        }

                        db.SaveChanges();



                        //tweetPostQueue.Enqueue(tweet)
                        //var fbPostResult = PostToFacebook(tweet).Result;
                    }
                }
                catch(Exception e)
                {

                }
                finally
                {

                }
            };

            _filteredStream.StartStreamMatchingAnyCondition();


        }

        public StreamState GetStreamStatus()
        {
            try
            {
                return _filteredStream.StreamState;
            }
            catch(Exception e)
            {
                return StreamState.Stop;
            }
            
        }
    }
}

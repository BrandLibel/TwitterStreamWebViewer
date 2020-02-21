using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace TwitterStreamWebViewer.SignalR
{
    public class SignalRTweetHub:Hub
    {
        public async Task SendTweet(Tweetinvi.Models.ITweet tweet)
        {
            await Clients.All.SendAsync("ReceiveTweet", tweet);
        }
    }
}

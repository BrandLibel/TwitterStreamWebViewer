using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tweetinvi.Core.Streaming;
using TwitterStreamWebViewer.Models;
using TwitterStreamWebViewer.SignalR;
using TwitterStreamWebViewer.Twitter;

namespace TwitterStreamWebViewer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<SignalRTweetHub> _tweetHub;
        private readonly TwitterStream _twitterStream;
        public HomeController(ILogger<HomeController> logger, IHubContext<SignalRTweetHub> tweetHub)
        {
            _logger = logger;
            _tweetHub = tweetHub;

           

            if(_twitterStream == null) _twitterStream = new TwitterStream(_tweetHub);

            //if (_twitterStream.GetStreamStatus() != Tweetinvi.Models.StreamState.Running) _twitterStream.StartStream();
            new Task(() => { _twitterStream.StartStream(); }).Start();
        }

        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

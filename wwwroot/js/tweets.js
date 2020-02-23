"use strict";
const tweets = [];
//const tColumns = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
var connection = new signalR.HubConnectionBuilder().withUrl("/signalRTweetHub").configureLogging(signalR.LogLevel.Information).withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();



connection.on("ReceiveTweet", function (tweetDTO, childTweetDTO) {
    var tweet = JSON.parse(tweetDTO);
    var childTweet = JSON.parse(childTweetDTO);
    tweets.unshift(tweet.id);
    console.log(tweet);
    console.log(childTweet);
    var tweetText;
    if (tweet.extended_tweet) { tweetText = tweet.extended_tweet.full_text || tweet.extended_tweet.text; }
    tweetText = tweetText || tweet.full_text || tweet.text;       
    tweetText = tweetText.replace(/((http|https|ftp):\/\/[\w?=&.\/-;#~%-]+(?![\w\s?&.\/;#~%"=-]*>))/g, '<a target="_blank" href="$1">$1</a> ');
    var subTweet = '';
    //var quotedTweet = tweet.quoted_status || tweet.in_reply_to_status;    
    
    if (childTweet)
    {
        var childTweetText;
        if (childTweet.extended_tweet) { childTweetText = childTweet.extended_tweet.full_text || childTweet.extended_tweet.text; }
        childTweetText = childTweetText || childTweet.full_text || childTweet.text;
        childTweetText = childTweetText.replace(/((http|https|ftp):\/\/[\w?=&.\/-;#~%-]+(?![\w\s?&.\/;#~%"=-]*>))/g, '<a target="_blank" href="$1">$1</a> ');
        var quotedVerifiedImg = (childTweet.user.verified) ? `<img height="15" src="/img/twitterverified.png">` : ``;
        
   
        subTweet = `<div class="card p-0 m-1 mb-2 shadow-sm"  >
                   <div class="card-header m-0 p-0" id="heading-${childTweet.id}"><img height="45"  src="${childTweet.user.profile_image_url_https}" class="float-left"><div class="float-left p-1"> ${childTweet.user.screen_name}  ${quotedVerifiedImg}<br>Followers: ${childTweet.user.followers_count}</div><div class="float-right p-1">#<a href="https://twitter.com/${childTweet.user.screen_name}/status/${childTweet.id_str}" target="_blank">${childTweet.id}</a><br><div class="float-right">${new Date(childTweet.created_at).toLocaleString("en-US")}</div></div></div>
                    <div class="card-body m-0 p-1">${childTweetText}</div>
                </div>`;
    }
    var verifiedImg = (tweet.user.verified) ? `<img height="15"  src="/img/twitterverified.png">` : ``;
    //console.log(tweet);
    //tweetID = tweetID.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //tweetUser = tweetUser.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //tweetFullText = tweetFullText.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //tweetedTimestamp = tweetedTimestamp.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //var encodedMsg = user + " says " + msg;
    //var li = document.createElement("li");
    //li.textContent = encodedMsg;
    //const cardBody = document.createElement('div').html(tweetFullText);
    //card.classList = 'card-body';

    // Construct card content
    const content = `
    <div id="${tweet.id}" class="card p-0 m-2 shadow-sm" >
    <div class="card-header m-0 p-0  " id="heading-${tweet.id}">
<img height="45" src="${tweet.user.profile_image_url_https}" class="float-left">
<div class="float-sm-left p-1"><a class="twitter-follow-button" href="https://twitter.com/${tweet.user.screen_name}">${tweet.user.screen_name}</a> ${verifiedImg}<br> Followers: ${tweet.user.followers_count}</div>
<div class="float-sm-right p-1">#<a href="https://twitter.com/${tweet.user.screen_name}/status/${tweet.id_str}" target="_blank">${tweet.id_str}</a>
<br/><span class=float-right">${new Date(tweet.created_at).toLocaleString("en-US")}</span></div></div>
        <div  class="card-body p-1 m-0">${tweetText}<div class="container-fluid"><div class="row"><div class="col-12">${subTweet}</div></div></div>
    </div></div></div>
  `;
    
    //const oTweet = `<div  class="col-sm"><div id="oTweetContainer-${tweet.id_str}" ></div></div>`;
    //$(oTweet).prependTo("#tweetContainer").slideDown(1000).fadeIn(1000, function () {
    //    twttr.widgets.createTweet(tweet.id_str, document.getElementById("oTweetContainer-"+tweet.id_str), { theme: "light",align:"center",width:"250",dnt:"true" });
       
    //    while (tweets.length > 10) {
    //        $("#" + tweets[tweets.length - 1]).fadeOut("slow", function () { $(this).remove(); });
    //        tweets.pop();
    //    }
    //});

    $(content).hide().prependTo("#tweetContainer").slideDown(1000).fadeIn(1000, function () {
        while (tweets.length > 5) {
            $("#" + tweets[tweets.length - 1]).fadeOut("slow", function () { $(this).remove(); });
            tweets.pop();
        }
    });
    //while (tweets.length > 2) {
    //    var t = tweets[tweets.length - 1];
    //    console.log(tweets.length);
    //    $(`#${tweets[tweets.length - 1]}`).fadeOut("slow");
        
    //    tweets.pop();
    //}
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});


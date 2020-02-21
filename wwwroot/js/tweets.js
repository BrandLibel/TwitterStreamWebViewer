"use strict";
const tweets = [];

var connection = new signalR.HubConnectionBuilder().withUrl("/signalRTweetHub").configureLogging(signalR.LogLevel.Information).withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol()).build();



connection.on("ReceiveTweet", function (tweetDTO) {
    var tweet = JSON.parse(tweetDTO);
    tweets.unshift(tweet.id);
    console.log(tweets);
    var tweetText;
    if (tweet.extended_tweet) { tweetText = tweet.extended_tweet.full_text || tweet.extended_tweet.text; }
    tweetText = tweetText|| tweet.full_text || tweet.text;    
    var subTweet = '';
    var quotedTweet = tweet.quoted_status || tweet.in_reply_to_status;    
    
    if (quotedTweet)
    {
        var quotedTweetText;
        if (quotedTweet.extended_tweet) { quotedTweetText = quotedTweet.extended_tweet.full_text || quotedTweet.extended_tweet.text; }
        quotedTweetText = quotedTweetText|| quotedTweet.full_text || quotedTweet.text;
        var quotedVerifiedImg = (quotedTweet.user.verified) ? `<img src="/img/twitterverified.png">` : ``;
        
   
        subTweet = `<div class="card p-1" style="bg-color:#cccccc;" >
                   <div class="card-header m-0 p-0" id="heading-${quotedTweet.id}"><img src="${quotedTweet.user.profile_image_url_https}" class="float-left"><small class="float-sm-left"> ${quotedTweet.user.screen_name}  ${quotedVerifiedImg}<br> Followers: ${quotedTweet.user.followers_count}</small><small class="float-sm-right">#<a href="https://twitter.com/${quotedTweet.user.screen_name}/status/${quotedTweet.id}" target="_blank">${quotedTweet.id}</a><br/>${quotedTweet.created_at}</small></div>
                    <div class="card-body m-1 p-1"><small>${quotedTweetText}</small></div>
                </div>`;
    }
    var verifiedImg = (tweet.user.verified) ? `<img src="/img/twitterverified.png">` : ``;
    console.log(tweet);
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
    <div id="${tweet.id}" class="card p-1" >
    <div class="card-header m-0 p-0" id="heading-${tweet.id}">
<img src="${tweet.user.profile_image_url_https}" class="float-left">
<small class="float-sm-left"> ${tweet.user.screen_name}  ${verifiedImg}<br> Followers: ${tweet.user.followers_count}</small>
<small class="float-sm-right">#<a href="https://twitter.com/${tweet.user.screen_name}/status/${tweet.id_str}" target="_blank">${tweet.id_str}</a>
<br/>${tweet.created_at}</small></div>
        <div class="card-body p-0 m-0"><small>${tweetText}</small><div class="container-fluid"><div class="row"><div class="col-12">${subTweet}</div></div></div>
    </div></div></div>
  `;
    

    
    $(content).hide().prependTo("#tweetContainer").slideDown(1000).fadeIn(1000, function () {
        while (tweets.length > 25) {
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


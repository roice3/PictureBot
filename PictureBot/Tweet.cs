namespace PictureBot
{
	using LinqToTwitter;
	using System;
	using System.IO;
	using System.Threading.Tasks;

	public class Tweet
	{
		// Move these to a separate txt file excluded from .gitignore
		private static string ConsumerKey = string.Empty;
		private static string ConsumerKeySecret = string.Empty;
		private static string AccessToken = string.Empty;
		private static string AccessTokenSecret = string.Empty;

		public static void ReadKeys( Settings settings )
		{
			string[] keys = File.ReadAllLines( settings.KeysFile );
			ConsumerKey = keys[0];
			ConsumerKeySecret = keys[1];
			AccessToken = keys[2];
			AccessTokenSecret = keys[3];
		}

		public static TwitterContext TwitterContext()
		{
			var auth = new SingleUserAuthorizer
			{
				CredentialStore = new SingleUserInMemoryCredentialStore
				{
					ConsumerKey = ConsumerKey,
					ConsumerSecret = ConsumerKeySecret,
					AccessToken = AccessToken,
					AccessTokenSecret = AccessTokenSecret
				}
			};
			var twitterCtx = new TwitterContext( auth );
			return twitterCtx;
		}

		// https://github.com/JoeMayo/LinqToTwitter/wiki/Tweeting-with-Media
		public static async Task Send( string status, string imagePath )
		{
			TwitterContext twitterCtx = TwitterContext();

			System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

			Media media = await twitterCtx.UploadMediaAsync( File.ReadAllBytes( imagePath ), "image/png", "tweet_image" );
			Status tweet = await twitterCtx.TweetAsync( status, new ulong[] { media.MediaID } );
			if( tweet != null )
				Console.WriteLine( $"Tweet sent: {tweet.Text}" );
		}

		public static async Task Reply( ulong tweetID, string status, string imagePath )
		{
			TwitterContext twitterCtx = TwitterContext();

			System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

			Media media = await twitterCtx.UploadMediaAsync( File.ReadAllBytes( imagePath ), "image/png", "tweet_image" );
			Status tweet = await twitterCtx.ReplyAsync( tweetID, status, new ulong[] { media.MediaID } );
			if( tweet != null )
				Console.WriteLine( $"Reply sent: {tweet.Text}" );
		}
	}
}

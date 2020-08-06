namespace PictureBot
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	class Program
	{
		static void Main( string[] args )
		{
			try
			{
				bool tweet = false;
				if( args.Length == 1 )
				{
					string arg = args[0];
					if( arg == "-Tweet" )
					{
						tweet = true;
					}
					else if( arg == "-TestQueue" )
					{
						TestCurrentQueue();
						return;
					}
					else if( arg == "-MakeTemplate" )
					{
						Settings settings = new Settings(); ;
						string settingsFileName = "settings.xml";
						settings = Persistence.LoadSettings( settingsFileName );
						var images = GetImages( settings );
						TemplateTweetStringsFileFromImages( settings, images );
					}
				}

				BotWork( tweet );
			}
			catch( Exception e )
			{
				Console.WriteLine( "PictureBot malfunction! " + e.Message + "\n" + e.StackTrace );
				throw;
			}
		}

		static void BotWork( bool tweet )
		{
			Settings settings = new Settings(); ;
			string settingsFileName = "settings.xml";
			settings = Persistence.LoadSettings( settingsFileName );

			var images = GetImages( settings );
			var tweets = Persistence.GetTweets( settings );

			string image = NextInQueue( settings, images, tweets );
			string tweetString;
			if( !tweets.TryGetValue( image, out tweetString ) )
				tweetString = "";
			string imagePath = images[image];

			if( tweet )
			{
				System.Console.WriteLine( "Attempting tweet." );
				Tweet.ReadKeys( settings );
				Tweet.Send( tweetString, imagePath ).Wait();
			}

			if( settings.Archive )
				Archive( settings );
		}

		/// <summary>
		/// Image names (keys) and full paths (values).
		/// </summary>
		static Dictionary<string, string> GetImages( Settings settings )
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			string[] extensions = new string[] { "*.png", "*.gif", "*.jpg", "*.jpeg" };
			foreach( string ext in extensions )
			foreach( string file in Directory.GetFiles( settings.ImageDirectory, ext, SearchOption.AllDirectories ) )
			{
				result[Path.GetFileNameWithoutExtension( file )] = file;
			}

			return result;
		}

		public static string NextInQueue( Settings settings, Dictionary<string, string> images, Dictionary<string, string> tweets )
		{
			// We prefer to work off the tweet file, but if it has no entries,
			// we'll use images directly.

			if( tweets.Count == 0 )
			{
				if( settings.RandomizeOrder )
					return RandomItem( images );
				return images.First().Key;
			}

			if( settings.RandomizeOrder )
				return RandomItem( tweets );
			return tweets.First().Key;
		}

		public static string RandomItem( Dictionary<string, string> all )
		{
			System.Random rand = new System.Random();
			int idx = rand.Next( 0, all.Count );
			return all.ElementAt( idx ).Key;
		}


		public static void Archive( Settings settings )
		{
			throw new System.NotImplementedException();
		}

		public static void Move( string filename, string from, string to )
		{
			File.Move(
				Path.Combine( from, filename ),
				Path.Combine( to, filename ) );
		}

		static void TestCurrentQueue()
		{
			
		}

		static void TemplateTweetStringsFileFromImages( Settings settings, Dictionary<string,string> images )
		{
			for( int i=0; i<images.Count; i++ )
			{
				string key = images.ElementAt( i ).Key;
				string desc = key;
				desc = desc.Replace( 'i', '∞' );
				desc = "Regular " + desc;
				images[key] = desc;
			}

			Persistence.WriteTweets( settings, images );
		}
	}
}

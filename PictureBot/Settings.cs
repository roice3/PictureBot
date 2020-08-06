namespace PictureBot
{
	using System.Runtime.Serialization;

	[DataContract( Namespace = "" )]
	public class Settings
	{
		public Settings()
		{
			SetDefaults();
		}

		[OnDeserializing]
		private void OnDeserializing( StreamingContext context )
		{
			SetDefaults();
		}

		private void SetDefaults()
		{
			KeysFile = @"keys.txt";
			ImageDirectory = @"./images";
			TweetStringFile = @"tweetStrings.txt";
			RandomizeOrder = true;
			Archive = false;
		}

		/// <summary>
		/// A file with the keys needed to interact with the Twitter API.
		/// </summary>
		[DataMember]
		public string KeysFile;

		/// <summary>
		/// A directory with all images to be tweeted.
		/// This directory will be enumerated recursively.
		/// NOTE: Duplicate image names will be ignored.
		/// </summary>
		[DataMember]
		public string ImageDirectory;

		/// <summary>
		/// A tab-delimited file with 2 columns.
		/// The first column is an image name (not the full path). The second column is the tweet string to use for the image.
		/// If no entry exists in this file for an image, no tweet string will be used.
		/// </summary>
		[DataMember]
		public string TweetStringFile;

		/// <summary>
		/// If true, we'll randomly pick an image.
		/// If false, we'll use the order of rows in the tweet string file.
		/// </summary>
		[DataMember]
		public bool RandomizeOrder;

		/// <summary>
		/// If true, the image will be moved to an archive directory
		/// and the entry removed from the tweet string file.
		/// This is to allow avoiding duplicate posts.
		/// </summary>
		[DataMember]
		public bool Archive;
	}
}

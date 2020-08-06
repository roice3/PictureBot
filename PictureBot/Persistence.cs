namespace PictureBot
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Xml;

	public class Persistence
	{
		/// <summary>
		/// An in-memory, parsed version of the tweet string file.
		/// Keys are image name. Values are the tweet strings.
		/// </summary>
		public static Dictionary<string, string> GetTweets( Settings settings )
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			if( !File.Exists( settings.TweetStringFile ) )
				return result;

			foreach( string line in File.ReadAllLines( settings.TweetStringFile ) )
			{
				var kvp = ReadOneEntry( line );
				result[kvp.Key] = kvp.Value;
			}

			return result;
		}

		public static void WriteTweets( Settings settings, Dictionary<string,string> tweets )
		{
			File.WriteAllLines( settings.TweetStringFile, tweets.Select( kvp => FormatEntry( kvp ) ) );
		}

		public static KeyValuePair<string,string> ReadOneEntry( string line )
		{
			string[] split = line.Split( new char[] { '\t' } );
			if( split.Length != 2 )
				throw new System.ArgumentException();

			return new KeyValuePair<string, string>( split[0], split[1] );
		}

		public static string FormatEntry( KeyValuePair<string, string> item )
		{
			return string.Format( "{0}\t{1}", item.Key, item.Value );
		}

		public static void SaveSettings( Settings settings, string path )
		{
			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.OmitXmlDeclaration = true;
			writerSettings.Indent = true;
			using( var writer = XmlWriter.Create( path, writerSettings ) )
			{
				DataContractSerializer dcs = new DataContractSerializer( settings.GetType() );
				dcs.WriteObject( writer, settings );
			}
		}

		public static Settings LoadSettings( string path )
		{
			XmlReaderSettings readingSettings = new XmlReaderSettings();
			readingSettings.IgnoreWhitespace = true;
			using( var reader = XmlReader.Create( path, readingSettings ) )
			{
				DataContractSerializer dcs = new DataContractSerializer( typeof( Settings ) );
				return (Settings)dcs.ReadObject( reader, verifyObjectName: false );
			}
		}
	}
}

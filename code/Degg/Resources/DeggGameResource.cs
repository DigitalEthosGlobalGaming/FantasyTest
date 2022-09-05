using Sandbox;
using System.Collections.Generic;

namespace Degg
{
	public class DeggGameResource: GameResource
	{
		public static List<T> GetAll<T>() where T: GameResource
		{
			var resources = ResourceLibrary.GetAll<T>();
			var results = new List<T>();
			foreach ( var item in resources )
			{
				results.Add( item );
			}

			return results;
		}

		public static T GetRandom<T>() where T: GameResource
		{
			var resources = GetAll<T>();
			return Rand.FromList( resources );
		}

		protected override void PostReload()
		{
			base.PostReload();
			var extension = GetExtension();
			Event.Run( "Resource.Reload", this);
			var fullName = $"Resource.Reload.{extension}";

			Event.Run( fullName, this );

		}



		public string GetExtension()
		{
			var path = ResourcePath;
			var parts = path.Split( "." );
			return parts[parts.Length - 1];

		}

		public static T Get<T>(string t) where T : GameResource
		{
			var resource = ResourceLibrary.Get<T>( t );

			if ( resource  == null)
			{
				Log.Error($"Unable to load resource {t}");
			}

			return resource;
		}
	}
}

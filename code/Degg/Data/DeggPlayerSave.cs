using Degg.Core;
using Degg.Websocket;
using Sandbox;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Degg.Data
{
	public partial class DeggPlayerSave: BaseNetworkable
	{
		[JsonIgnore]
		public DeggPlayer Player { get; set; }

		[JsonIgnore]
		public bool Dirty { get; set; }

		[Net]
		public string Name { get; set; } = "";


		public string Serialize()
		{
			var data =  Serialize( this );
			return data;
		}
		public static string Serialize( DeggPlayerSave save)
		{
			var options = new JsonSerializerOptions
			{
				IgnoreReadOnlyProperties = true
			};

			return JsonSerializer.Serialize( save,save.GetType(), options );
		}

		public static DeggPlayerSave Deserialize(string data)
		{
			return Deserialize<DeggPlayerSave>( data );
		}
		public static T Deserialize<T>( string data ) where T: DeggPlayerSave
		{
			var options = new JsonSerializerOptions
			{
				IgnoreReadOnlyProperties = true,
				NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
				UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode
			};
			return JsonSerializer.Deserialize<T>(data, options );
		}

		public void Save()
		{
			Save( this );
		}

		public static void Save( DeggPlayerSave p )
		{
			Save( p.Player, p.Name, p.Serialize() );
			p.Dirty = false;
		}

		public static void Save( DeggPlayer p, string name, string data )
		{
			Save( p.Client, name, data );
		}

		public static void Save( Client client, string name, string data )
		{
			new PlayerDataSetEvent( client, $"saves.{name}", data );
		}
		public static void Load<T>( DeggPlayer player, string name, Action<T> callback ) where T : DeggPlayerSave
		{
			Load<T>( player.Client, name, callback );
		}

		public static void Load<T>( Client c, string name, Action<T> callback ) where T : DeggPlayerSave
		{
			var _ = new PlayerDataGetEvent( c, $"saves.{name}", ( DeggSocketEvent e ) =>
			{
				try
				{
					var item = e.ToObject<T>();
					callback( item );
				}
				catch ( Exception ex )
				{
					Log.Info( ex );
					callback( null );
				}
			} );
		}

	}
}

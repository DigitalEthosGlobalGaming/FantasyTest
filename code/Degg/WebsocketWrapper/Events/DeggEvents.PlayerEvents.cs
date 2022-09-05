using Degg.Core;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Degg.Websocket
{
	public class PlayerJoinEvent
	{
		[JsonPropertyName( "name" )]
		public string Name { get; init; }
		[JsonPropertyName( "playerId" )]
		public string PlayerId { get; set; }
		public PlayerJoinEvent( Client cl )
		{
			Name = cl.Name;
			PlayerId = cl.PlayerId.ToString();
			if ( DeggSocket.Current != null )
			{
				DeggSocket.Current.Event( "Player.Join", this );
			}
		}
	}

	public class PlayerLeaveEvent
	{
		[JsonPropertyName( "name" )]
		string Name { get; init; }
		[JsonPropertyName( "playerId" )]
		public string PlayerId { get; set; }
		public PlayerLeaveEvent( Client cl )
		{
			Name = cl.Name;
			PlayerId = cl.PlayerId.ToString();
			if ( DeggSocket.Current != null )
			{
				DeggSocket.Current.Event( "Player.Leave", this );
			}
		}
	}

	public class PlayerDataSetEvent
	{
		[JsonPropertyName( "playerId" )]
		public string PlayerId { get; set; }
		[JsonPropertyName( "key" )]
		public string Key { get; set; }
		[JsonPropertyName( "value" )]
		public object Value { get; set; }
		public PlayerDataSetEvent(Client cl, string key, object data )
		{
			Value = data;
			Key = key;
			PlayerId = cl.PlayerId.ToString();
			DeggSocket.Current.Event( "player.set", this );
		}
	}

	public class PlayerDataGetEvent
	{
		[JsonPropertyName( "playerId" )]
		public string PlayerId { get; set; }
		[JsonPropertyName( "key" )]
		public string Key { get; set; }
		public PlayerDataGetEvent( Client cl, string key, Action<DeggSocketEvent> callback )
		{
			Key = key;
			PlayerId = cl.PlayerId.ToString();
			DeggSocket.Current.Event( "player.get", this, callback );
		}
	}

	public class PlayerSyncEvent
	{
		[JsonPropertyName( "playerId" )]
		public string PlayerId { get; set; }
		[JsonPropertyName( "codes" )]
		public string[] Codes { get; set; }
		public PlayerSyncEvent( Client cl, string[] code, Action<DeggSocketEvent> callback )
		{
			var codes = new List<string>( code );
			codes.Add( "DeggPoints" );
			codes.Add( "DeggItems" );
			code = codes.ToArray();
			Codes = code;
			PlayerId = cl.PlayerId.ToString();
			DeggSocket.Current.Event( "player.sync", this, (DeggSocketEvent e) =>
			{
				Dictionary<string, JsonElement> data = e.GetData<Dictionary<string, JsonElement>>();

				if (cl?.Pawn?.IsValid() ?? false)
				{
					if ( cl.Pawn is DeggPlayer pawn && data != null )
					{						
						foreach ( var item in data )
						{
							pawn.SetData( item.Key, DeggJsonHelpers.GetJsonElementValue(item.Value), false );
						}
					}
				}

				if ( callback  != null)
				{
					callback(e);
				}
			} );
		}
	}
}

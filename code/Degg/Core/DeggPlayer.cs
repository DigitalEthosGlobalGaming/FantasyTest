using Degg.Data;
using Degg.Util;
using Degg.Websocket;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Degg.Core
{
	public partial class DeggPlayer: Player
	{

		[Net]
		public DeggPlayerSave SaveData { get; set; }


		[Net]
		public bool DisabledControls { get; set; }

		[BindComponent] public CameraMode Camera { get; set; }

		[ConCmd.Server]
		public static void SetControlsDisabledCmd( bool value )
		{
			var player = ClientUtil.GetCallingPawn<DeggPlayer>();
			player.DisabledControls = value;
		}

		public void SetControlsDisabled( bool value )
		{
			if (IsClient)
			{
				SetControlsDisabledCmd( value );
			} else
			{
				DisabledControls = value;
			}
		}


		public override void Spawn()
		{
			base.Spawn();
			Transmit = TransmitType.Owner;
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();
			if ( Hud == null )
			{
				HudSetup();
			}
		}

		[Event.Tick]
		public virtual void Tick()
		{
			if (IsServer)
			{
				ServerTick();
			}
			if (IsClient)
			{
				ClientTick();
			}
		}

		public virtual void ServerTick()
		{

		}

		public virtual void ClientTick()
		{

		}

		public void OnSaveDataChanged(DeggPlayerSave prev, DeggPlayerSave next)
		{

		}
		public List<string[]> SocketSubscriptions { get; set; }
		public string GetPlayerId()
		{
			return Client?.PlayerId.ToString() ?? "";
		}

		public void InitSocketSubscriptions()
		{
			if (SocketSubscriptions == null)
			{
				SocketSubscriptions = new List<string[]>();
			}
		}

		public void SocketSubscribe(string topic, Action<DeggSocketEvent> callback)
		{
			InitSocketSubscriptions();
			var playerId = GetPlayerId();
			if ( playerId != "" ) {
				var token = DeggSocket.Current.Subscribe( $"player.{playerId}.{topic}", callback );
				SocketSubscriptions.Add( token );
			}
		}

		public void SocketUnSubscribeAll()
		{
			InitSocketSubscriptions();
			foreach (var i in SocketSubscriptions )
			{
				SocketUnSubscribe( i );
			}
		}
		public void SocketUnSubscribe( string topic, string id )
		{
			SocketUnSubscribe( new string[] { topic, id } );
		}

		public void SocketUnSubscribe( string[] key)
		{
			if (key.Length != 2)
			{
				return;
			}
			DeggSocket.Current.Unsubscribe( key );
			SocketSubscriptions.Remove( key );
		}
		public void SetDataFromDeggEvent(DeggSocketEvent e)
		{
			Dictionary<string, JsonElement> data = e.GetData<Dictionary<string, JsonElement>>();

			foreach ( var item in data )
			{
				this.SetData( item.Key, item.Value.ToString(), false );
			}
		}
		public void SetData( string key, object value, bool sync = true )
		{
			if (value is JsonElement)
			{
				value = JsonSerializer.Serialize( value );
			}

			if ( value is int i )
			{
				value = (float) i;
			}

			if ( sync && GetData<object>( key ) != value)
			{
				new PlayerDataSetEvent( Client, key, value );
			}

			Client.SetValue( key, value );
		}

		public T GetData<T>( string key, T def = default( T ) )
		{
			return Client.GetValue( key, def );
		}

		public T GetSaveData<T>() where T: DeggPlayerSave
		{
			if (SaveData is T save)
			{
				return save;
			}
			return null;
		}

		public void Load<T>(string name, Action<T> callback = null) where T: DeggPlayerSave, new()
		{
			DeggPlayerSave.Load( this, name, (T save) =>
			{
				if ( save  == null)
				{
					save = new T();
				}
				save.Player = this;
				save.Name = name;
				SaveData = save;
				if (callback != null)
				{
					callback( save );
				}
			} );
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				if ( SaveData != null )
				{
					SaveData.Save();
				}
			} else
			{
				DeleteHud();
			}
		}

		public static void LoadAndCreate<PlayerType,SaveType>(Client c, string name, Action<PlayerType> callback)
			where PlayerType: DeggPlayer, new()
			where SaveType: DeggPlayerSave, new()
		{
			var player = new PlayerType();
			c.Pawn = player;

			player.Load( name, (SaveType t) =>
			 {
				 if ( t == null )
				 {
					 t = new SaveType();
				 }
				 callback( player );
			 } );

		}

		public T SetCamera<T>() where T : CameraMode, new()
		{
			if ( Camera != null )
			{
				Components.Remove( Camera );
			}

			Camera = Components.Create<T>();
			return (T)Camera;
		}

		public T GetCamera<T>() where T : CameraMode
		{
			if ( CameraMode is T )
			{
				return (T)CameraMode;
			}
			return null;
		}

		public static List<T> GetAllPlayers<T>() where T: DeggPlayer
		{
			return All.OfType<T>().ToList<T>();
		}
		public static T GetRandomPlayer<T>() where T : DeggPlayer
		{
			var players = All.OfType<T>().ToList<T>();

			return Rand.FromList( players );
		}

		public static KeyValuePair<float, T> GetClosestPlayer<T>(Vector3 position) where T : DeggPlayer
		{
			T closest = null;
			var lowestDistance = float.MaxValue;
			var players = GetAllPlayers<T>();
			foreach ( var item in players)
			{
				var distance = item.Position.Distance( position );
				if (distance < lowestDistance)
				{
					lowestDistance = distance;
					closest = item;
				}
			}

			return new KeyValuePair<float, T>( lowestDistance, closest );
		}
	}
}

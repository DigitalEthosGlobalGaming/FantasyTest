
using Degg.Util;
using Degg.Utils;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Degg.Websocket
{
	public partial class DeggSocket
	{

		public static DeggSocket Current { get; set; }
		public static Dictionary<string,DeggSocketClient> Clients {get;set;}
		public Dictionary<string, Action<DeggSocketEvent>> Callbacks { get; set; }

		public string Token { get; set; }

		public bool IsLoggedIn { get; set; }

		public WebSocket Connection { get; set; }


		public delegate void ConnectionChangeHandler();
		public string Uri { get; set; }

		// public event ConnectionChangeHandler OnConnectionChange;

		public DeggSocket(string uri)
		{
			Current = this;
			Uri = uri;
			Callbacks = new Dictionary<string, Action<DeggSocketEvent>>();
		}

		public bool IsConnected()
		{
			return Connection?.IsConnected ?? false;
		}

		public Dictionary<string,string> SetCredentials( string username, string password )
		{
			var credentials = new Dictionary<string, string>();
			credentials["username"] = username;
			credentials["password"] = password;
			FileSystem.Data.WriteJson( "degg-cred.json", credentials );
			return credentials;
		}
		public Dictionary<string, string> GetCredentials()
		{
			return FileSystem.Data.ReadJson<Dictionary<string,string>>( "degg-cred.json" );
		}


		public void Login( Action<DeggSocketEvent> callback = null )
		{
			var credentials = GetCredentials();
			AdvLog.Info( credentials["username"], credentials["password"] );
			Event( "login", credentials, callback );
		}

		public void Event(string typeName, object data, Action<DeggSocketEvent> callback = null)
		{
			if (!Connection?.IsConnected ?? false)
			{
				return;
			}
			var e = new DeggSocketEvent();

			e.Type = typeName;
			e.SetData(data);

			if ( callback != null ) {
				e.CallbackId = Guid.NewGuid().ToString();
				Callbacks[e.CallbackId] = callback;
			}

			var dataString = e.Serialise();

			SendMessage( dataString );
		}

		public void AddClient()
		{

		}

		private void OnConnect( string file )
		{
			// This method will be called whenever a file is processed.
		}


		public async Task ConnectAsync(Action<DeggSocketEvent> callback = null)
		{
			Connection = new WebSocket();
			try
			{
				Connection.OnMessageReceived += MessageRecievedHandler;

				var timer =new DelayedCallback( (Timer t) => {

				 },2000 );

				await Connection.Connect( Uri );
				timer.Delete();
				Log.Info( "HERE" );
				Login( callback );

			} catch(Exception e)
			{
				Log.Info( e );
				callback( null );
			}
		}

		public void TriggerEvent()
		{

		}

		public void SendMessage(string message)
		{
			_ = Connection.Send( message );
		}

		public void Reconnect( Action callback = null, Action errorCallback = null)
		{
			Disconnect();
			_ = ConnectAsync((e) =>
			{
				
				if (IsConnected())
				{
					if ( callback != null )
					{
						callback();
					}
				} else
				{
					if ( errorCallback != null )
					{
						errorCallback();
					}
				}
				
			} );
		}

		public void Disconnect()
		{
			if ( Connection != null )
			{
				if ( Connection.IsConnected )
				{
					Connection.Dispose();
					Connection = null;
				}
			}
			UnsubscribeAll();
		}

		public void FireCallback( DeggSocketEvent e)
		{
			if (Callbacks.ContainsKey(e.CallbackId))
			{
				var task = Callbacks[e.CallbackId];
				task( e );
				Callbacks.Remove( e.CallbackId );
			}
		}
		public void MessageRecievedHandler( string data )
		{ 
			try
			{
				DeggSocketEvent eventResponse = DeggSocketEvent.Deserialise( data );
				FireSubscriptions( eventResponse );
				if ( eventResponse.CallbackId != null )
				{
					FireCallback( eventResponse );
				}
			} catch(Exception e)
			{
				Log.Info( data );
				Log.Info( e );
			}
		}
	}
}

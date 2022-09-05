using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Degg.Websocket
{

	public class UnSubscriptionEvent
	{
		[JsonPropertyName( "topic" )]
		public string Topic { get; init; }
		[JsonPropertyName( "id" )]
		public string Id { get; set; }
		public UnSubscriptionEvent( string topic, string id )
		{
			Topic = topic;
			Id = id;
			if ( DeggSocket.Current != null )
			{
				DeggSocket.Current.Event( "unsubscribe", this );
			}
		}
	}

	public class SubscriptionEvent
	{
		[JsonPropertyName( "topic" )]
		public string Topic { get; init; }
		[JsonPropertyName( "id" )]
		public string Id { get; set; }
		public SubscriptionEvent( string topic, string id )
		{
			Topic = topic;
			Id = id;
			if ( DeggSocket.Current != null )
			{
				DeggSocket.Current.Event( "subscribe", this );
			}
		}
	}
	public partial class DeggSocket
	{
		public Dictionary<string, Dictionary<string, Action<DeggSocketEvent>>> Subscriptions { get; set; }
	
		public string[] Subscribe(string topic, Action<DeggSocketEvent> callback)
		{
			CheckSubscriptions();

			var id = Guid.NewGuid().ToString();
			if ( !Subscriptions.ContainsKey(topic) )
			{
				Subscriptions[topic] = new Dictionary<string, Action<DeggSocketEvent>>();
			}
			new SubscriptionEvent( topic, id );
			Subscriptions[topic][id] = callback;

			return new string[] { topic, id };
		}

		public void CheckSubscriptions()
		{
			if ( Subscriptions == null )
			{
				Subscriptions = new Dictionary<string, Dictionary<string, Action<DeggSocketEvent>>>();
			}
		}

		public List<Action<DeggSocketEvent>> GetSubscriptions(string topic)
		{
			CheckSubscriptions();
			var items = new List<Action<DeggSocketEvent>>();
			if (Subscriptions.ContainsKey(topic))
			{
				foreach ( var callbacks in Subscriptions[topic]) {
					items.Add( callbacks.Value );
				}
			}
			return items;
		}


		public void FireSubscriptions( DeggSocketEvent eventResponse )
		{
			var subscriptions = GetSubscriptions( eventResponse?.Type );
			foreach ( var sub in subscriptions )
			{
				if ( sub != null )
				{
					try
					{
						sub( eventResponse );
					}
					catch ( Exception )
					{

					}
				}
			}
		}

		public void UnsubscribeAll()
		{
			if ( Subscriptions == null )
			{
				return;
			}
			if (Subscriptions != null)
			{
				foreach(var subs in Subscriptions)
				{
					var topic = subs.Key;
					foreach(var sub in subs.Value)
					{
						var id = sub.Key;
						Unsubscribe(topic,id );
					}
				}
			}
		}

		public void Unsubscribe( string topic, string id )
		{
			if ( Subscriptions == null )
			{
				return;
			}
			if ( Subscriptions.ContainsKey( topic ) )
			{
				Subscriptions[topic].Remove( id );
			}
			new UnSubscriptionEvent( topic, id );
		}

		public void Unsubscribe(string[] data )
		{
			if (Subscriptions == null)
			{
				return;
			}
			if ( data.Length != 2)
			{
				return;
			}
			var topic = data[0];
			var id = data[1];
			Unsubscribe( topic, id );
		}
	}
}

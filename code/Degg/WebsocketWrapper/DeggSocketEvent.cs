using System.Text.Json;

namespace Degg.Websocket
{
	public partial class DeggSocketEvent
	{
		public string Id { get; set; } = "";
		public string Data { get; set; } = "";
		public string Type { get; set; } = "";
		public string CallbackId { get; set; } = "";

		public string Serialise()
		{
			return JsonSerializer.Serialize(this);
		}

		public void SetData(object data)
		{
			Data = JsonSerializer.Serialize( data );
		}


		public T ToObject<T>( JsonSerializerOptions options = null )
		{
			var data = GetData();
			var str = data.GetString();
			return JsonSerializer.Deserialize<T>( str, options );
		}

		public T GetData<T>()
		{
			return ToObject<T>();
		}

		public JsonElement GetData()
		{
			return JsonSerializer.Deserialize<JsonElement>( Data );
		}

		public static DeggSocketEvent Deserialise(string payload)
		{
			return JsonSerializer.Deserialize<DeggSocketEvent>( payload );
		}

	}
}

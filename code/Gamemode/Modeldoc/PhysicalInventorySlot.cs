using System.Text.Json.Serialization;

namespace Sandbox.Gamemode.Modeldoc
{

	public class PhysicalInventoryItem
	{
		[JsonPropertyName( "Tags" )]
		public string[] Tags { get; set; }

		[JsonPropertyName( "index" )]
		public float Index { get; set; }
	}

	[ModelDoc.GameData( "physical_inventory_slot", AllowMultiple = true )]
	[ModelDoc.Sphere( "scale", "position" )]
	public class PhysicalInventoryItemSlot : PhysicalInventoryItem
	{
		[JsonPropertyName( "position" )]
		public Vector3 Position { get; set; }
		[JsonPropertyName( "scale" )]
		public float Scale { get; set; }
	}


	[ModelDoc.GameData( "physical_inventory_control", AllowMultiple = true )]
	[ModelDoc.Sphere( "scale", "position" )]
	public class PhysicalInventoryItemControl : PhysicalInventoryItem
	{
		[JsonPropertyName( "position" )]
		public Vector3 Position { get; set; }

		[JsonPropertyName( "scale" )]
		public float Scale { get; set; }

		[JsonPropertyName( "action" )]
		public string Action { get; set; }
	}
}

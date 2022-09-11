using Degg;
using Sandbox;

namespace FantasyTest
{

	public enum PropResourceCategoryEnum
	{
		General
	}

	[GameResource( "FantasyProp", "fprop", "Describes a Fanatasy Game Prop" )]
	public partial class PropResource : DeggGameResource
	{
		public string PropName { get; set; }
		public string Description { get; set; }
		public PropResourceCategoryEnum Category { get; set; }
		[ResourceType( "vmdl" )]
		public string Model { get; set; }
		public string ClassName { get; set; }
		public float Mass { get; set; } = -1f;
		public float Scale { get; set; } = 1f;
		public bool CanPickup { get; set; } = false;
	}
}

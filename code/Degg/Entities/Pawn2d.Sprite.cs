
using Degg.Core;
using Sandbox;

namespace Degg.Entities
{
	public partial class Pawn2D : DeggPlayer
	{
		[Net]
		public Entity2DSprite PlayerSprite { get; set; }

	}
}

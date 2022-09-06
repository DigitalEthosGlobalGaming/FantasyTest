using Sandbox;

namespace FantasyTest.Gamemode.Player.VR
{
	public class VRHandLeft : VRHandBase
	{
		protected override string ModelPath => "models/hands/handleft.vmdl";
		public override Input.VrHand InputHand => Input.VR.LeftHand;

		public override void Spawn()
		{
			base.Spawn();
			Log.Info( "VR Controller Left Spawned" );
			SetInteractsAs( CollisionLayer.LEFT_HAND );
		}
	}
}

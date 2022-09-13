using FantasyTest;

namespace Sandbox.Gamemode.Player.PC
{
	public partial class GamePlayerPC : GameBasePlayer
	{
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Log.Info( "Respawn PC Player?" );

			SetModel( "models/citizen/citizen.vmdl" );
			SetCamera<PlayerCamera>();

			Controller = new PCPlayerController();
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}
	}
}

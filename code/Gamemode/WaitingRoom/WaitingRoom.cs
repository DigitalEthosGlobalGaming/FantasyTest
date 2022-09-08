using Degg.Util;
using Sandbox;

namespace FantasyTest
{
	public partial class WaitingRoom : Room
	{
		SpawnPoint MainSpawn { get; set; }
		public void Create()
		{
			if ( IsClient )
			{
				return;
			}

			if ( MainSpawn?.IsValid() ?? false )
			{
				MainSpawn.Delete();
			}

			MainSpawn = new SpawnPoint();
			var spawnPosition = Vector3.Up * 1000f;
			MainSpawn.Position = spawnPosition + (Vector3.Up * Metrics.TerryHeight);
			Init<RoomTile>( spawnPosition, new Vector2( FloorSize, FloorSize ), 5, 5 );
		}

		public override void OnSetup()
		{
			base.OnSetup();
			SpawnAllLoadingPlayers();
			Log.Info( "SETUP" );

		}

		public void SpawnAllLoadingPlayers()
		{
			var players = GameLoadingPawn.GetAllPlayers<GameLoadingPawn>();
			foreach ( var player in players )
			{
				player.OnJoin();
			}

			MyGame.TeleportAllPlayersToWaitingRoom();
		}
	}
}


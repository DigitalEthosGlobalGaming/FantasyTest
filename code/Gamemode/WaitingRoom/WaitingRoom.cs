using Degg.Entities.Common;
using Degg.Util;
using Sandbox;
using System.Collections.Generic;

namespace FantasyTest
{
	public partial class WaitingRoom : Room
	{
		SpawnPoint MainSpawn { get; set; }
		public List<Entity> WatchedEntities { get; set; }

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
			WatchedEntities = new List<Entity>();

			var light = new CandlePointLight()
			{

			};
			WatchedEntities.Add( light );
			light.Position = MainSpawn.Position + Vector3.Up * 250f;
		}


		public override void OnSetup()
		{
			base.OnSetup();
			SpawnAllLoadingPlayers();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				if ( WatchedEntities != null )
				{
					foreach ( var i in WatchedEntities )
					{
						i.Delete();
					}
				}
				WatchedEntities = new List<Entity>(); ;
			}
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


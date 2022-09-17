using Degg;
using Degg.Core;
using Degg.Entities;
using Sandbox;
using Sandbox.DeggCommon.Util;

namespace FantasyTest
{
	public partial class GameLoadingPawn : DeggLoadingPawn
	{
		public ModelStore ModelLoader { get; set; }

		[Net]
		public bool IsReady { get; set; }

		public bool ClientReadied { get; set; }

		[Net]
		public bool ModelsReady { get; set; }
		public override void HudSetup()
		{
			// Setups the hud.. duh
			base.HudSetup();
		}

		public override void Spawn()
		{
			base.Spawn();
			WarmupModels();
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();
			WarmupModels();
		}

		[ConCmd.Server( "ss.client.loaded" )]
		public static void OnLoad()
		{
			if ( MyGame.GameWaitingMap == null )
			{
				MyGame.SetupWaitingRoom();
				return;
			}
			if ( MyGame.GameState == GameStateEnum.READY )
			{
				var player = ClientUtil.GetCallingPawn<GameLoadingPawn>();
				if ( ConsoleSystem.Caller.IsUsingVr )
				{
					player.EntityName = "GamePlayerVR";
				}
				else
				{
					player.EntityName = "GameBasePlayer";
				}
			}
		}

		public void GameStart()
		{
			MyGame.ReadyUp();
		}

		public void WarmupModels()
		{

			ModelsReady = false;
			if ( ModelLoader?.IsValid() ?? false )
			{
				ModelLoader.Delete();
			}

			ModelLoader = new ModelStore();
			ModelLoader.LoadModel( "weapon_dagger", "deggassets/models/fantasy/items/dagger.vmdl" );
			ModelLoader.LoadModel( "weapon_small_axe", "deggassets/models/fantasy/viewmodels/v_crowbar.vmdl" );
			ModelLoader.LoadModel( "v_book_builder", "deggassets/models/fantasy/viewmodels/v_book_builder.vmdl" );
			//ModelLoader.LoadModel( "floor", "models/fantasy/floor_standard.vmdl" );
			//ModelLoader.LoadModel( "wall", "models/fantasy/floor_standard.vmdl" );
			//ModelLoader.LoadModel( "wall", " models/fantasy/walls.vmdl" );
			ModelLoader.LoadModel( "floor", "deggassets/models/basic/basic_floor.vmdl" );
			ModelLoader.LoadModel( "wall", "deggassets/models/basic/basic_wall.vmdl" );
			ModelLoader.LoadModel( "door", "deggassets/models/basic/basic_doorway.vmdl" );

			ModelLoader.LoadModel( "player_backpack", "assets/items/containers/backpack.vmdl" );



		}


		public override Entity OnJoin()
		{
			if ( ModelsReady && (MyGame.GameWaitingMap?.IsAllSetup() ?? false) )
			{
				if ( Client.IsUsingVr )
				{
					EntityName = "GamePlayerVR";
				}
				else
				{
					EntityName = "GamePlayerPC";
				}

				var result = base.OnJoin();
				if ( result is DeggPlayer player )
				{
					player.Respawn();
					player.Position = MyGame.GameWaitingMap.MainSpawn.Position;
				}


				return result;
			}
			return this;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( IsClient )
			{
				if ( !ClientReadied )
				{
					GameStart();
					ClientReadied = true;
				}
			}

			if ( ModelsReady == false && (ModelLoader?.IsValid() ?? false) )
			{
				if ( ModelLoader.IsComplete )
				{
					ModelsReady = true;
				}
			}
			if ( IsServer )
			{
				if ( IsReady )
				{
					OnJoin();
				}
			}
		}

	}
}

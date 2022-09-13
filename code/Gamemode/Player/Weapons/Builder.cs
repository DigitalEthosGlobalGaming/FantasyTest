using Degg;
using Degg.Util;
using FantasyTest.MapEntities;
using Sandbox;
using System;

namespace FantasyTest
{
	/// <summary>
	/// A common base we can use for weapons so we don't have to implement the logic over and over
	/// again. Feel free to not use this and to implement it however you want to.
	/// </summary>
	[Title( "Builder Weapon" ), Icon( "sports_martial_arts" )]
	public partial class Builder : PlayerWeaponBase
	{
		[Net, Change]
		public PropResource CurrentPropResource { get; set; }

		[Net, Predicted]
		public float PlacementRotation { get; set; }

		public MapProp ClientGhost { get; set; }

		[Net, Predicted]
		public Vector3 PlacePosition { get; set; }
		public override string WorldModel => "weapon_dagger";
		public override string ViewModelPath => "weapon_dagger";

		public float NextPlaceTime = 0f;

		public override Transform ViewModelOffset => new Transform( Vector3.Down * 5f + Vector3.Forward * 20f + Vector3.Right * 5f, Rotation.FromAxis( Vector3.Left, 90f ), 0.5f );
		// override ViewModelScale

		public override void Spawn()
		{
			base.Spawn();
			Tags.Add( "item" );
			CurrentPropResource = PropResource.Get<PropResource>( "assets/props/chest_open.fprop" );
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ClientGhost?.Delete();
		}
		public override void ActiveEnd( Entity ent, bool dropped )
		{
			base.ActiveEnd( ent, dropped );
			ClientGhost?.Delete();
		}

		public void OnCurrentPropResourceChanged( PropResource before, PropResource after )
		{
			ClientGhost?.Delete();

			if ( after != null )
			{

				var className = after.ClassName;
				if ( className.Trim() == "" )
				{
					className = "GenericProp";
				}

				var newEntity = Entity.CreateByName<MapProp>( className );
				ClientGhost = newEntity;
				ClientGhost.SetPropData( after );
			}
		}

		public void CreateProp( Vector3 position, Rotation rot )
		{
			if ( IsServer )
			{
				var prop = CurrentPropResource;
				if ( prop == null )
				{
					return;
				}

				if ( !IsValidPlacement( position, rot ) )
				{
					return;
				}

				var className = prop.ClassName;
				if ( className.Trim() == "" )
				{
					className = "GenericProp";
				}
				var newEntity = Entity.CreateByName<MapProp>( className );
				newEntity.Position = position;
				newEntity.Rotation = rot;
				newEntity.SetPropData( prop );

				var playerRoom = GetOwner<GameBasePlayer>()?.MyRoom;
				newEntity.AddToRoom( playerRoom );

				CurrentPropResource = DeggGameResource.GetRandom<PropResource>();
			}
		}

		public bool IsValidPlacement( Vector3 position, Rotation rot )
		{
			var player = GetOwner<GameBasePlayer>();
			if ( player?.IsValid() ?? false )
			{

				var bboxMaybe = player?.MyRoom?.GetBBox();
				if ( bboxMaybe.HasValue )
				{
					var bbox = bboxMaybe.Value;
					var checker = new BBox( position );
					if ( bbox.Contains( checker ) )
					{
						return true;
					}
				}

			}
			return false;
		}

		public override void AttackPrimary()
		{
			base.AttackPrimary();

			if ( IsClient )
			{
				return;
			}


			AdvLog.Info( this.GetHashCode(), Time.Now );

			var trace = TraceFromEyes( 500f, 5f, new string[] { "player-room" } );
			if ( trace.Hit )
			{
				var rotation = PlacementRotation;
				if ( Input.Down( InputButton.Run ) )
				{
					rotation = (float)Math.Ceiling( rotation / (45 / 2) ) * (45 / 2);
				}

				CreateProp( trace.EndPosition, Rotation.FromAxis( Vector3.Up, rotation ) );
			}
		}
		public override void Simulate( Client player )
		{
			base.Simulate( player );

			var mouseRotation = (float)Input.MouseWheel * Time.Delta * 1000f;

			PlacementRotation += mouseRotation;

			if ( IsClient )
			{
				if ( ClientGhost?.IsValid() ?? false )
				{
					var trace = TraceFromEyes( 500f, 5f, new string[] { "player-room" } );
					if ( trace.Hit )
					{
						var rotation = PlacementRotation;
						if ( Input.Down( InputButton.Run ) )
						{
							rotation = (float)Math.Ceiling( rotation / (45 / 2) ) * (45 / 2);
						}
						ClientGhost.Position = trace.EndPosition;
						ClientGhost.Rotation = Rotation.FromAxis( Vector3.Up, rotation );

						if ( IsValidPlacement( ClientGhost.Position, ClientGhost.Rotation ) )
						{
							ClientGhost.RenderColor = Color.Green.WithAlpha( 0.5f );
						}
						else
						{
							ClientGhost.RenderColor = Color.Red.WithAlpha( 0.5f );
						}
					}
				}
			}
		}
	}
}

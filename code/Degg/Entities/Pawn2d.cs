using Degg.Cameras;
using Degg.Core;
using Degg.Util;
using Sandbox;
using System;

namespace Degg.Entities
{
	public partial class Pawn2D : DeggPlayer
	{

		public Entity2DCursor Cursor { get; set; }

		public string CursorPlaneTag { get; set; }

		[Net, Change]
		public string EntityMaterial { get; set; }

		public string ClientEntityMaterial { get; set; }

		public void OnEntityMaterialChanged( string before, string after )
		{
			SetMaterial( after );
		}

		public Vector3 GetCursorPosition()
		{
			AdvLog.Info( CursorPlaneTag );
			if ( CursorPlaneTag != null)
			{
				var from = Input.Cursor.Origin;
				var to = Input.Cursor.Project( 250f );
				var ray = Trace.Ray( from, to );
				DebugOverlay.Line( from, to, 5f );
				var entity = ray.Run();
				return entity.HitPosition;
			}
			return Vector3.Zero;
		}

		public void SetMaterial( string name )
		{
			var mat = Material.Load( name );
			SetMaterialOverride( mat );
		}

		public float ZIndex { get; set; }
		/// <summary>
		/// Called when the entity is first created 
		/// </summary>
		public override void Spawn()
		{
			base.Spawn();
			SetupCamera();
			EnableDrawing = true;
		}

		public virtual TopdownCamera SetupCamera()
		{
			return SetCamera<TopdownCamera>();
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();
			if ( EntityMaterial != null )
			{
				SetMaterialOverride( EntityMaterial );
			}
		}

		public override void Simulate( Client cl )
		{
			if ( Position.z != ZIndex )
			{
				Position = Position.WithZ( ZIndex );
			}
			
			base.Simulate( cl );


			if (IsClient)
			{
				if ( Cursor?.IsValid() ?? false )
				{
					var distanceFromZero = Input.Cursor.Origin.z - Vector3.Zero.z;
					Cursor.Position = Input.Cursor.Project( distanceFromZero );
					Cursor.Position = Cursor.Position.WithZ( 0 );
				}
			}
		}


		public override void ServerTick()
		{
			base.ServerTick();
		}
		public override void ClientTick()
		{
			base.ClientTick();
			if ( Hud?.IsValid() ?? false )
			{
				Hud.HasCursor = Cursor?.IsValid() ?? false;
			}

			if ( ClientEntityMaterial != EntityMaterial && EntityMaterial != null )
			{
				SetMaterialOverride( EntityMaterial );
				EntityMaterial = ClientEntityMaterial;
			}
		}




		public void LookAt( float x, float y, float rotateAmount = 1f , float degreeOffset = 90f )
		{
			Rotation = GetRotationLookingAt( x, y, rotateAmount, degreeOffset );
		}

		public Rotation GetRotationLookingAt( Vector3 pos, float rotateAmount, float degreeOffset = 90f )
		{
			return GetRotationLookingAt( pos.x, pos.y, rotateAmount, degreeOffset );
		}

		public Rotation GetRotationLookingAt(float x, float y, float rotateAmount, float degreeOffset = 90f )
		{
			float rad = (float)Math.Atan2( -x, y );
			float deg = (float)(rad * (180 / Math.PI)) + degreeOffset;

			var rotation = Rotation.FromAxis( Vector3.Up, deg ) ;
			var difference = Rotation.Distance( rotation );
			if ( rotateAmount == 0 )
			{
				return rotation;
			}
			else
			{
				return Rotation.Slerp( Rotation, rotation, (RealTime.Delta * rotateAmount * 100f) / difference );
			}
		}

		public void SetShape( Entity2DShapes shape, float scale = 1f )
		{
			switch ( shape )
			{				
				case Entity2DShapes.Square:
					var a = Entity2D.DefaultEntitySize / 2;
					a = a * scale;
					SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, -a, a );
					break;
				case Entity2DShapes.Circle:
					SetupPhysicsFromSphere( PhysicsMotionType.Dynamic, Vector3.Zero, 2.5f * scale) ;
					break;
				case Entity2DShapes.Other:
					break;
			}

			if ( PhysicsBody != null )
			{
				PhysicsBody.Mass = Entity2D.DefaultEntityMass;
				PhysicsBody.GravityEnabled = false;
			}
		}

		public override void Touch( Entity other )
		{
			base.Touch( other );
			if (other is BaseTrigger trigger)
			{
				OnTriggerTouch( trigger );
			}
		}

		public virtual void OnTriggerTouch(BaseTrigger trigger)
		{

		}
	}
}

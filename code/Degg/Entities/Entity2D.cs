using Degg.Networking;
using Degg.Util;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Degg.Entities
{
	public enum Entity2DShapes
	{
		Circle,
		Square,
		Other
	}
	public partial class Entity2D : ModelEntity
	{
		public bool LockTo2D { get; set; } = true;
		public const float DefaultEntityMass = 10f;
		public const float DefaultEntitySize = 10f;
		public string EntityMaterial { get; set; }

		public float RotationDegrees { get; set; }

		[Net]
		public BBox Dimensions { get; set; }

		public float ZIndex { get; set; }


		public T GetClosest<T>( float? min = null, float? max = null ) where T : Entity2D
		{
			var entities = Entity2D.All.ToList();
			float closestDistance = float.MaxValue;
			T closest = null;
			foreach ( var entity in entities )
			{
				if ( entity != this )
				{
					if ( entity is T t )
					{
						if ( entity?.IsValid() ?? false )
						{
							var distance = Position.Distance( entity.Position );
							var minD = min.GetValueOrDefault( distance );
							var maxD = max.GetValueOrDefault( distance );
							if ( distance >= minD && distance <= maxD )
							{
								if ( distance < closestDistance )
								{
									closestDistance = distance;
									closest = t;
								}
							}
						}
					}
				}
			}
			return closest;
		}

		public static KeyValuePair<float, T> FindClosest<T>( Vector3 position ) where T : Entity2D
		{
			var entities = Entity2D.All.ToList();
			float closestDistance = float.MaxValue;
			T closest = null;
			foreach ( var entity in entities )
			{
				if ( entity is T t )
				{
					if ( entity?.IsValid() ?? false )
					{
						var distance = position.Distance( entity.Position );
						if ( distance < closestDistance )
						{
							closestDistance = distance;
							closest = t;
						}
					}
				}
			}
			return new KeyValuePair<float, T>( closestDistance, closest);
		}

		public T GetClosestPawn<T>( float? min = null, float? max = null ) where T : Pawn2D
		{
			var entities = Pawn2D.All.ToList();
			float closestDistance = float.MaxValue;
			T closest = null;
			foreach ( var entity in entities )
			{
				if ( entity != this )
				{
					if ( entity is T t )
					{
						if ( entity?.IsValid() ?? false )
						{
							var distance = Position.Distance( entity.Position );
							var minD = min.GetValueOrDefault( distance );
							var maxD = max.GetValueOrDefault( distance );
							if ( distance >= minD && distance <= maxD )
							{
								if ( distance < closestDistance )
								{
									closestDistance = distance;
									closest = t;
								}
							}
						}
					}
				}
			}
			return closest;
		}

		public void SetShape( float radius, float scale )
		{
			radius = (radius / DefaultEntitySize) * (scale);
			SetupPhysicsFromSphere( PhysicsMotionType.Dynamic, Vector3.Zero, radius );
		}

		public void SetShape( float width, float height, float scale = 1f )
		{
			Scale = scale;
			width = (width / DefaultEntitySize) * (scale / 2);
			height = (height / DefaultEntitySize) * (scale / 2);

			var a = new Vector3( width, height, 1 );
			Dimensions = new BBox( -a, a );

			SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, Dimensions.Mins, Dimensions.Maxs );

			if ( PhysicsBody != null )
			{
				PhysicsBody.Mass = DefaultEntityMass;
				PhysicsBody.GravityEnabled = false;
			}
		}

		public float GetRadius()
		{
			return 5f * Scale;
		}
		public void SetShape( Entity2DShapes shape, float scale = 1f )
		{
			Scale = scale;
			switch ( shape )
			{
				case Entity2DShapes.Square:
					var a = DefaultEntitySize;
					a = a * scale;
					SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, -a, a );
					break;
				case Entity2DShapes.Circle:
					SetupPhysicsFromSphere( PhysicsMotionType.Dynamic, Vector3.Zero, GetRadius() );
					break;
				case Entity2DShapes.Other:
					break;
			}
			if ( PhysicsBody != null )
			{
				PhysicsBody.Mass = DefaultEntityMass;
				PhysicsBody.GravityEnabled = false;
			}
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();
		}

		[Event.Tick]
		public void Tick()
		{
			if ( IsServer )
			{
				ServerTick();
			}
			if ( IsClient )
			{
				ClientTick();
			}
		}
		public virtual void ClientTick()
		{
		}

		public virtual void ServerTick()
		{

			if ( LockTo2D )
			{
				var zPosition = ZIndex * 5f;
				if ( Position.z != zPosition )
				{
					Position = Position.WithZ( zPosition );
				}

				var rotation = Rotation.FromAxis( Vector3.Up, RotationDegrees );
				Rotation = rotation;
			}
		}

		public virtual Rotation LookAt( Vector3 position, float? rotateAmount = null, float degreeOffset = 90f )
		{
			return LookAt( position.x, position.y, rotateAmount, degreeOffset );

		}

		public Rotation LookAt( Vector2 position, float? rotateAmount = null, float degreeOffset = 90f )
		{
			return LookAt( position.x, position.y, rotateAmount, degreeOffset );
		}

		public Rotation LookAt( float x, float y, float? rotateAmount = null, float degreeOffset = 90f )
		{
			Rotation = GetRotationLookingAt( x, y, rotateAmount, degreeOffset );
			return Rotation;
		}

		public Rotation GetRotationLookingAt( Vector3 pos, float? rotateAmount = null, float degreeOffset = 90f )
		{
			return GetRotationLookingAt( pos.x, pos.y, rotateAmount, degreeOffset );
		}

		public void SetVelocityFromAngle( float degrees, float amount )
		{
			var velocity = Vector2.FromRadians( (float)(degrees * Math.PI) / 180f ) * amount;
			Velocity = Velocity.WithX( velocity.x ).WithY( velocity.y );
		}

		public Rotation GetRotationLookingAt( float x, float y, float? rotateAmount, float degreeOffset = 90f )
		{
			x = x - Position.x;
			y = y - Position.y;
			float rad = (float)Math.Atan2( -x, y );
			float deg = (float)(rad * (180 / Math.PI)) + degreeOffset;

			var rotation = Rotation.FromAxis( Vector3.Up, deg );
			if ( !rotateAmount.HasValue )
			{
				return rotation;
			}
			else
			{
				var difference = Rotation.Distance( rotation );
				return Rotation.Slerp( Rotation, rotation, (RealTime.Delta * rotateAmount.Value * 100f) / difference );
			}
		}

		[ClientRpc]
		public void EmitSound( string name )
		{
			PlaySound( name );
		}

		public override void TakeDamage( DamageInfo info )
		{
			base.TakeDamage( info );
			var networkedInfo = new NetworkedDamageInfo( info );
			ClientTakeDamage( networkedInfo.Serialise() );
		}


		[ClientRpc]
		public void ClientTakeDamage( string data )
		{
			var info = NetworkedDamageInfo.Deserialise( data );
			ClientTakeDamage( info );
		}

		public virtual void ClientTakeDamage( NetworkedDamageInfo data )
		{

		}

		public static List<T> FindAll<T>( ) where T : Entity
		{
			var entities = All.Where( ( item ) =>
			{				
				if ( item is T t )
				{
					return true;
				}
				return false;
			} );

			List<T> list = new List<T>();
			foreach ( var ent in entities )
			{
				if ( ent is T t )
				{
					list.Add( t );
				}
			}

			return list;
		}

		public List<T> FindInSphere<T>( float radius, bool ignoreSelf = true ) where T : Entity
		{
			var foundEntitied = FindInSphere<T>( Position, radius );
			if (ignoreSelf)
			{
				return foundEntitied.FindAll( ( item ) => item != this );
			}
			return foundEntitied;
		}
		public static List<T> FindInSphere<T>(Vector3 position, float radius) where T: Entity
		{
			var nearbyEntities = FindInSphere( position, radius );
			List<T> nearbyValidEntities = new List<T>();
			foreach ( var entity in nearbyEntities )
			{
				if ( entity is T t )
				{
					nearbyValidEntities.Add( t );
				}
			}

			return nearbyValidEntities;
		}
	}
}

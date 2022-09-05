
using Sandbox;
using System;
using System.Collections.Generic;
namespace Degg.GridSystem
{


	// Grid map is an instance of a space. This is the top level "grid" itself.
	public partial class GridMap : Entity
	{
		public enum Direction
		{
			Up = 0,
			Right = 1,
			Down = 2,
			Left = 3,
		}

		[Net]
		public List<GridSpace> Grid { get; set; }
		[Net]
		public int XSize { get; set; }
		[Net]
		public int YSize { get; set; }

		[Net]
		public float TileScale { get; set; }

		public int CurrentX { get; set; }
		public int CurrentY { get; set; }

		public bool IsStarting { get; set; }

		public Queue<Func<bool>> SetupFunctions { get; set; }

		public Action OnSetupAction { get; set; }

		[Net]
		public Vector2 GridSize { get; set; }

		[Net]
		public bool IsSetup { get; set; }

		[Net]
		public bool CanSetup { get; set; }

		[Net]
		public ModelEntity PhysicsModel { get; set; }


		public GridMap()
		{
			Transmit = TransmitType.Always;
			TileScale = 1f;
		}

		public void SetupPhysics()
		{
			if ( PhysicsModel?.IsValid() ?? false )
			{
				return;
			}

			var ent = new ModelEntity();
			ent.Parent = this;
			var mapSize = this.GetMapSize();
			var other = (new Vector3( GridSize.x, GridSize.y )) / 2;
			ent.Position = -(mapSize / 2) - other;
			var mins = Vector3.Zero.WithZ( -10f );
			var max = mapSize;
			ent.SetupPhysicsFromOBB( PhysicsMotionType.Static, mins, max );
			ent.Tags.Add( "GridMapPhysicsModel" );
			PhysicsModel = ent;
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				PhysicsModel?.Delete();

				foreach ( var i in Grid )
				{

					i.Delete();
				}
			}
		}


		public Vector3 GetWorldSpace( int x, int y )
		{
			var percentages = (new Vector2( x, y )) * GridSize;
			return new Vector3( percentages.x, percentages.y, 0 ) + Position - (GetMapSize() / 2);
		}

		public Vector2 GetGridSpace( float x, float y )
		{
			return GetGridSpace( new Vector2( x, y ) );
		}
		public Vector2 GetGridSpace( Vector3 position )
		{
			return GetGridSpace( new Vector2( position.x, position.y ) );
		}

		public Vector2 GetGridSpace( Vector2 v )
		{
			var original = v;
			var mapZeroedPositionV3 = Position - (GetMapSize() / 2);
			var v2 = new Vector2( mapZeroedPositionV3.x, mapZeroedPositionV3.y );
			v = v - v2;

			v = (v / GridSize);

			return v;
		}



		public void Init<T>( Vector3 position, Vector2 gridSize, int xSize, int ySize ) where T : GridSpace, new()
		{
			if ( !IsSetup )
			{
				XSize = xSize;
				YSize = ySize;
				Grid = new List<GridSpace>( xSize * ySize );
				Position = position;
				GridSize = gridSize;
				SetupFunctions = new Queue<Func<bool>>();
				for ( int x = 0; x < XSize; x++ )
				{
					for ( int y = 0; y < YSize; y++ )
					{
						var tempX = x;
						var tempY = y;
						SetupFunctions.Enqueue( () => AddTile<T>( tempX, tempY ) == null );
					}
				}
				CanSetup = true;
			}
		}


		public T AddTile<T>( int x, int y ) where T : GridSpace, new()
		{
			var existing = GetSpace( x, y );
			var existingIndex = -1;
			if ( existing != null )
			{
				existingIndex = Grid.IndexOf( existing );
			}

			try
			{
				var newSpace = new T();
				newSpace.Scale = TileScale;
				newSpace.Map = this;
				newSpace.GridPosition = new Vector2( x, y );
				if ( existingIndex >= 0 )
				{
					Grid.RemoveAt( existingIndex );
					Grid.Insert( existingIndex, newSpace );
					existing.Delete();
				}
				else
				{
					Grid.Add( newSpace );
				}
				newSpace.SetParent( this );
				OnSpaceSetup( newSpace );
				newSpace.OnAddToMap();
				return newSpace;
			}
			catch ( Exception e )
			{
				Log.Info( e );
				return null;
			}

		}

		public void CheckFinish()
		{
			IsStarting = true;
		}

		public int TransformGridPosition( int x, int y )
		{
			return (x * YSize) + y;
		}
		public int TransformGridPosition( float x, float y )
		{
			return ((int)x * XSize) + ((int)y);
		}

		public GridSpace GetGridSpaceFromWorld( Vector2 position )
		{
			return GetGridSpaceFromWorld<GridSpace>( position );
		}

		public T GetGridSpaceFromWorld<T>( Vector3 position ) where T : GridSpace
		{
			return GetGridSpaceFromWorld<T>( new Vector2( position.x, position.y ) );
		}

		public T GetGridSpaceFromWorld<T>( Vector2 position ) where T : GridSpace
		{
			position = GetGridSpace( position );

			return GetSpace<T>( position.x, position.y );
		}

		public List<T> GetGridAsList<T>()
		{
			var grid = new List<T>();

			foreach ( var item in Grid )
			{
				if ( item is T t )
				{
					grid.Add( t );
				}
			}
			return grid;
		}


		public List<GridSpace> GetGridAsList()
		{
			return GetGridAsList<GridSpace>();
		}


		public GridSpace GetSpace( Vector2 position )
		{
			return GetSpace( position.x, position.y );
		}

		public Vector2 GetGridSize()
		{
			return this.GridSize;
		}
		public Vector3 GetMapSize()
		{
			return new Vector3( this.GridSize * new Vector2( XSize, YSize ) );
		}

		public Vector2 GetMapSizeV2()
		{
			return new Vector2( this.GridSize * new Vector2( XSize, YSize ) );
		}

		public T GetSpace<T>( float x, float y ) where T : GridSpace
		{
			return GetSpace<T>( (int)Math.Round( x ), (int)Math.Round( y ) );
		}

		public GridSpace GetSpace( float x, float y )
		{
			return GetSpace<GridSpace>( x, y );
		}

		public T GetSpace<T>( int x, int y ) where T : GridSpace
		{
			if ( Grid == null )
			{
				return null;
			}
			if ( (x < XSize && x >= 0) && (y < YSize && y >= 0) )
			{
				var amount = TransformGridPosition( x, y );
				if ( amount >= 0 && amount < Grid.Count )
				{
					var space = Grid[amount];
					if ( space is T t )
					{
						return t;
					}
				}

			}

			return null;
		}

		public GridSpace GetSpace( int x, int y )
		{
			return GetSpace<GridSpace>( x, y );
		}



		public List<T> CreatePath<T>( GridSpace start, GridSpace end )
		{
			var mesh = new NavMesh( this );
			var navMesh = mesh.BuildPath( start.GridPosition, end.GridPosition );
			List<T> path = new List<T>();
			foreach ( var i in navMesh )
			{
				if ( i is T t )
				{
					path.Add( t );
				}
				else
				{
					return null;
				}
			}

			return path;
		}

		public List<GridSpace> CreatePath( GridSpace start, GridSpace end )
		{
			return CreatePath<GridSpace>( start, end );
		}

		public List<T> CreatePath<T>( Vector2 start, Vector2 end )
		{
			var mesh = new NavMesh( this );
			var navMesh = mesh.BuildPath( start, end );
			List<T> path = new List<T>();
			foreach ( var i in navMesh )
			{
				if ( i is T t )
				{
					path.Add( t );
				}
				else
				{
					return null;
				}
			}

			return path;
		}
		public List<GridSpace> CreatePath( Vector2 start, Vector2 end )
		{
			return CreatePath<GridSpace>( start, end );
		}

		public bool IsPath( Vector2 start, Vector2 end )
		{
			var mesh = new NavMesh( this );
			return mesh.BuildPath( start, end ).Count > 0;
		}
		public GridSpace GetRandomSpace()
		{
			var rnd = new Random();
			var x = rnd.Next( 0, XSize - 1 );
			var y = rnd.Next( 0, YSize - 1 );

			return GetSpace( (int)x, (int)y );
		}

		public List<T> GetTilesAtEdgeOfMap<T>() where T : GridSpace
		{
			List<T> tiles = new();
			for ( int i = 0; i < XSize - 1; i++ )
			{
				tiles.Add( (T)GetSpace( i, 0 ) );
				tiles.Add( (T)GetSpace( i, YSize - 1 ) );
			}
			for ( int i = 0; i < YSize - 1; i++ )
			{
				tiles.Add( (T)GetSpace( 0, i ) );
				tiles.Add( (T)GetSpace( XSize - 1, i ) );
			}

			return tiles;
		}

		public bool MoveItem( GridItem item, Vector2 newPosition )
		{
			var oldSpace = item.Space;
			var newSpace = GetSpace( newPosition );
			if ( newSpace == null )
			{
				return false;
			}
			oldSpace.RemoveItem( item, false );
			newSpace.AddItem( item, false );

			item.OnMove( newPosition, oldSpace.Position );

			return true;
		}

		public bool AddItem( GridItem item, Vector2 newPosition )
		{
			if ( item.Space != null )
			{
				return MoveItem( item, newPosition );
			}

			var newSpace = GetSpace( newPosition );
			if ( newSpace == null )
			{
				return false;
			}

			newSpace.AddItem( item );
			return true;
		}

		public virtual void OnSpaceSetup( GridSpace space )
		{

		}

		public virtual void OnSetup()
		{

		}




		[Event.Tick.Client]
		public virtual void ClientTick()
		{
			if ( IsSetup )
			{
				foreach ( var item in Grid )
				{
					if ( item != null )
					{
						item.ClientTick( Time.Delta, Time.Tick );
					}
				}
			}
		}

		[Event.Tick.Server]
		public virtual void ServerTick()
		{
			var startTime = Time.Now;
			var amountToCreateAtOnce = 10;

			if ( IsSetup == false && CanSetup )
			{
				while ( amountToCreateAtOnce > 0 && !IsSetup )
				{
					amountToCreateAtOnce = amountToCreateAtOnce - 1;
					if ( SetupFunctions.Count > 0 )
					{
						var callBack = SetupFunctions.Dequeue();
						if ( callBack != null )
						{
							callBack();
						}
					}
					else
					{
						OnSetup();
						OnChildrenSetup();
						IsSetup = true;
						OnSetupAction?.Invoke();
					}
				}
			}

			if ( IsSetup )
			{
				foreach ( var item in Grid )
				{
					if ( item != null )
					{
						item.ServerTick( Time.Delta, Time.Tick );
					}
				}
			}
		}

		public void OnChildrenSetup()
		{
			var tiles = GetGridAsList();
			foreach ( var item in tiles )
			{
				item.OnMapSetup();
			}
		}

	}
}

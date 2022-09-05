
using Degg.Util;
using Sandbox;
using System.Collections.Generic;

namespace Degg.GridSystem
{

	public partial class GridSpace : ModelEntity
	{
		[Net]
		public Vector2 GridPosition { get; set; }

		[Net]
		public GridMap Map { get; set; }

		[Net]
		public List<GridItem> Items { get; set; }

		public override void Spawn()
		{
			base.Spawn();
			Transmit = TransmitType.Always;
			Tags.Add( "GridSpace" );
		}


		public Vector3 GetWorldPosition()
		{
			if ( Map == null )
			{
				return Vector3.Zero;
			}

			return Map.GetWorldSpace( (int)this.GridPosition.x, (int)this.GridPosition.y );
		}
		public Vector3 GetTopWorldPosition()
		{
			var position = GetWorldPosition().WithZ( WorldSpaceBounds.Maxs.z );
			return position;
		}

		public virtual float GetMovementWeight( GridSpace a, NavPoint n )
		{
			if ( a == null )
			{
				return -1;
			}
			return 10;
		}


		public List<T> GetItems<T>() where T : GridItem
		{
			var items = new List<T>();
			foreach ( var item in Items )
			{
				if ( item is T tItem )
				{
					items.Add( tItem );
				}
			}

			return items;
		}


		public virtual void ClientTick( float delta, float currentTick )
		{

		}
		public virtual void ServerTick( float delta, float currentTick )
		{
			this.Scale = Map.TileScale;
		}


		public virtual void OnAddToMap()
		{
			Items = new List<GridItem>();
			this.Transmit = TransmitType.Always;
			UpdatePosition();
		}

		public void UpdatePosition()
		{
			Scale = Map.TileScale;
			Position = GetWorldPosition();
		}

		public virtual void OnMapSetup()
		{

		}

		public T GetNeighbour<T>( int x, int y ) where T : GridSpace
		{
			var positionToGet = new Vector2( x, y ) + GridPosition;
			if ( Map == null )
			{
				return null;
			}
			var tile = Map.GetSpace( positionToGet );
			if ( tile is T )
			{
				return (T)tile;
			}

			return null;
		}
		// Grabs immediate neighbours.
		// Note:
		// Up, Right, Down, Left in a clock-wise pattern to grab the neighbours.
		// If a neighbour does not exist, we will place the element as null;
		//	do check if the element in the array is null when using this in a for-loop.
		public T[] GetNeighbours<T>() where T : GridSpace
		{
			var up = GetNeighbour<T>( 0, -1 );
			var down = GetNeighbour<T>( 0, 1 );
			var left = GetNeighbour<T>( -1, 0 );
			var right = GetNeighbour<T>( 1, 0 );
			T[] neighbours = { up, right, down, left };
			return neighbours;
		}

		public bool IsEdgeTile()
		{
			var n = GetNeighbours<GridSpace>();
			var nullType = n[0] == null || n[1] == null || n[2] == null || n[3] == null;

			return nullType;
		}

		public GridSpace GetNeighbour( Vector2 pos )
		{
			var positionToGet = pos + GridPosition;
			return Map.GetSpace( positionToGet );
		}

		public void AddItem( GridItem item, bool triggerEvents = true )
		{
			item.Space = this;
			Items.Add( item );
			if ( triggerEvents )
			{
				OnItemAdded( item );
				item.OnAdded();
			}
		}

		public void RemoveItem<T>( T item, bool triggerEvents = true ) where T : GridItem
		{

			item.Space = null;
			Items.Remove( item );
			if ( triggerEvents )
			{
				OnItemRemoved( item );
				item.OnRemove();
			}
			item.Delete();
		}

		public void RemoveItems<T>( List<T> items, bool triggerEvents = true ) where T : GridItem
		{
			AdvLog.Info( items.Count );
			foreach ( var item in items )
			{
				RemoveItem( item, triggerEvents );
			}
		}

		public virtual void OnItemAdded( GridItem item ) { }
		public virtual void OnItemRemoved( GridItem item ) { }

		public override string ToString()
		{
			return $"SPACE [{GridPosition.x},{GridPosition.y}]";
		}
	}
}


using Degg.Util;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Degg.WaveFormCollapse
{

	public partial class WaveFormCollapse2dItemCollection: Array2dItem<WaveFormCollapseItem2dResource>
	{
		public WaveFormCollapse2dItemCollection( WaveFormCollapse2dGrid parent, int x, int y ) : base( parent, x, y )
		{
			Items = new List<WaveFormCollapseItem2dResource>();
			Grid = parent;
		}
		public WaveFormCollapse2dGrid Grid { get; set; }
		public WaveFormCollapseItem2dResource Current { get; set; }
		public List<WaveFormCollapseItem2dResource> Items { get; set; }

		public bool IsSolved()
		{
			return Current != null;
		}

		public bool SetItem( WaveFormCollapseItem2dResource current)
		{
			if (Current != null)
			{
				return false;
			}

			Current = current;

			Items = new List<WaveFormCollapseItem2dResource>();
			Items.Add( Current );
			CollapseNeighbours();
			return true;
		}

		public void Collapse( string[] codes)
		{
			bool isChange = false;

			var existingItems = new List<WaveFormCollapseItem2dResource>();


			var strings = new List<string>();
			foreach(var i in Items)
			{
				if(codes.Contains(i.Code))
				{
					existingItems.Add( i );
					strings.Add( i.Code );
				} else
				{
					isChange = true;
				}
			}

			Items = existingItems;

			AdvLog.Info( codes );
			AdvLog.Info( strings );

			


			if ( existingItems.Count == 1)
			{
				this.Set( existingItems.First() );
				return;
			}

			if ( isChange ) {
				CollapseNeighbours();
			}			
		}

		public void CollapseNeighbours()
		{
			if (Current == null)
			{
				return;
			}
			try
			{
				if ( Left() is WaveFormCollapse2dItemCollection left )
				{
					Log.Info( "LEFT" );
					left?.Collapse( GetLeftCodes() );
				}
				if ( Down() is WaveFormCollapse2dItemCollection down )
				{
					Log.Info( "DOWN" );
					down?.Collapse( GetBottomCodes() );
				}
				if ( Up() is WaveFormCollapse2dItemCollection up )
				{
					Log.Info( "UP" );
					up?.Collapse( GetTopCodes() );
				}
				if ( Right() is WaveFormCollapse2dItemCollection right )
				{
					Log.Info( "RIGHT" );
					right?.Collapse( GetRightCodes() );
				}
			} catch(Exception e)
			{
				Log.Info( e );
			}
		}

		public string[] GetLeftCodes()
		{
			var codes = new List<string>(); ;
			if (Current != null)
			{
				return Current.LeftCodes.Split( "," );
			}

			foreach(var i in Items)
			{
				if ( (i?.LeftCodes ?? "") != "" )
				{
					codes.AddRange( i.LeftCodes.Split( "," ) );
				}
			}

			return codes.ToArray();
		}
		public string[] GetRightCodes()
		{
			var codes = new List<string>(); ;
			if ( Current != null )
			{
				return Current.RightCodes.Split( "," );
			}

			foreach ( var i in Items )
			{
				if ( (i?.RightCodes ?? "") != "" )
				{
					codes.AddRange( i.RightCodes.Split( "," ) );
				}
			}

			return codes.ToArray();
		}

		public string[] GetTopCodes()
		{
			var codes = new List<string>(); ;
			if ( Current != null )
			{
				return Current.TopCodes.Split( "," );
			}

			foreach ( var i in Items )
			{
				if ( (i?.TopCodes ?? "") != "" )
				{
					codes.AddRange( i.TopCodes.Split( "," ) );
				}
			}

			return codes.ToArray();
		}

		public string[] GetBottomCodes()
		{
			var codes = new List<string>(); ;
			if ( Current != null )
			{
				return Current.BottomCodes.Split( "," );
			}

			foreach ( var i in Items )
			{
				if ( (i?.BottomCodes ?? "") != "" )
				{
					codes.AddRange( i.BottomCodes.Split( "," ) );
				}
			}

			return codes.ToArray();
		}

		public void PickRandom()
		{
			var newList = Items.FindAll( ( item ) =>
			{
				return item.Code != "blank";
			} );

			var item = Rand.FromList( newList );
			// Log.Info( item.Code );
			SetItem( item );
		}
	}

	public partial class WaveFormCollapse2dGrid: Array2d<WaveFormCollapseItem2dResource>
	{
		public WaveFormCollapse2dGrid( int width, int height ) : base( width, height )
		{
			PossibleItems = new List<WaveFormCollapseItem2dResource>();
		}

		public List<WaveFormCollapseItem2dResource> PossibleItems { get; set; }

		public bool Solved { get; set; }
		public int CurrentSolve { get; set; }

		public void AddPossibleItem(string name)
		{
			var item = WaveFormCollapseItem2dResource.Get<WaveFormCollapseItem2dResource>( name );

			if (item != null)
			{
				PossibleItems.Add( item );
			}
		}

		public WaveFormCollapse2dItemCollection Solve()
		{
			if (Solved)
			{
				return null;
			}
			var items = GetNotCollapsed();

			if (items.Count == 0)
			{
				Solved = true;
				return null;
			}

			var position = Rand.FromList(items );			
			return Solve( position.X, position.Y);

		}

		public List<WaveFormCollapse2dItemCollection> GetNotCollapsed()
		{
			List<WaveFormCollapse2dItemCollection> items = new List<WaveFormCollapse2dItemCollection>();
			foreach ( object i in this )
			{
				if ( i is WaveFormCollapse2dItemCollection collection )
				{
					if ( !collection.IsSolved() )
					{
						items.Add( collection );
					}
				}
			}
			return items;
		}

		public override WaveFormCollapse2dItemCollection CreateArray2dItem( int x, int y )
		{
			return new WaveFormCollapse2dItemCollection( this, x, y );
		}

		public WaveFormCollapse2dItemCollection Solve(int x, int y)
		{
			var item = Get( x, y );			
			if ( item is WaveFormCollapse2dItemCollection collection )
			{
				collection.PickRandom();
				return collection;
			} else
			{
				throw new System.Exception( "Invalid item" );
			}
		}
		public int CountNotCollapsed()
		{
			return GetNotCollapsed().Count;
		}

		public void GridUpdateItems()
		{

		}

		public void Init()
		{
			foreach ( var item in this )
			{
				if ( item is WaveFormCollapse2dItemCollection collection )
				{
					foreach ( var i in PossibleItems )
					{
						collection.Items.Add( i );
					}
				}
			}
		}
	}
}

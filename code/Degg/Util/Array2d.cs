using Sandbox;
using System;
using System.Collections;

namespace Degg.Util
{
	public enum Array2dDirections {
		Up,
		Right,
		Down,
		Left
	}

	public partial class Array2dItem<T>{
		public int X { get; set; }
		public int Y { get; set; }
		public bool IsValid { get; set; }
		public Array2dItem( Array2d<T> parent,int x, int y)
		{
			X = x;
			Y = y;
			Parent = parent;
		}
		public Array2d<T> Parent { get; set; }

		public T Get()
		{
			return Parent.GetItem( X, Y );
		}

		public void Set(T item)
		{
			if (IsValid)
			{
				Parent.Set( item, X, Y );
			}
		}

		public Array2dItem<T> Down()
		{
			return Parent.Get( X, Y + 1 );
		}
		public Array2dItem<T> Up()
		{
			return Parent.Get( X, Y - 1 );
		}
		public Array2dItem<T> Left()
		{
			return Parent.Get( X - 1, Y );
		}
		public Array2dItem<T> Right()
		{
			return Parent.Get( X + 1, Y);
		}
	}

	public partial class Array2d<T> : IEnumerable
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public Array2dItem<T>[] Items { get; set; }

		public Array2d(int width, int height )
		{
			SetupItems(width, height);
		}

		public void SetupItems(int width, int height )
		{
			Items = new Array2dItem<T>[width * height];
			Width = width;
			Height = height;
			for ( int x = 0; x < Width; x++ )
			{
				for ( int y = 0; y < Height; y++ )
				{
					var index = GetIndexOf( x, y );
					Items[index] = CreateArray2dItem( x, y );
				}
			}
		}

		public virtual Array2dItem<T> CreateArray2dItem(int x, int y)
		{
			return new Array2dItem<T>( this, x, y );
		}
		public T GetItem(int x, int y)
		{
			return Get( x, y ).Get();
		}

		public T GetItem(int x)
		{
			return Get( x ).Get();
		}

		public bool IsValidPosition( int x, int y )
		{
			var index = GetIndexOf( x, y );

			if (IsValidIndex(index))
			{
				return true;
			}
			return false;
		}

		public bool IsValidIndex( int index )
		{
			if ( index < 0 || index >= Items.Length )
			{
				return false;
			}
			return true;
		}

		public virtual Array2dItem<T> Get( int index )
		{
			if (!IsValidIndex(index) )
			{
				throw new Exception( $"Index {index} out of bounds" );
			}

			return Items[index];
		}


		public virtual Array2dItem<T> Get( int x, int y )
		{
			var index = GetIndexOf( x, y );
			if (IsValidIndex(index))
			{
				return Get( index );
			}
			return null;
		}


		public void Set( T item, int x, int y )
		{
			var position = Get( x,y );
			position.Set( item );			
		}

		public int GetIndexOf(int x, int y)
		{
			return (x * Height) + y;
		}
		public int[] GetXYFromIndex( int a)
		{
			int x = a / Height;
			int y = a % Height;
			return new int[2]{ x,y};
		}

		public Array2dItem<T> GetRandom()
		{
			var index = Rand.Int(0, Items.Length - 1);
			var position = GetXYFromIndex( index );
			return Get( position[0], position[1] );
		}

		//private enumerator class
		private class Array2dEnumerator : IEnumerator
		{
			public Array2d<T> Parent { get; set; }
			public Array2dItem<T>[] itemsList;
			int position = -1;

			//constructor
			public Array2dEnumerator( Array2d<T> parent, Array2dItem<T>[] list )
			{
				Parent = parent;
				itemsList = list;
			}
			private IEnumerator getEnumerator()
			{
				return (IEnumerator)this;
			}
			//IEnumerator
			public bool MoveNext()
			{
				position++;
				return (position < itemsList.Length);
			}
			//IEnumerator
			public void Reset()
			{
				position = -1;
			}
			//IEnumerator
			public object Current
			{
				get
				{
					try
					{
						return Parent.Get( position );
					}
					catch ( IndexOutOfRangeException )
					{
						throw new InvalidOperationException();
					}
				}
			}
		}  //end nested class
		public IEnumerator GetEnumerator()
		{
			return new Array2dEnumerator(this, Items );
		}
	}
}

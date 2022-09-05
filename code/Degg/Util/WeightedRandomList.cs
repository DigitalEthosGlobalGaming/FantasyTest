using System;
using System.Collections.Generic;

namespace Degg.Util
{
	public partial class WeightedRandomList<T>
	{
		private struct Entry
		{
			public double accumulatedWeight;
			public T item;
		}

		private List<Entry> entries = new List<Entry>();
		private double accumulatedWeight;
		private Random rand = new Random();

		public void AddEntry( T item, double weight )
		{
			accumulatedWeight += weight;
			entries.Add( new Entry { item = item, accumulatedWeight = accumulatedWeight } );
		}

		public T GetRandom(T def)
		{
			double r = rand.NextDouble() * accumulatedWeight;

			foreach ( Entry entry in entries )
			{
				if ( entry.accumulatedWeight >= r )
				{
					return entry.item;
				}
			}

			return default( T ); //should only happen when there are no entries
		}
	}
}

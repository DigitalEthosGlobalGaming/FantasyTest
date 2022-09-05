
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Degg.Core
{
	public partial class DifficultyScalableItem {
		public string Name { get; set; }
		public float ScaleAmount { get; set; }
		public float GetAmount (float difficulty)
		{
			return ScaleAmount * difficulty;
		}
	}

	public partial class DifficultySystem
	{
		public float Difficulty { get; set; }
		public Dictionary<string, DifficultyScalableItem> Items { get; set; }
		public DifficultySystem()
		{
			Items = new Dictionary<string, DifficultyScalableItem>();
		}

		public void SetDifficulty( float amount )
		{
			Difficulty = amount;
		}

		public float UpdateDifficulty( float amount )
		{
			Difficulty = Difficulty + amount;
			return Difficulty;
		}

		public void AddItem( string name, float scaleAmount )
		{
			var item = new DifficultyScalableItem();
			item.Name = name;
			item.ScaleAmount = scaleAmount;

			AddItem( item );
		}

		public void AddItem(DifficultyScalableItem item)
		{
			Items[item.Name] = item;
		}

		public float GetValue( string key, float def = 0)
		{
			if (Items.ContainsKey(key))
			{
				return Items[key].GetAmount( Difficulty );
			} else
			{
				return def;
			}
		}

	}

}

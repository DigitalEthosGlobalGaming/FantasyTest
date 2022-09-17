using Sandbox.Gamemode.Modeldoc;
using System.Collections.Generic;

namespace Sandbox.Gamemode.Entities
{
	public partial class ContainerSlot : Entity
	{
		public Entity Item { get; set; }
		public PhysicalInventoryItem Data { get; set; }

		public bool IsControl()
		{
			return Data is PhysicalInventoryItemControl;
		}

		public bool IsEmpty()
		{
			if ( IsControl() )
			{
				return false;
			}
			return Item?.IsValid ?? false;
		}

		public bool IsFull()
		{
			if ( IsControl() )
			{
				return true;
			}
			return Item?.IsValid ?? true;
		}

	}
	public partial class Container : Entity
	{
		[Net]
		public List<ContainerSlot> Items { get; set; }


		public override void Spawn()
		{
			base.Spawn();
			Items = new List<ContainerSlot>();
		}
		public virtual bool Contains( Entity i )
		{
			return GetSlotContaining( i ) == null;
		}
		public virtual ContainerSlot GetSlotContaining( Entity i )
		{
			foreach ( var item in Items )
			{
				if ( item.Item == i )
				{
					return item;
				}
			}
			return null;
		}

		public virtual void AddSlot( PhysicalInventoryItem data )
		{
			var slot = new ContainerSlot();
			slot.Data = data;
			Items.Add( slot );
		}


		public ContainerSlot GetSlot( int index )
		{
			if ( index >= Items.Count )
			{
				return null;
			}

			if ( index < 0 )
			{
				return null;
			}
			return Items[index];
		}


		public virtual bool Add( Entity item, int index )
		{
			var slot = GetSlot( index );
			if ( slot == null )
			{
				return false;
			}

			if ( slot?.IsFull() ?? true )
			{
				return false;
			}

			slot.Item = item;
			return true;
		}

		public virtual bool Remove( Entity i )
		{
			if ( !CanRemove( i ) )
			{
				return false;
			}

			var slot = GetSlotContaining( i );
			if ( slot == null )
			{
				return false;
			}

			slot.Item = null;

			return true;
		}

		public virtual bool CanAdd( Entity i )
		{
			return GetSlotContaining( i ) != null;
		}

		public virtual bool CanRemove( Entity i )
		{
			return GetSlotContaining( i ) == null;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				foreach ( var item in Items )
				{
					item.Delete();
					item?.Item?.Delete();
				}
				Items = null;
			}
		}

	}
}

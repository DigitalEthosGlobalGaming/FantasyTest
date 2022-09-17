using Sandbox.Gamemode.Modeldoc;
using System;

namespace Sandbox.Gamemode.Entities
{
	public partial class PhysicalContainer : ModelEntity
	{
		[Net]
		public Container Container { get; set; }

		public static PhysicalContainer LoadFromModel( string m )
		{
			var model = Model.Load( m );
			return LoadFromModel( model );
		}

		public static PhysicalContainer LoadFromModel( Model m )
		{
			try
			{
				Log.Info( m );
				var slots = m.GetData<PhysicalInventoryItemSlot[]>();
				var contols = m.GetData<PhysicalInventoryItemControl[]>();

				var container = new Container();
				if ( slots != null )
				{
					foreach ( var slot in slots )
					{
						container.AddSlot( slot );
					}
				}

				if ( contols != null )
				{
					foreach ( var slot in contols )
					{
						container.AddSlot( slot );
					}
				}

				var ent = new PhysicalContainer();
				ent.Container = container;
				ent.Model = m;

				return ent;
			}
			catch ( Exception e )
			{
				Log.Warning( e );
			}
			return null;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if ( IsServer )
			{
				Container?.Delete();
			}
		}
	}
}

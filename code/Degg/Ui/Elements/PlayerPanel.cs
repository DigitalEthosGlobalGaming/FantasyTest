
using Sandbox;
using Sandbox.UI;


namespace Degg.UI.Elements
{
	public partial class PlayerPanel<T> : Panel where T: Entity
	{

		protected T Player { get; set; }

		public A GetClientPawn<A>() where A: Entity
		{
			if (Local.Client?.Pawn?.IsValid() ?? false)
			{
				var pawn = Local.Client.Pawn;
				if (pawn is A)
				{
					return (A)pawn;
				}
			}
			return null;
		}

		public T GetClientPawn()
		{
			if ( Local.Client?.Pawn?.IsValid() ?? false )
			{
				var pawn = Local.Client.Pawn;
				if ( pawn is T )
				{
					return (T)pawn;
				}
			}
			return null;
		}
		public T GetPlayer()
		{
			return Player;
		}
		public A GetPlayer<A>() where A: T
		{
			if (GetPlayer() is A player)
			{
				return player;
			}
			return null;
		}
		public void SetPlayer(T player)
		{
			Player = player;
			OnPlayerSet();
		}
		public virtual void OnPlayerSet()
		{

		}
	}
}

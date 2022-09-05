using Degg.Core;
using Sandbox;

namespace Degg
{
	public class ClientUtil
	{
		public static T GetPawn<T>() where T: Entity
		{
			var client = Local.Client;
			if (client != null && client.Pawn is T pawn)
			{
				return pawn;
			}
			return null;
		}
		public static DeggPlayer GetPawn()
		{
			return GetPawn<DeggPlayer>();
		}

		public static T GetCallingPawn<T>() where T: Entity
		{
			var client = ConsoleSystem.Caller;
			if ( client?.Pawn is T clientPawn )
			{
				return clientPawn;
			}
			return null;
		}



	}
}

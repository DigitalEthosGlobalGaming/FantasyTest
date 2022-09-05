using Sandbox;

namespace Degg
{
	public static class DeggConsoleSystem
	{
		public static bool IsCallingAdmin()
		{
			var client = ConsoleSystem.Caller;
			return client.HasPermission( "admin" );
		}

	}
}

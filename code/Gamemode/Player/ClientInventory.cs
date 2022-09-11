using Sandbox;

namespace FantasyTest;
public partial class ClientInventory : BaseInventory
{
	public Client OwnerClient { get; set; }
	public ClientInventory( Client client ) : base( client?.Pawn )
	{
		OwnerClient = client;
	}
}

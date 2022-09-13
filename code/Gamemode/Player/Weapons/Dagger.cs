using Sandbox;

namespace FantasyTest
{
	/// <summary>
	/// A common base we can use for weapons so we don't have to implement the logic over and over
	/// again. Feel free to not use this and to implement it however you want to.
	/// </summary>
	[Title( "Base Weapon" ), Icon( "sports_martial_arts" )]
	public partial class Dagger : PlayerWeaponBase
	{

		public override string WorldModel => "weapon_dagger";
		public override string ViewModelPath => "weapon_dagger";

		public override Transform ViewModelOffset => new Transform( Vector3.Down * 5f + Vector3.Forward * 20f + Vector3.Right * 5f, Rotation.FromAxis( Vector3.Left, 90f ), 0.5f );
		// override ViewModelScale

		public override void Spawn()
		{
			base.Spawn();
			Tags.Add( "item" );
		}

		public override void Simulate( Client player )
		{
			base.Simulate( player );
		}
	}
}

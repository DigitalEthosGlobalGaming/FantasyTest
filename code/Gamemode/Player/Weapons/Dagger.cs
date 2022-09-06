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

		public static readonly Model WorldModel = Model.Load( "models/weapons/dagger.vmdl" );
		public override string ViewModelPath => "models/weapons/dagger.vmdl";

		public override void Spawn()
		{
			base.Spawn();
			Tags.Add( "item" );
		}
	}
}

using Sandbox;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Degg.Networking
{
	public partial class NetworkedDamageInfo : BaseNetworkable
	{
		private int _AttackerId { get; set; }
		private int _WeaponId { get; set; }
		[JsonIgnore]
		public Entity Attacker { get; set; }
		[JsonIgnore]
		public Entity Weapon { get; set; }
		public float Damage { get; set; }
		public int HitboxIndex { get; set; }
		public Vector3 Force { get; set; }

		public NetworkedDamageInfo() { }


		public NetworkedDamageInfo( DamageInfo d)
		{
			Attacker = d.Attacker;
			Weapon = d.Weapon;
			HitboxIndex = d.HitboxIndex;
			Damage = d.Damage;
			Force = d.Force;
		}


		public string Serialise()
		{
			return JsonSerializer.Serialize( this );
		}
		public static NetworkedDamageInfo Deserialise( string payload )
		{
			var data = JsonSerializer.Deserialize<NetworkedDamageInfo>( payload );
			data.Weapon = Entity.FindByIndex( data._WeaponId );
			data.Attacker = Entity.FindByIndex( data._AttackerId );

			return data;
		}
	}
}

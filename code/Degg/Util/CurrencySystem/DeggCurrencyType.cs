using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Degg.Util.CurrencySystem
{
	public partial class DeggCurrencyType: Entity
	{
		[Net]
		Dictionary<string, DeggCurrencyAccount> Accounts { get; set; }
		[Net]
		public bool Public { get; set; }


		public DeggCurrencyType()
		{
			Accounts = new Dictionary<string, DeggCurrencyAccount>();
		}

		public void Save()
		{
			foreach ( var kv in Accounts )
			{
				kv.Value.Save();
			}
		}

		public string GetKey( object obj )
		{
			if ( obj is Client c )
			{
				return "client_" + c.PlayerId;
			}
			else if ( obj is Entity e )
			{
				return "entity_" + e.NetworkIdent;
			}

			return obj.ToString();
		}

		public DeggCurrencyAccount GetAccount( object owner )
		{
			var accountId = GetKey( owner );
			if (IsClient)
			{
				var entity = All.FirstOrDefault( ( item ) =>
				 {
					 if ( item is DeggCurrencyAccount acc )
					 {
						 Log.Info( acc );
						 if ( acc.AccountId == accountId )
						 {
							 return true;
						 }
					 }
					 return false;
				 } );
				if ( entity is DeggCurrencyAccount account)
				{
					return account;
				}
				return null;
			}

			if ( !Accounts.ContainsKey( accountId ) )
			{	
				DeggCurrencyAccount account = new DeggCurrencyAccount();
				account.CurrencyType = this;
				if (owner is Entity e)
				{
					account.Owner = e;
				}
				account.AccountId = accountId;
				account.Load();

				Accounts[accountId] = account;
			}

			return Accounts[accountId];
		}

		public float GetAmount(string key)
		{
			return GetAccount( key ).Amount;
		}
		public void SetAmount( string key, float amount )
		{
			var account = GetAccount( key );
			account.Amount = amount;
		}

		public float GetAmount( object s )
		{
			return GetAmount(GetKey( s ));
		}

		public void SetAmount( object s, float amount )
		{
			var account = GetAccount( s );
			account.SetAmount( amount );
		}
	}
}


using System.Collections.Generic;
using Sandbox;

namespace Degg.Util.CurrencySystem
{
	public partial class DeggCurrency: Entity
	{
		[Net]
		public Dictionary<string, DeggCurrencyType> Currencies { get; set; }

		public DeggCurrency()
		{
			Currencies = new Dictionary<string, DeggCurrencyType>();
			Transmit = TransmitType.Always;
		}

		public DeggCurrencyType CreateCurrency( string name)
		{
			if (Currencies.ContainsKey(name))
			{
				return Currencies[name];
			}

			var currency = new DeggCurrencyType();
			Currencies[name] = currency;
			currency.Name = "degg-currency-type-" + name;
			return currency;
		}

		public DeggCurrencyType GetOrCreateCurrency(string name)
		{
			return CreateCurrency( name );
		}

		public DeggCurrencyType GetCurrency(string name)
		{
			if ( Currencies.ContainsKey( name ) )
			{
				return Currencies[name];
			}
			return null;
		}

		public DeggCurrencyAccount GetAccount(string name, object t)
		{
			return GetOrCreateCurrency( name )?.GetAccount( t );
		}


		public void Save()
		{
			if (IsClient)
			{
				return;
			}
			foreach ( var kv in Currencies )
			{
				kv.Value.Save();
			}
		}
	}
}

using Sandbox;
using System;

namespace Degg.Util.CurrencySystem
{

	class DeggCurrencyAccountData
	{
		public string AccountId { get; set; }
		public float Amount { get; set; }
	}
	public partial class DeggCurrencyAccount: Entity
	{
		[Net]
		public DeggCurrencyType CurrencyType { get; set; }

		[Net]
		public string AccountId { get; set; }

		public bool IsPublic { get;set; }

		[Net]
		public float Amount { get; set; }

		public void SetAmount(float amount)
		{
			Amount = amount;
		}

		public void AddAmount( float amount)
		{
			SetAmount(Amount + amount);
		}
		public void Subtract( float amount )
		{
			SetAmount( Amount - amount );
		}


		public void Load( )
		{
			Load( GetFolderPath() );
		}
		public void Save()
		{
			Save( GetFolderPath() );
		}

		public string GetFolderPath()
		{
			var folderPath = $"{GetSaveDirectory()}/{AccountId}";
			folderPath = folderPath.Trim().Replace( ' ', '-' ).Replace( '.', '-' );
			folderPath = folderPath + ".json";
			return folderPath;
		}
		public string GetSaveDirectory()
		{
			var folderPath = $"degg/degg-currency/{CurrencyType.Name}";
			folderPath = folderPath.Trim().Replace( ' ', '-' ).Replace( '.', '-' );
			return folderPath;
		}

		public void Save(string path)
		{
			var accountData = new DeggCurrencyAccountData();
			accountData.AccountId = AccountId;
			accountData.Amount = Amount;
			if (!FileSystem.Data.DirectoryExists( GetSaveDirectory ()) )
			{
				FileSystem.Data.CreateDirectory( GetSaveDirectory() );
			}

			FileSystem.Data.WriteJson( path, accountData );
		}

		public void Load( string path )
		{
			try
			{
				DeggCurrencyAccountData data = FileSystem.Data.ReadJson<DeggCurrencyAccountData>( path );
				if ( data != null )
				{
					Amount = data.Amount;
				}
			} catch(Exception)
			{

			}
		}
	}
}

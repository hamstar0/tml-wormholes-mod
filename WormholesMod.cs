using HamstarHelpers.Components.Config;
using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.DotNetHelpers;
using HamstarHelpers.Helpers.TmlHelpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Wormholes {
	partial class WormholesMod : Mod {
		public static WormholesMod Instance { get; private set; }
		


		////////////////

		public JsonConfig<WormholesConfigData> ConfigJson { get; private set; }
		public WormholesConfigData Config => this.ConfigJson.Data;

		private WormholesUI UI;



		////////////////

		public WormholesMod() : base() {
			this.ConfigJson = new JsonConfig<WormholesConfigData>( WormholesConfigData.ConfigFileName,
				ConfigurationDataBase.RelativePath, new WormholesConfigData() );
		}

		////////////////

		public override void Load() {
			string depErr = TmlHelpers.ReportBadDependencyMods( this );
			if( depErr != null ) { throw new HamstarException( depErr ); }

			WormholesMod.Instance = this;
			
			this.LoadConfig();

			// Clients and single only
			if( Main.netMode != 2 ) {
				WormholePortal.Initialize();
				WormholesUI.Initialize();

				this.UI = new WormholesUI();
			}
		}

		private void LoadConfig() {
			if( !this.ConfigJson.LoadFile() ) {
				this.ConfigJson.SaveFile();
			}

			if( this.Config.CanUpdateVersion() ) {
				this.Config.UpdateToLatestVersion();
				ErrorLogger.Log( "Wormholes updated to " + this.Version.ToString() );
				this.ConfigJson.SaveFile();
			}
		}

		public override void Unload() {
			WormholesMod.Instance = null;
		}
		
		////////////////

		public override void AddRecipeGroups() {
			RecipeGroup evacGrp = new RecipeGroup( () => Lang.misc[37] + " Evac Potion", new int[] {
				ItemID.RecallPotion, ItemID.WormholePotion
			} );
			RecipeGroup bookGrp = new RecipeGroup( () => Lang.misc[37] + " Basic Book", new int[] {
				ItemID.Book, ItemID.SpellTome
			} );

			RecipeGroup.RegisterGroup( "WormholesMod:EvacPotions", evacGrp );
			RecipeGroup.RegisterGroup( "WormholesMod:BasicBooks", bookGrp );
		}


		////////////////

		public override object Call( params object[] args ) {
			if( args == null || args.Length == 0 ) { throw new HamstarException( "Undefined call type." ); }

			string callType = args[0] as string;
			if( callType == null ) { throw new HamstarException( "Invalid call type." ); }

			var methodInfo = typeof( WormholesAPI ).GetMethod( callType );
			if( methodInfo == null ) { throw new HamstarException( "Invalid call type " + callType ); }

			var newArgs = new object[args.Length - 1];
			Array.Copy( args, 1, newArgs, 0, args.Length - 1 );

			try {
				return ReflectionHelpers.SafeCall( methodInfo, null, newArgs );
			} catch( Exception e ) {
				throw new HamstarException( "Wormholes.WormholesMod.Call - Bad API call.", e );
			}
		}
	}
}

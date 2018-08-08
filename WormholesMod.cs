using HamstarHelpers.Components.Config;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Wormholes {
	partial class WormholesMod : Mod {
		public static WormholesMod Instance { get; private set; }
		


		////////////////

		public JsonConfig<WormholesConfigData> ConfigJson { get; private set; }
		public WormholesConfigData Config { get { return this.ConfigJson.Data; } }

		private WormholesUI UI;


		////////////////

		public WormholesMod() : base() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
			
			this.ConfigJson = new JsonConfig<WormholesConfigData>( WormholesConfigData.ConfigFileName,
				ConfigurationDataBase.RelativePath, new WormholesConfigData() );
		}

		////////////////

		public override void Load() {
			WormholesMod.Instance = this;

			var hamhelpmod = ModLoader.GetMod( "HamstarHelpers" );
			var min_vers = new Version( 1, 2, 0 );
			if( hamhelpmod.Version < min_vers ) {
				throw new Exception( "Hamstar Helpers must be version " + min_vers.ToString() + " or greater." );
			}

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

			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Wormholes updated to " + WormholesConfigData.ConfigVersion.ToString() );
				this.ConfigJson.SaveFile();
			}
		}

		public override void Unload() {
			WormholesMod.Instance = null;
		}
		
		////////////////

		public override void AddRecipeGroups() {
			RecipeGroup evac_grp = new RecipeGroup( () => Lang.misc[37] + " Evac Potion", new int[] {
				ItemID.RecallPotion, ItemID.WormholePotion
			} );
			RecipeGroup book_grp = new RecipeGroup( () => Lang.misc[37] + " Basic Book", new int[] {
				ItemID.Book, ItemID.SpellTome
			} );

			RecipeGroup.RegisterGroup( "WormholesMod:EvacPotions", evac_grp );
			RecipeGroup.RegisterGroup( "WormholesMod:BasicBooks", book_grp );
		}


		////////////////

		public override object Call( params object[] args ) {
			if( args.Length == 0 ) { throw new Exception( "Undefined call type." ); }

			string call_type = args[0] as string;
			if( args == null ) { throw new Exception( "Invalid call type." ); }

			var new_args = new object[args.Length - 1];
			Array.Copy( args, 1, new_args, 0, args.Length - 1 );

			return WormholesAPI.Call( call_type, new_args );
		}
	}
}

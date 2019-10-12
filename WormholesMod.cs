using HamstarHelpers.Helpers.TModLoader.Mods;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Wormholes {
	partial class WormholesMod : Mod {
		public static WormholesMod Instance { get; private set; }
		


		////////////////

		public WormholesConfig Config => ModContent.GetInstance<WormholesConfig>();

		private WormholesUI UI;



		////////////////

		public WormholesMod() : base() {
			WormholesMod.Instance = this;
		}

		////////////////

		public override void Load() {
			// Clients and single only
			if( Main.netMode != 2 ) {
				WormholePortal.Initialize();
				WormholesUI.Initialize();

				this.UI = new WormholesUI();
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
			return ModBoilerplateHelpers.HandleModCall( typeof( WormholesAPI ), args );
		}
	}
}

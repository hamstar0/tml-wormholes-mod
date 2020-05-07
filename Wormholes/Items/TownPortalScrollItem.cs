using HamstarHelpers.Helpers.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Wormholes.Items {
	class TownPortalScrollItem : ModItem {
		public static void OpenPortal( Player player, Vector2 rightPos, Vector2 leftPos ) {
			WormholesPlayer modplayer = player.GetModPlayer<WormholesPlayer>();
			if( modplayer.MyPortal != null ) {
				modplayer.MyPortal.Close();
			}

			var link = new TownPortalLink( Color.Cyan, rightPos, leftPos );
			link.LeftPortal.AnimateOpen();
			link.RightPortal.AnimateOpen();

			modplayer.MyPortal = link;
		}


		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Scroll of Town Portal" );
			this.Tooltip.SetDefault( "Creates a temporary portal between here and home"+'\n'+
				"Still no cow level." );
		}

		public override void SetDefaults() {
			this.item.UseSound = SoundID.Item46;
			this.item.useTurn = true;
			this.item.useStyle = 4;
			this.item.useTime = 30;
			this.item.useAnimation = 30;
			this.item.maxStack = 30;
			this.item.consumable = true;
			this.item.width = 35;
			this.item.height = 14;
			this.item.value = 10000;
			this.item.rare = 1;
		}
		
		public override bool UseItem( Player player ) {
			if( player.itemAnimation > 0 && player.itemTime == 0 ) {
				player.itemTime = this.item.useTime;
				return true;
			}
			return base.UseItem( player );
		}

		public override bool ConsumeItem( Player player ) {
			var playerPos = player.Center;
			var homePos = PlayerWarpHelpers.GetSpawnPoint( player );
			playerPos.X -= WormholePortal.Width / 2;
			playerPos.Y -= 128 + player.height + 1;
			homePos.X -= WormholePortal.Width / 2;
			homePos.Y -= 128 + player.height + 1;

			TownPortalScrollItem.OpenPortal( player, playerPos, homePos );

			return base.ConsumeItem( player );
		}

		public override void AddRecipes() {
			ModRecipe recipe = new TownPortalScrollRecipe( this );
			recipe.AddRecipe();

			/*ModRecipe alt_wormhole_pot = new ModRecipe( this.mod );
			alt_wormhole_pot.AddIngredient( ItemID.RecallPotion, 1 );
			alt_wormhole_pot.AddIngredient( ItemID.Blinkroot, 1 );
			alt_wormhole_pot.SetResult( ItemID.WormholePotion" );
			alt_wormhole_pot.AddTile( 13 ); // Bottle
			alt_wormhole_pot.AddRecipe();

			ModRecipe alt_recall_pot = new ModRecipe( this.mod );
			alt_recall_pot.AddIngredient( ItemID.WormholePotion, 1 );
			alt_recall_pot.AddIngredient( ItemID.Daybloom, 1 );
			alt_recall_pot.SetResult( ItemID.RecallPotion );
			alt_recall_pot.AddTile( 13 ); // Bottle
			alt_recall_pot.AddRecipe();*/
		}
	}




	class TownPortalScrollRecipe : ModRecipe {
		public TownPortalScrollRecipe( TownPortalScrollItem moditem ) : base( moditem.mod ) {
			this.AddTile( 18 );   // Crafting bench
			
			this.AddRecipeGroup( "WormholesMod:EvacPotions", 3 );
			this.AddRecipeGroup( "WormholesMod:BasicBooks", 1 );
			//this.AddIngredient( ItemID.WormholePotion, 1 );
			this.AddIngredient( ItemID.ManaCrystal, 1 );
			this.SetResult( moditem, WormholesConfig.Instance.TownPortalRecipeQuantity );
		}

		public override bool RecipeAvailable() {
			return WormholesConfig.Instance.CraftableTownPortalScrolls;
		}
	}
}

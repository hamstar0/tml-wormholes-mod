using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Utils;


namespace Wormholes.Items {
	class TownPortalScrollItem : ModItem {
		public static void OpenPortal( WormholesMod mymod, Player player, Vector2 to, Vector2 fro ) {
			WormholesPlayer modplayer = player.GetModPlayer<WormholesPlayer>( mymod );
			if( modplayer.MyPortal != null ) {
				modplayer.MyPortal.Close();
			}

			var link = new WormholeLink( mymod, Color.Cyan, to, fro );
			link.LeftPortal.AnimateOpen();
			link.RightPortal.AnimateOpen();

			modplayer.MyPortal = link;
		}


		public override void SetDefaults() {
			this.item.name = "Scroll of Town Portal";
			this.item.UseSound = SoundID.Item46;
			this.item.useTurn = true;
			this.item.useStyle = 4;
			this.item.useTime = 30;
			this.item.useAnimation = 30;
			this.item.maxStack = 30;
			this.item.consumable = true;
			this.item.width = 35;
			this.item.height = 14;
			this.item.toolTip = "Creates a temporary portal between here and home";
			this.item.toolTip2 = "Still no cow level.";
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
			var player_pos = player.Center;
			var home_pos = PlayerHelper.GetSpawnPoint( player );
			player_pos.X -= WormholePortal.Width / 2;
			player_pos.Y -= 128 + player.height + 1;
			home_pos.X -= WormholePortal.Width / 2;
			home_pos.Y -= 128 + player.height + 1;

			TownPortalScrollItem.OpenPortal( (WormholesMod)this.mod, player, player_pos, home_pos );

			return base.ConsumeItem( player );
		}

		public override void AddRecipes() {
			ModRecipe recipe = new TownPortalScrollRecipe( this );
			recipe.AddRecipe();
			
			/*ModRecipe alt_wormhole_pot = new ModRecipe( this.mod );
			alt_wormhole_pot.AddIngredient( "Recall Potion", 1 );
			alt_wormhole_pot.AddIngredient( "Blinkroot", 1 );
			alt_wormhole_pot.SetResult( "Wormhole Potion" );
			alt_wormhole_pot.AddTile( 13 ); // Bottle
			alt_wormhole_pot.AddRecipe();

			ModRecipe alt_recall_pot = new ModRecipe( this.mod );
			alt_recall_pot.AddIngredient( "Wormhole Potion", 1 );
			alt_recall_pot.AddIngredient( "Daybloom", 1 );
			alt_recall_pot.SetResult( "Recall Potion" );
			alt_recall_pot.AddTile( 13 ); // Bottle
			alt_recall_pot.AddRecipe();*/
		}
	}



	class TownPortalScrollRecipe : ModRecipe {
		public TownPortalScrollRecipe( TownPortalScrollItem moditem ) : base( moditem.mod ) {
			this.AddTile( 18 );   // Crafting bench

			this.AddIngredient( "Book", 1 );
			this.AddRecipeGroup( "WormholesMod:EvacPotions", 1 );
			//this.AddIngredient( "Wormhole Potion", 1 );
			this.AddIngredient( "Mana Crystal", 1 );
			this.SetResult( moditem, 1 );
		}

		public override bool RecipeAvailable() {
			var mymod = (WormholesMod)this.mod;
			return mymod.Config.Data.CraftableTownPortalScrolls;
		}
	}
}

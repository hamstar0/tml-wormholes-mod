using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Wormholes.Projectiles;


namespace Wormholes.Items {
	class ChaosBombItem : ModItem {
		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Chaos Bomb" );
			this.Tooltip.SetDefault( "Randomly scatters blocks within blast radius"+'\n'+
				"20% chance to permanently relocate a wormholes" );
		}

		public override void SetDefaults() {
			this.item.useStyle = 1;
			this.item.shootSpeed = 5f;
			this.item.shoot = this.mod.ProjectileType<ChaosBombProjectile>();
			this.item.width = 20;
			this.item.height = 20;
			this.item.maxStack = 99;
			this.item.consumable = true;
			this.item.UseSound = SoundID.Item1;
			this.item.useAnimation = 25;
			this.item.useTime = 25;
			this.item.noUseGraphic = true;
			this.item.noMelee = true;
			this.item.damage = 0;
			this.item.value = Item.buyPrice( 0, 0, 20, 0 );
			this.item.rare = 1;
		}
		
		public override void AddRecipes() {
			ModRecipe recipe = new ChaosBombRecipe( this );
			recipe.AddRecipe();
		}
	}



	class ChaosBombRecipe : ModRecipe {
		public ChaosBombRecipe( ChaosBombItem moditem ) : base( moditem.mod ) {
			var mymod = (WormholesMod)this.mod;

			this.AddTile( 18 );   // Crafting bench

			this.AddRecipeGroup( "WormholesMod:EvacPotions", 1 );
			this.AddIngredient( ItemID.BouncyBomb, mymod.Config.ChaosBombRecipeBouncyBombCost );
			this.AddIngredient( ItemID.Amethyst, 1 );
			this.AddIngredient( ItemID.Topaz, 1 );
			this.AddIngredient( ItemID.Sapphire, 1 );
			this.AddIngredient( ItemID.Emerald, 1 );
			this.AddIngredient( ItemID.Ruby, 1 );
			this.AddIngredient( ItemID.Diamond, 1 );
			this.AddIngredient( ItemID.Amber, 1 );
			//this.AddIngredient( ItemID.Glass, 10 );
			this.SetResult( moditem, mymod.Config.ChaosBombRecipeQuantity );
		}

		public override bool RecipeAvailable() {
			var mymod = (WormholesMod)this.mod;
			return mymod.Config.CraftableTownPortalScrolls;
		}
	}
}

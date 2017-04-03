using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Wormholes.Projectiles;


namespace Wormholes.Items {
	class ChaosBombItem : ModItem {
		public override void SetDefaults() {
			this.item.name = "Chaos Bomb";
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
			this.item.toolTip = "Randomly scatters blocks within blast radius";
			this.item.toolTip2 = "20% chance to permanently relocate a wormholes";
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
			this.AddTile( 18 );   // Crafting bench

			this.AddIngredient( "Bouncy Bomb", 5 );
			this.AddRecipeGroup( "WormholesMod:EvacPotions", 1 );
			this.AddIngredient( "Amethyst", 1 );
			this.AddIngredient( "Topaz", 1 );
			this.AddIngredient( "Sapphire", 1 );
			this.AddIngredient( "Emerald", 1 );
			this.AddIngredient( "Ruby", 1 );
			this.AddIngredient( "Diamond", 1 );
			this.AddIngredient( "Amber", 1 );
			//this.AddIngredient( "Glass", 10 );
			this.SetResult( moditem, 5 );
		}

		public override bool RecipeAvailable() {
			var mymod = (WormholesMod)this.mod;
			return mymod.Config.Data.CraftableTownPortalScrolls;
		}
	}
}

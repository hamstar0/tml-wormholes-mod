using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace Wormholes {
	public class WormholesConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		public bool DebugModeInfo = false;

		public bool DebugModeMapCheat = false;

		public bool DebugModeReset = false;


		[DefaultValue( 4 )]
		public int TinyWorldPortals = 4;    // SmallWorldPortals / 2

		[DefaultValue( 8 )]
		public int SmallWorldPortals = 8;  // 4200 x 1200 = 5040000

		[DefaultValue( 14 )]
		public int MediumWorldPortals = 14; // 6400 x 1800 = 11520000

		[DefaultValue( 20 )]
		public int LargeWorldPortals = 20;  // 8400 x 2400 = 20160000

		[DefaultValue( 27 )]
		public int HugeWorldPortals = 27;


		[DefaultValue( true )]
		public bool CraftableTownPortalScrolls = true;

		[DefaultValue( 5 )]
		public int TownPortalRecipeQuantity = 5;

		[DefaultValue( 60 * 60 )]
		public int TownPortalDuration = 60 * 60;    // 1 hour

		[DefaultValue( true )]
		public bool TownPortalConsumesOnReturn = true;


		[DefaultValue( 0.45f )]
		public float WormholeSoundVolume = 0.45f;

		[DefaultValue( 1.25f )]
		public float WormholeLightScale = 1.25f;

		[DefaultValue( 0.9f )]
		public float WormholeEntrySoundVolume = 0.9f;


		public bool DisableNaturalWormholes = false;


		[DefaultValue( 5 )]
		public int ChaosBombRecipeBouncyBombCost = 5;

		[DefaultValue( 25 )]
		public int ChaosBombRecipeQuantity = 25;

		[DefaultValue( 5 )]
		public int ChaosBombWormholeCloseOdds = 5;

		[DefaultValue( 4 )]
		public int ChaosBombRadius = 4;

		[DefaultValue( 32 )]
		public int ChaosBombScatterRadius = 32;
	}
}

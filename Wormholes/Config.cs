using HamstarHelpers.Classes.UI.ModConfig;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace Wormholes {
	class MyFloatInputElement : FloatInputElement { }




	public class WormholesConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		public bool DebugModeInfo = false;

		public bool DebugModeMapCheat = false;

		public bool DebugModeReset = false;


		[Range( 0, 100 )]
		[DefaultValue( 4 )]
		public int TinyWorldPortals = 4;    // SmallWorldPortals / 2

		[Range( 0, 100 )]
		[DefaultValue( 8 )]
		public int SmallWorldPortals = 8;  // 4200 x 1200 = 5040000

		[Range( 0, 100 )]
		[DefaultValue( 14 )]
		public int MediumWorldPortals = 14; // 6400 x 1800 = 11520000

		[Range( 0, 100 )]
		[DefaultValue( 20 )]
		public int LargeWorldPortals = 20;  // 8400 x 2400 = 20160000

		[Range( 0, 100 )]
		[DefaultValue( 27 )]
		public int HugeWorldPortals = 27;


		[DefaultValue( true )]
		public bool CraftableTownPortalScrolls = true;

		[Range( 0, 99 )]
		[DefaultValue( 5 )]
		public int TownPortalRecipeQuantity = 5;

		[Label( "Town portal duration (in seconds)" )]
		[Range( 0, 60 * 60 * 24 )]
		[DefaultValue( 60 * 60 )]
		public int TownPortalDuration = 60 * 60;    // 1 hour

		[DefaultValue( true )]
		public bool TownPortalConsumesOnReturn = true;


		[Range( 0f, 10f )]
		[DefaultValue( 0.45f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float WormholeSoundVolume = 0.45f;

		[Range( 0f, 100f )]
		[DefaultValue( 1.25f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float WormholeLightScale = 1.25f;

		[Range( 0f, 10f )]
		[DefaultValue( 0.9f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float WormholeEntrySoundVolume = 0.9f;


		public bool DisableNaturalWormholes = false;


		[Range( 0, 99 )]
		[DefaultValue( 5 )]
		public int ChaosBombRecipeBouncyBombCost = 5;

		[Range( 0, 99 )]
		[DefaultValue( 25 )]
		public int ChaosBombRecipeQuantity = 25;

		[Range( 0, 1000 )]
		[DefaultValue( 5 )]
		public int ChaosBombWormholeCloseOdds = 5;

		[Range( 0, 100 )]
		[DefaultValue( 4 )]
		public int ChaosBombRadius = 4;

		[Range( 0, 1000 )]
		[DefaultValue( 32 )]
		public int ChaosBombScatterRadius = 32;
	}
}

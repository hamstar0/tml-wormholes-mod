using HamstarHelpers.Components.Config;
using System;


namespace Wormholes {
	public class WormholesConfigData : ConfigurationDataBase {
		public readonly static string ConfigFileName = "Wormholes Config.json";


		////////////////

		public string VersionSinceUpdate = "";

		public bool DebugModeInfo = false;
		public bool DebugModeMapCheat = false;
		public bool DebugModeReset = false;

		public int TinyWorldPortals = 4;    // SmallWorldPortals / 2
		public int SmallWorldPortals = 8;  // 4200 x 1200 = 5040000
		public int MediumWorldPortals = 14; // 6400 x 1800 = 11520000
		public int LargeWorldPortals = 20;  // 8400 x 2400 = 20160000
		public int HugeWorldPortals = 27;

		public bool CraftableTownPortalScrolls = true;
		public int TownPortalRecipeQuantity = 5;
		public int TownPortalDuration = 60 * 60;    // 1 hour
		public bool TownPortalConsumesOnReturn = true;

		public float WormholeSoundVolume = 0.45f;
		public float WormholeLightScale = 1.25f;
		public float WormholeEntrySoundVolume = 0.9f;

		public bool DisableNaturalWormholes = false;

		public int ChaosBombRecipeBouncyBombCost = 5;
		public int ChaosBombRecipeQuantity = 25;
		public int ChaosBombWormholeCloseOdds = 5;
		public int ChaosBombRadius = 4;
		public int ChaosBombScatterRadius = 32;



		////////////////

		public void SetDefaults() { }

		////

		public bool CanUpdateVersion() {
			if( this.VersionSinceUpdate == "" ) { return true; }
			var versSince = new Version( this.VersionSinceUpdate );
			return versSince < WormholesMod.Instance.Version;
		}

		public void UpdateToLatestVersion() {
			var mymod = WormholesMod.Instance;
			var newConfig = new WormholesConfigData();
			newConfig.SetDefaults();

			var versSince = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( this.VersionSinceUpdate == "" ) {
				this.SetDefaults();
			}

			this.VersionSinceUpdate = mymod.Version.ToString();
		}
	}
}

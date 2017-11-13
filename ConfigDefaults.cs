using HamstarHelpers.Utilities.Config;
using System;

namespace Wormholes {
	public class WormholesConfigData : ConfigurationDataBase {
		public static readonly Version ConfigVersion = new Version( 1, 6, 4 );
		public readonly static string ConfigFileName = "Wormholes Config.json";


		////////////////

		public string VersionSinceUpdate = "";

		public int DEBUGFLAGS = 0;  // 1: Info;  2: View wormholes on map;  4: Reset wormholes

		public int TinyWorldPortals = 4;    // SmallWorldPortals / 2
		public int SmallWorldPortals = 8;  // 4200 x 1200 = 5040000
		public int MediumWorldPortals = 14; // 6400 x 1800 = 11520000
		public int LargeWorldPortals = 20;  // 8400 x 2400 = 20160000
		public int HugeWorldPortals = 27;

		public bool CraftableTownPortalScrolls = true;
		public int TownPortalDuration = 60 * 60;    // 1 hour

		public float WormholeSoundVolume = 0.45f;
		public float WormholeLightScale = 1.25f;
		public float WormholeEntrySoundVolume = 0.9f;

		public bool DisableNaturalWormholes = false;

		public int ChaosBombWormholeCloseOdds = 5;
		public int ChaosBombRadius = 4;
		public int ChaosBombScatterRadius = 32;


		////////////////

		public bool UpdateToLatestVersion() {
			var new_config = new WormholesConfigData();
			var vers_since = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( vers_since >= WormholesConfigData.ConfigVersion ) {
				return false;
			}

			if( vers_since < new Version( 1, 5, 0 ) ) {
				this.SmallWorldPortals = new WormholesConfigData().SmallWorldPortals;
				this.MediumWorldPortals = new WormholesConfigData().MediumWorldPortals;
				this.LargeWorldPortals = new WormholesConfigData().LargeWorldPortals;
				this.HugeWorldPortals = new WormholesConfigData().HugeWorldPortals;
			}
			if( vers_since < new Version( 1, 6, 0 ) ) {
				this.WormholeSoundVolume = new WormholesConfigData().WormholeSoundVolume;
			}
			if( vers_since < new Version( 1, 6, 4 ) ) {
				WormholeManager.ForceRegenWormholes = true;
			}

			this.VersionSinceUpdate = WormholesConfigData.ConfigVersion.ToString();

			return true;
		}
	}
}

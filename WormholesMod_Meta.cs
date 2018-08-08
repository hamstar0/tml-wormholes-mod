using HamstarHelpers.Components.Config;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Wormholes {
	partial class WormholesMod : Mod {
		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-wormholes-mod"; } }

		public static string ConfigFileRelativePath {
			get { return ConfigurationDataBase.RelativePath + Path.DirectorySeparatorChar + WormholesConfigData.ConfigFileName; }
		}
		public static void ReloadConfigFromFile() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reload configs outside of single player." );
			}
			if( WormholesMod.Instance != null ) {
				if( !WormholesMod.Instance.ConfigJson.LoadFile() ) {
					WormholesMod.Instance.ConfigJson.SaveFile();
				}
			}
		}

		public static void ResetConfigFromDefaults() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reset to default configs outside of single player." );
			}

			var new_config = new WormholesConfigData();
			//new_config.SetDefaults();

			WormholesMod.Instance.ConfigJson.SetData( new_config );
			WormholesMod.Instance.ConfigJson.SaveFile();
		}
	}
}

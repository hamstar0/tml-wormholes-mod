using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;


namespace Wormholes {
	public static partial class WormholesAPI {
		public static bool WormholesFinishedSpawning {
			get {
				var myworld = ModContent.GetInstance<WormholesWorld>();
				return myworld.Wormholes.WormholesFinishedSpawning;
			}
		}

		////////////////

		public static int GetWormholeCount() {
			var myworld = ModContent.GetInstance<WormholesWorld>();
			return myworld.Wormholes.Links.Count;
		}

		public static IEnumerable<WormholeLink> GetWormholes() {
			var myworld = ModContent.GetInstance<WormholesWorld>();
			return (IEnumerable<WormholeLink>)myworld.Wormholes.Links.GetEnumerator();
		}

		////////////////

		public static void RandomizeWormhole( WormholeLink link ) {
			var myworld = ModContent.GetInstance<WormholesWorld>();
			myworld.Wormholes.Reroll( link );
		}

		////////////////

		public static bool AddWormhole( Color color ) {
			var myworld = ModContent.GetInstance<WormholesWorld>();
			var link = myworld.Wormholes.CreateRandomWormholePair( color );
			if( link == null ) {
				return false;
			}

			myworld.Wormholes.Links.Add( link );
			return true;
		}

		public static void RemoveWormhole( WormholeLink link ) {
			var myworld = ModContent.GetInstance<WormholesWorld>();

			if( myworld.Wormholes.Links.Remove( link ) ) {
				link.Close();
			}
		}
	}
}

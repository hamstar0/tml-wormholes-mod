using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Wormholes {
	public static partial class WormholesAPI {
		public static bool WormholesFinishedSpawning {
			get {
				var myworld = WormholesMod.Instance.GetModWorld<WormholesWorld>();
				return myworld.Wormholes.WormholesFinishedSpawning;
			}
		}

		////////////////

		public static int GetWormholeCount() {
			var myworld = WormholesMod.Instance.GetModWorld<WormholesWorld>();
			return myworld.Wormholes.Links.Count;
		}

		public static IEnumerable<WormholeLink> GetWormholes() {
			var myworld = WormholesMod.Instance.GetModWorld<WormholesWorld>();
			return (IEnumerable<WormholeLink>)myworld.Wormholes.Links.GetEnumerator();
		}

		////////////////

		public static void RandomizeWormhole( WormholeLink link ) {
			var myworld = WormholesMod.Instance.GetModWorld<WormholesWorld>();
			myworld.Wormholes.Reroll( link );
		}

		////////////////

		public static void AddWormhole( Color color ) {
			var myworld = WormholesMod.Instance.GetModWorld<WormholesWorld>();
			var link = myworld.Wormholes.CreateRandomWormholePair( color );

			myworld.Wormholes.Links.Add( link );
		}

		public static void RemoveWormhole( WormholeLink link ) {
			var myworld = WormholesMod.Instance.GetModWorld<WormholesWorld>();

			if( myworld.Wormholes.Links.Remove( link ) ) {
				link.Close();
			}
		}
	}
}

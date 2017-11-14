using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Wormholes {
	public static class WormholesAPI {
		public static WormholeModContext GetModContext() {
			return WormholesMod.Instance.Context;
		}

		public static WormholesConfigData GetModSettings() {
			return WormholesMod.Instance.Config.Data;
		}

		////////////////

		public static bool WormholesFinishedSpawning {
			get {
				var myworld = WormholesMod.Instance.GetModWorld<MyWorld>();
				return myworld.Wormholes.WormholesFinishedSpawning;
			}
		}

		////////////////

		public static int GetWormholeCount() {
			var myworld = WormholesMod.Instance.GetModWorld<MyWorld>();
			return myworld.Wormholes.Links.Count;
		}

		public static IEnumerable<WormholeLink> GetWormholes() {
			var myworld = WormholesMod.Instance.GetModWorld<MyWorld>();
			return (IEnumerable<WormholeLink>)myworld.Wormholes.Links.GetEnumerator();
		}

		////////////////

		public static void RandomizeWormhole( WormholeLink link ) {
			var myworld = WormholesMod.Instance.GetModWorld<MyWorld>();
			myworld.Wormholes.Reroll( link );
		}

		////////////////

		public static void AddWormhole( Color color ) {
			var myworld = WormholesMod.Instance.GetModWorld<MyWorld>();
			var link = myworld.Wormholes.CreateRandomWormholePair( color );

			myworld.Wormholes.Links.Add( link );
		}

		public static void RemoveWormhole( WormholeLink link ) {
			var myworld = WormholesMod.Instance.GetModWorld<MyWorld>();

			if( myworld.Wormholes.Links.Remove( link ) ) {
				link.Close();
			}
		}
	}
}

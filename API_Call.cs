using Microsoft.Xna.Framework;
using System;


namespace Wormholes {
	public static partial class WormholesAPI {
		internal static object Call( string call_type, params object[] args ) {
			WormholeLink link;
			Color color;

			switch( call_type ) {
			case "GetModContext":
				return WormholesAPI.GetModContext();
			case "GetModSettings":
				return WormholesAPI.GetModSettings();
			case "WormholesFinishedSpawning":
				return WormholesAPI.WormholesFinishedSpawning;
			case "GetWormholeCount":
				return WormholesAPI.GetWormholeCount();
			case "GetWormholes":
				return WormholesAPI.GetWormholes();
			case "RandomizeWormhole":
				if( args.Length == 0 || !(args[1] is WormholeLink) ) {
					throw new ArgumentException( "Invalid argument for "+call_type+"; must be a WormholeLink" );
				}

				link = (WormholeLink)args[1];

				WormholesAPI.RandomizeWormhole( link );
				return null;
			case "AddWormhole":
				if( args.Length == 0 || !( args[1] is WormholeLink ) ) {
					throw new ArgumentException( "Invalid argument for " + call_type + "; must be a Color" );
				}

				color = (Color)args[1];

				WormholesAPI.AddWormhole( color );
				return null;
			case "RemoveWormhole":
				if( args.Length == 0 || !( args[1] is WormholeLink ) ) {
					throw new ArgumentException( "Invalid argument for " + call_type + "; must be a WormholeLink" );
				}

				link = (WormholeLink)args[1];

				WormholesAPI.RemoveWormhole( link );
				return null;
			}
			
			throw new Exception("No such api call "+call_type);
		}
	}
}

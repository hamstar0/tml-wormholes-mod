using Terraria;


namespace Wormholes.Utils {
	public class DustHelper {
		public static bool IsActive( int who ) {
			return who != 6000 && Main.dust[who].active && Main.dust[who].type != 0;
		}
	}
}

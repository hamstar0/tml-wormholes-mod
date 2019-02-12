using Microsoft.Xna.Framework;
using Terraria;


namespace Wormholes {
	public class TownPortalLink : WormholeLink {
		public TownPortalLink( Color color, Vector2 leftNodePos, Vector2 rightNodePos ) :
			base( color, leftNodePos, rightNodePos ) { }


		protected override void TeleportToLeft( Player player ) {
			base.TeleportToLeft( player );

			var mymod = WormholesMod.Instance;

			if( mymod.Config.TownPortalConsumesOnReturn ) {
				this.Close();
			}
		}
	}
}

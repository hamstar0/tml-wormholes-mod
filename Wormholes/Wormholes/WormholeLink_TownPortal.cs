using Microsoft.Xna.Framework;
using Terraria;


namespace Wormholes {
	public class TownPortalLink : WormholeLink {
		public TownPortalLink( Color color, Vector2 leftNodePos, Vector2 rightNodePos ) :
			base( color, leftNodePos, rightNodePos ) { }


		protected override void TeleportToLeft( Player player ) {
			base.TeleportToLeft( player );

			if( WormholesConfig.Instance.TownPortalConsumesOnReturn ) {
				this.Close();
			}
		}
	}
}

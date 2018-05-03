using Microsoft.Xna.Framework;
using Terraria;


namespace Wormholes {
	public class TownPortalLink : WormholeLink {
		public TownPortalLink( Color color, Vector2 left_node_pos, Vector2 right_node_pos ) :
			base( color, left_node_pos, right_node_pos ) { }


		protected override void TeleportToLeft( Player player ) {
			base.TeleportToLeft( player );

			var mymod = WormholesMod.Instance;

			if( mymod.Config.TownPortalConsumesOnReturn ) {
				this.Close();
			}
		}
	}
}

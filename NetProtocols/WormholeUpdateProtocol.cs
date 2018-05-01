using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Utilities.Network;
using Microsoft.Xna.Framework;


namespace Wormholes.NetProtocols {
	class WormholeUpdateProtocol : PacketProtocol {
		public static void BroadcastToClients( string id ) {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();

			WormholeLink link = myworld.Wormholes.GetLinkById( id );
			if( link == null ) {
				LogHelpers.Log( "WormholeUpdateProtocol.BroadcastToClients - Invalid wormhole link id " + id );
				return;
			}

			var protocol = new WormholeUpdateProtocol( id, link.RightPortal.Pos.X, link.RightPortal.Pos.Y, link.LeftPortal.Pos.X, link.LeftPortal.Pos.Y );
			protocol.SendToClient( -1, -1 );
		}


		////////////////

		public string Id;
		public float RightPosX;
		public float RightPosY;
		public float LeftPosX;
		public float LeftPosY;


		////////////////

		private WormholeUpdateProtocol( string id, float r_pos_x, float r_pos_y, float l_pos_x, float l_pos_y ) {
			this.Id = id;
			this.RightPosX = r_pos_x;
			this.RightPosY = r_pos_y;
			this.LeftPosX = l_pos_x;
			this.LeftPosY = l_pos_y;
		}


		////////////////

		protected override void ReceiveWithClient() {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();
			var mngr = mymod.GetModWorld<WormholesWorld>().Wormholes;

			Vector2 pos_r = new Vector2( this.RightPosX, this.RightPosY );
			Vector2 pos_l = new Vector2( this.LeftPosX, this.LeftPosY );

			var link = mngr.GetLinkById( this.Id );
			link.ChangePosition( pos_r, pos_l );
		}
	}
}

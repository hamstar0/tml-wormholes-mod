using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;


namespace Wormholes.NetProtocols {
	class WormholeUpdateProtocol : PacketProtocolSentToEither {
		public static void BroadcastToClients( string id ) {
			var mymod = WormholesMod.Instance;
			var myworld = ModContent.GetInstance<WormholesWorld>();

			WormholeLink link = myworld.Wormholes.GetLinkById( id );
			if( link == null ) {
				LogHelpers.Alert( "Invalid wormhole link id " + id );
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

		private WormholeUpdateProtocol() { }

		public WormholeUpdateProtocol( string id, float rPosX, float rPosY, float lPosX, float lPosY ) {
			this.Id = id;
			this.RightPosX = rPosX;
			this.RightPosY = rPosY;
			this.LeftPosX = lPosX;
			this.LeftPosY = lPosY;
		}


		////////////////

		protected override void ReceiveOnClient() {
			var mymod = WormholesMod.Instance;
			var myworld = ModContent.GetInstance<WormholesWorld>();
			var mngr = ModContent.GetInstance<WormholesWorld>().Wormholes;

			var posR = new Vector2( this.RightPosX, this.RightPosY );
			var posL = new Vector2( this.LeftPosX, this.LeftPosY );

			var link = mngr.GetLinkById( this.Id );
			link.ChangePosition( posR, posL );
		}

		protected override void ReceiveOnServer( int fromWho ) {
			throw new System.NotImplementedException();
		}
	}
}

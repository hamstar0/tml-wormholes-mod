using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using Microsoft.Xna.Framework;


namespace Wormholes.NetProtocols {
	class WormholesProtocol : PacketProtocolRequestToServer {
		public static void QuickRequest() {
			PacketProtocolRequestToServer.QuickRequestToServer<WormholesProtocol>( -1 );
		}



		////////////////

		public string[] Ids;
		public float[] RightPosX;
		public float[] RightPosY;
		public float[] LeftPosX;
		public float[] LeftPosY;



		////////////////

		private WormholesProtocol() { }

		////////////////

		protected override void InitializeServerSendData( int who ) {
			var mymod = WormholesMod.Instance;
			var modworld = mymod.GetModWorld<WormholesWorld>();

			// Be sure our wormholes are ready to send (if not already)
			modworld.SetupWormholes();

			int count = WormholeManager.PortalCount;

			this.Ids = new string[count];
			this.RightPosX = new float[count];
			this.RightPosY = new float[count];
			this.LeftPosX = new float[count];
			this.LeftPosY = new float[count];

			for( int i=0; i<count; i++ ) {
				var link = modworld.Wormholes.Links[i];

				this.Ids[i] = link.ID;
				this.RightPosX[i] = link.RightPortal.Pos.X;
				this.RightPosY[i] = link.RightPortal.Pos.Y;
				this.LeftPosX[i] = link.LeftPortal.Pos.X;
				this.LeftPosY[i] = link.LeftPortal.Pos.Y;
			}
		}

		////////////////

		protected override void ReceiveReply() {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();

			for( int i=0; i<this.Ids.Length; i++ ) {
				var posR = new Vector2( this.RightPosX[i], this.RightPosY[i] );
				var posL = new Vector2( this.LeftPosX[i], this.LeftPosY[i] );

				var link = new WormholeLink( this.Ids[i], WormholeLink.GetColor(i), posL, posR );
				myworld.Wormholes.Links.Insert( i, link );
			}
		}
	}
}

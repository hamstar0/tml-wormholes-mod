using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;


namespace Wormholes.NetProtocols {
	class WormholeRerollProtocol : PacketProtocolSentToEither {
		public static void ClientRequestReroll( string id ) {
			WormholeRerollProtocol protocol = new WormholeRerollProtocol( id );
			protocol.SendToServer( false );
		}



		////////////////

		public string ID;



		////////////////

		private WormholeRerollProtocol() { }

		public WormholeRerollProtocol( string id ) {
			this.ID = id;
		}


		////////////////

		protected override void ReceiveOnClient() {
			throw new System.NotImplementedException();
		}

		protected override void ReceiveOnServer( int fromWho ) {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();
			var mngr = myworld.Wormholes;

			var link = mngr.GetLinkById( this.ID );
			mngr.Reroll( link );

			WormholeUpdateProtocol.BroadcastToClients( this.ID );
		}
	}
}

using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Utilities.Network;


namespace Wormholes.NetProtocols {
	class WormholeRerollProtocol : PacketProtocol {
		public static void ClientRequestReroll( string id ) {
			var protocol = new WormholeRerollProtocol {
				Id = id
			};

			protocol.SendToServer( false );
		}



		////////////////

		public string Id;


		////////////////
		
		protected override void ReceiveWithServer( int from_who ) {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();
			var mngr = myworld.Wormholes;

			var link = mngr.GetLinkById( this.Id );
			mngr.Reroll( link );

			WormholeUpdateProtocol.BroadcastToClients( this.Id );
		}
	}
}

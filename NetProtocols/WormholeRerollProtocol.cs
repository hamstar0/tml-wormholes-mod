using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;


namespace Wormholes.NetProtocols {
	class WormholeRerollProtocol : PacketProtocol {
		public static void ClientRequestReroll( string id ) {
			var protocol = new WormholeRerollProtocol( id );

			protocol.SendToServer( false );
		}



		////////////////

		public string ID;


		////////////////

		private WormholeRerollProtocol( PacketProtocolDataConstructorLock ctor_lock ) { }

		private WormholeRerollProtocol( string id ) {
			this.ID = id;
		}

		////////////////

		protected override void ReceiveWithServer( int from_who ) {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();
			var mngr = myworld.Wormholes;

			var link = mngr.GetLinkById( this.ID );
			mngr.Reroll( link );

			WormholeUpdateProtocol.BroadcastToClients( this.ID );
		}
	}
}

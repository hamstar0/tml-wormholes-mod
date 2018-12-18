using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;


namespace Wormholes.NetProtocols {
	class WormholeRerollProtocol : PacketProtocolSentToEither {
		protected class MyFactory : Factory<WormholeRerollProtocol> {
			public string ID;

			public MyFactory( string id ) {
				this.ID = id;
			}

			protected override void Initialize( WormholeRerollProtocol data ) {
				data.ID = this.ID;
			}
		}



		////////////////

		public static void ClientRequestReroll( string id ) {
			var factory = new MyFactory( id );
			WormholeRerollProtocol protocol = factory.Create();

			protocol.SendToServer( false );
		}



		////////////////

		public string ID;


		////////////////

		protected WormholeRerollProtocol( PacketProtocolDataConstructorLock ctor_lock ) : base( ctor_lock ) { }

		////////////////

		protected override void ReceiveOnClient() {
			throw new System.NotImplementedException();
		}

		protected override void ReceiveOnServer( int from_who ) {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();
			var mngr = myworld.Wormholes;

			var link = mngr.GetLinkById( this.ID );
			mngr.Reroll( link );

			WormholeUpdateProtocol.BroadcastToClients( this.ID );
		}
	}
}

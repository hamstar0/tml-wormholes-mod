using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;
using Microsoft.Xna.Framework;


namespace Wormholes.NetProtocols {
	class WormholeUpdateProtocol : PacketProtocolSentToEither {
		protected class MyFactory : Factory<WormholeUpdateProtocol> {
			public string Id;
			public float RightPosX;
			public float RightPosY;
			public float LeftPosX;
			public float LeftPosY;

			public MyFactory( string id, float r_pos_x, float r_pos_y, float l_pos_x, float l_pos_y ) {
				this.Id = id;
				this.RightPosX = r_pos_x;
				this.RightPosY = r_pos_y;
				this.LeftPosX = l_pos_x;
				this.LeftPosY = l_pos_y;
			}

			protected override void Initialize( WormholeUpdateProtocol data ) {
				data.Id = this.Id;
				data.RightPosX = this.RightPosX;
				data.RightPosY = this.RightPosY;
				data.LeftPosX = this.LeftPosX;
				data.LeftPosY = this.LeftPosY;
			}
		}



		////////////////

		public static void BroadcastToClients( string id ) {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();

			WormholeLink link = myworld.Wormholes.GetLinkById( id );
			if( link == null ) {
				LogHelpers.Log( "WormholeUpdateProtocol.BroadcastToClients - Invalid wormhole link id " + id );
				return;
			}

			var factory = new MyFactory( id, link.RightPortal.Pos.X, link.RightPortal.Pos.Y, link.LeftPortal.Pos.X, link.LeftPortal.Pos.Y );
			WormholeUpdateProtocol protocol = factory.Create();

			protocol.SendToClient( -1, -1 );
		}


		////////////////

		public string Id;
		public float RightPosX;
		public float RightPosY;
		public float LeftPosX;
		public float LeftPosY;


		////////////////

		protected WormholeUpdateProtocol( PacketProtocolDataConstructorLock ctor_lock ) : base( ctor_lock ) { }


		////////////////

		protected override void ReceiveOnClient() {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();
			var mngr = mymod.GetModWorld<WormholesWorld>().Wormholes;

			Vector2 pos_r = new Vector2( this.RightPosX, this.RightPosY );
			Vector2 pos_l = new Vector2( this.LeftPosX, this.LeftPosY );

			var link = mngr.GetLinkById( this.Id );
			link.ChangePosition( pos_r, pos_l );
		}

		protected override void ReceiveOnServer( int fromWho ) {
			throw new System.NotImplementedException();
		}
	}
}

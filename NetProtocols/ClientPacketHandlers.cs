using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Wormholes.NetProtocol {
	static class ClientPacketHandlers {
		public static void HandlePacket( WormholesMod mymod, BinaryReader reader ) {
			WormholeNetProtocolTypes protocol = (WormholeNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case WormholeNetProtocolTypes.WormholesAndModSettings:
				ClientPacketHandlers.ReceiveWormholesAndSettingsOnClient( mymod, reader );
				break;
			case WormholeNetProtocolTypes.WormholeUpdate:
				ClientPacketHandlers.ReceiveWormholeUpdateOnClient( mymod, reader );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}



		////////////////////////////////
		// Senders (Client)
		////////////////////////////////

		public static void SendWormholesAndSettingsRequestViaClient( WormholesMod mymod, Player player ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			ModPacket packet = mymod.GetPacket();
			packet.Write( (byte)WormholeNetProtocolTypes.RequestWormholesAndModSettings );
			packet.Send();
		}

		public static void SendWormholeRerollRequestViaClient( WormholesMod mymod, string id ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			ModPacket packet = mymod.GetPacket();
			packet.Write( (byte)WormholeNetProtocolTypes.RequestWormholeReroll );
			packet.Write( (string)id );
			packet.Send();
		}


		////////////////////////////////
		// Recipients (Client)
		////////////////////////////////

		private static void ReceiveWormholesAndSettingsOnClient( WormholesMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			WormholesWorld modworld = mymod.GetModWorld<WormholesWorld>();
			string json = reader.ReadString();
			int wormhole_count = reader.ReadInt32();

			mymod.Config.DeserializeMe( json );

			for( int i = 0; i < wormhole_count; i++ ) {
				string id = reader.ReadString();
				float worm_l_x = reader.ReadSingle();
				float worm_l_y = reader.ReadSingle();
				float worm_r_x = reader.ReadSingle();
				float worm_r_y = reader.ReadSingle();

				Vector2 pos_l = new Vector2( worm_l_x, worm_l_y );
				Vector2 pos_r = new Vector2( worm_r_x, worm_r_y );

				var link = new WormholeLink( id, mymod, WormholeLink.GetColor( i ), pos_l, pos_r );
				modworld.Wormholes.Links.Insert( i, link );
			}
		}

		private static void ReceiveWormholeUpdateOnClient( WormholesMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { return; } // Clients only

			string id = reader.ReadString();
			float worm_r_x = reader.ReadSingle();
			float worm_r_y = reader.ReadSingle();
			float worm_l_y = reader.ReadSingle();
			float worm_l_x = reader.ReadSingle();

			Vector2 pos_l = new Vector2( worm_r_x, worm_l_y );
			Vector2 pos_r = new Vector2( worm_l_x, worm_r_y );

			var mngr = mymod.GetModWorld<WormholesWorld>().Wormholes;
			var link = mngr.GetLinkById( id );
			link.ChangePosition( pos_r, pos_l );
		}
	}
}

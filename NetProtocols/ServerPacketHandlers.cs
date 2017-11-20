using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Wormholes.NetProtocol {
	static class ServerPacketHandlers {
		public static void HandlePacket( WormholesMod mymod, BinaryReader reader, int player_who ) {
			WormholeNetProtocolTypes protocol = (WormholeNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case WormholeNetProtocolTypes.RequestWormholesAndModSettings:
				ServerPacketHandlers.ReceiveWormholesAndSettingsRequestOnServer( mymod, reader, player_who );
				break;
			case WormholeNetProtocolTypes.RequestWormholeReroll:
				ServerPacketHandlers.ReceiveWormholeRerollRequestOnServer( mymod, reader, player_who );
				break;
			default:
				ErrorLogger.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}


		
		////////////////////////////////
		// Senders (Server)
		////////////////////////////////

		public static void SendWormholesAndSettingsViaServer( WormholesMod mymod, Player player ) {
			if( Main.netMode != 2 ) { return; } // Server only

			WormholesWorld modworld = mymod.GetModWorld<WormholesWorld>();
			ModPacket packet = mymod.GetPacket();

			// Be sure our wormholes are ready to send (if not already)
			modworld.SetupWormholes();

			packet.Write( (byte)WormholeNetProtocolTypes.WormholesAndModSettings );
			packet.Write( (string)mymod.Config.SerializeMe() );
			packet.Write( (int)WormholeManager.PortalCount );

			for( int i = 0; i < WormholeManager.PortalCount; i++ ) {
				var link = modworld.Wormholes.Links[i];
				packet.Write( (string)link.ID.ToString() );
				packet.Write( (float)link.LeftPortal.Pos.X );
				packet.Write( (float)link.LeftPortal.Pos.Y );
				packet.Write( (float)link.RightPortal.Pos.X );
				packet.Write( (float)link.RightPortal.Pos.Y );
			}
			packet.Send( player.whoAmI );
		}

		public static void BroadcastWormholeUpdateViaServer( WormholesMod mymod, string id ) {
			if( Main.netMode != 2 ) { return; } // Server only

			WormholesWorld modworld = mymod.GetModWorld<WormholesWorld>();
			WormholeLink link = modworld.Wormholes.GetLinkById( id );
			ModPacket packet = mymod.GetPacket();

			if( link == null ) {
				ErrorLogger.Log( "WormholesNetProtocol.BroadcastWormholeUpdateViaServer - Invalid wormhole link id." );
				return;
			}

			packet.Write( (byte)WormholeNetProtocolTypes.WormholeUpdate );
			packet.Write( (string)id );
			packet.Write( (float)link.RightPortal.Pos.X );
			packet.Write( (float)link.RightPortal.Pos.Y );
			packet.Write( (float)link.LeftPortal.Pos.X );
			packet.Write( (float)link.LeftPortal.Pos.Y );

			packet.Send();	// Broadcast
		}

		
		////////////////////////////////
		// Recipients (Server)
		////////////////////////////////

		private static void ReceiveWormholeRerollRequestOnServer( WormholesMod mymod, BinaryReader reader, int player_who ) {
			if( Main.netMode != 2 ) { return; } // Server only
			
			string id = reader.ReadString();
			if( id == "" ) {
				ErrorLogger.Log( "WormholesNetProtocol.ReceiveWormholeRerollRequestOnServer - Blank id." );
				return;
			}

			var mngr = mymod.GetModWorld<WormholesWorld>().Wormholes;
			var link = mngr.GetLinkById( id );
			mngr.Reroll( link );

			ServerPacketHandlers.BroadcastWormholeUpdateViaServer( mymod, id );
		}

		private static void ReceiveWormholesAndSettingsRequestOnServer( WormholesMod mymod, BinaryReader reader, int player_who ) {
			if( Main.netMode != 2 ) { return; }	// Server only

			ServerPacketHandlers.SendWormholesAndSettingsViaServer( mymod, Main.player[player_who] );
		}
	}
}

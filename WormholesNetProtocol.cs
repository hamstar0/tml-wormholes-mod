using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace Wormholes {
	public enum WormholeNetProtocolTypes : byte {
		RequestWormholesAndModSettings,
		RequestWormholeReroll,
		WormholesAndModSettings,
		WormholeUpdate
	}
	

	public static class WormholesNetProtocol {
		public static void RoutePacket( WormholesMod mymod, BinaryReader reader ) {
			WormholeNetProtocolTypes protocol = (WormholeNetProtocolTypes)reader.ReadByte();

			switch( protocol ) {
			case WormholeNetProtocolTypes.RequestWormholesAndModSettings:
				WormholesNetProtocol.ReceiveWormholesAndSettingsRequestOnServer( mymod, reader );
				break;
			case WormholeNetProtocolTypes.RequestWormholeReroll:
				WormholesNetProtocol.ReceiveWormholeRerollRequestOnServer( mymod, reader );
				break;
			case WormholeNetProtocolTypes.WormholesAndModSettings:
				WormholesNetProtocol.ReceiveWormholesAndSettingsOnClient( mymod, reader );
				break;
			case WormholeNetProtocolTypes.WormholeUpdate:
				WormholesNetProtocol.ReceiveWormholeUpdateOnClient( mymod, reader );
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
			packet.Write( (int)player.whoAmI );
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
		// Senders (Server)
		////////////////////////////////

		public static void SendWormholesAndSettingsViaServer( WormholesMod mymod, Player player ) {
			if( Main.netMode != 2 ) { return; } // Server only

			WormholesWorld modworld = mymod.GetModWorld<WormholesWorld>();
			ModPacket packet = mymod.GetPacket();

			// Be sure our wormholes are ready to send (if not already)
			modworld.Wormholes.SetupWormholes( mymod );

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


		////////////////////////////////
		// Recipients (Server)
		////////////////////////////////

		private static void ReceiveWormholeRerollRequestOnServer( WormholesMod mymod, BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; } // Server only
			
			string id = reader.ReadString();
			if( id == "" ) {
				ErrorLogger.Log( "WormholesNetProtocol.ReceiveWormholeRerollRequestOnServer - Blank id." );
				return;
			}

			var mngr = mymod.GetModWorld<WormholesWorld>().Wormholes;
			var link = mngr.GetLinkById( id );
			mngr.Reroll( link );

			WormholesNetProtocol.BroadcastWormholeUpdateViaServer( mymod, id );
		}

		private static void ReceiveWormholesAndSettingsRequestOnServer( WormholesMod mymod, BinaryReader reader ) {
			if( Main.netMode != 2 ) { return; }	// Server only

			int who = reader.ReadInt32();
			if( who < 0 || who >= Main.player.Length || Main.player[who] == null ) {
				ErrorLogger.Log( "WormholesNetProtocol.ReceiveWormholesRequestOnServer - Invalid player whoAmI. " + who );
				return;
			}

			WormholesNetProtocol.SendWormholesAndSettingsViaServer( mymod, Main.player[who] );
		}
	}
}

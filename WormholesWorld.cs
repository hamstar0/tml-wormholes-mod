using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace Wormholes {
	public class WormholesWorld : ModWorld {
		public WormholeManager Wormholes { get; private set; }
		public string ID { get; private set; }

		public bool HasCorrectID { get; private set; }



		public override void Initialize() {
			this.Wormholes = new WormholeManager( (WormholesMod)this.mod );
			this.ID = Guid.NewGuid().ToString( "D" );
			this.HasCorrectID = false;	// 'Load()' decides if no pre-existing one is found
		}


		public override void Load( TagCompound tags ) {
			string id = tags.GetString( "world_id" );
			if( id.Length > 0 ) { this.ID = id; }
			this.HasCorrectID = true;

			var mymod = (WormholesMod)this.mod;
			if( this.Wormholes.Load( mymod, tags ) ) {
				this.Wormholes.SetupWormholes( mymod );
			}

			WormholeManager.ForceRegenWormholes = false;
		}


		public override TagCompound Save() {
			var tags = this.Wormholes.Save();
			tags.Set( "world_id", this.ID );
			return tags;
		}


		public override void NetSend( BinaryWriter writer ) {
			writer.Write( this.ID );
		}

		public override void NetReceive( BinaryReader reader ) {
			this.ID = reader.ReadString();
			this.HasCorrectID = true;

			var modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>( this.mod );
			
			if( modplayer.HasEnteredWorld && !modplayer.HasLoadedTownPortals ) {
				modplayer.ReopenTownPortal();
			}
		}


		public override void PostDrawTiles() {
			if( this.Wormholes == null ) { return; }
			
			var myplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>( this.mod );
			var mymod = (WormholesMod)this.mod;

			Main.spriteBatch.Begin();
			try {
				this.Wormholes.DrawAll( mymod, myplayer.MyPortal );
			} catch( Exception e ) {
				ErrorLogger.Log( "PostDrawTiles: " + e.ToString() );
				throw e;
			}
			Main.spriteBatch.End();
		}
	}
}

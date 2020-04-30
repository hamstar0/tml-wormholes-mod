using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;


namespace Wormholes {
	partial class WormholesMod : Mod {
		public override void PostDrawInterface( SpriteBatch sb ) {
			// Clients and single only (redundant?)
			if( Main.netMode == 2 ) { return; }
			
			try {
				if( !Main.mapFullscreen && (Main.mapStyle == 1 || Main.mapStyle == 2) ) {
					this.DrawMiniMap( sb );
				}
			} catch( Exception e ) {
				ErrorLogger.Log( "PostDrawInterface: " + e.ToString() );
				throw e;
			}
		}
		
		public override void PostDrawFullscreenMap( ref string mouseText ) {
			// Clients and single only (redundant?)
			if( Main.netMode == 2 ) { return; }

			try {
				this.DrawFullMap( Main.spriteBatch );
			} catch( Exception e ) {
				ErrorLogger.Log( "PostDrawFullscreenMap: " + e.ToString() );
				throw e;
			}
		}


		////////////////

		private void DrawMiniMap( SpriteBatch sb ) {
			this.UI.Update();

			WormholesWorld modworld = ModContent.GetInstance<WormholesWorld>();
			WormholesPlayer curr_modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>();

			if( !this.Config.DisableNaturalWormholes ) {
				if( modworld.Wormholes != null ) {
					for( int i = 0; i < modworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = modworld.Wormholes.Links[i];
						if( link == null ) { break; }

						if( Main.mapStyle == 1 ) {
							this.UI.DrawMiniMap( link, sb );
						} else {
							this.UI.DrawOverlayMap( link, sb );
						}
					}
				}
			}
			
			if( curr_modplayer.MyPortal != null ) {
				if( Main.mapStyle == 1 ) {
					this.UI.DrawMiniMap( curr_modplayer.MyPortal, sb );
				} else {
					this.UI.DrawOverlayMap( curr_modplayer.MyPortal, sb );
				}
			}
		}


		private void DrawFullMap( SpriteBatch sb ) {
			this.UI.Update();

			WormholesWorld modworld = ModContent.GetInstance<WormholesWorld>();
			WormholesPlayer curr_modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>();

			if( !this.Config.DisableNaturalWormholes ) {
				if( modworld.Wormholes != null ) {
					for( int i = 0; i < modworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = modworld.Wormholes.Links[i];
						if( link == null ) { break; }

						this.UI.DrawFullscreenMap( link, sb );
					}
				}
			}

			if( curr_modplayer.MyPortal != null ) {
				this.UI.DrawFullscreenMap( curr_modplayer.MyPortal, sb );
			}
		}
	}
}

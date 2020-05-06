using HamstarHelpers.Helpers.Debug;
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
				LogHelpers.Warn( e.ToString() );
				throw e;
			}
		}
		
		public override void PostDrawFullscreenMap( ref string mouseText ) {
			// Clients and single only (redundant?)
			if( Main.netMode == 2 ) { return; }

			try {
				this.DrawFullMap( Main.spriteBatch );
			} catch( Exception e ) {
				LogHelpers.Warn( e.ToString() );
				throw e;
			}
		}


		////////////////

		private void DrawMiniMap( SpriteBatch sb ) {
			this.UI.Update();

			WormholesWorld myworld = ModContent.GetInstance<WormholesWorld>();
			WormholesPlayer myplayer = Main.LocalPlayer.GetModPlayer<WormholesPlayer>();

			if( !this.Config.DisableNaturalWormholes ) {
				if( myworld.Wormholes != null ) {
					for( int i = 0; i < myworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = myworld.Wormholes.Links[i];
						if( link == null ) { break; }

						if( Main.mapStyle == 1 ) {
							this.UI.DrawMiniMap( link, sb );
						} else {
							this.UI.DrawOverlayMap( link, sb );
						}
					}
				}
			}
			
			if( myplayer.MyPortal != null ) {
				if( Main.mapStyle == 1 ) {
					this.UI.DrawMiniMap( myplayer.MyPortal, sb );
				} else {
					this.UI.DrawOverlayMap( myplayer.MyPortal, sb );
				}
			}
		}


		private void DrawFullMap( SpriteBatch sb ) {
			this.UI.Update();

			WormholesWorld myworld = ModContent.GetInstance<WormholesWorld>();
			WormholesPlayer myplayer = Main.LocalPlayer.GetModPlayer<WormholesPlayer>();

			if( !this.Config.DisableNaturalWormholes ) {
				if( myworld.Wormholes != null ) {
					for( int i = 0; i < myworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = myworld.Wormholes.Links[i];
						if( link == null ) { break; }

						this.UI.DrawFullscreenMap( link, sb );
					}
				}
			}

			if( myplayer.MyPortal != null ) {
				this.UI.DrawFullscreenMap( myplayer.MyPortal, sb );
			}
		}
	}
}

using HamstarHelpers.Helpers.HudHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Wormholes.Utils;


namespace Wormholes {
	class WormholesUI {
		private static Texture2D Tex = null;


		internal static void Initialize() {
			if( Main.netMode != 2 ) {
				WormholesUI.Tex = WormholesMod.Instance.GetTexture( "Wormholes/MiniWormhole" );
			}
		}


		////////////////

		private SpriteAnimator TexAnim;


		////////////////

		public WormholesUI() {
			if( Main.netMode == 2 ) {
				throw new Exception("Cannot create class instance on server.");
			}
			this.TexAnim = new SpriteAnimator( 12, 4, WormholesUI.Tex, Color.White );
		}

		////////////////

		public void Update() {
			this.TexAnim.Animate();
		}

		////////////////

		public void DrawMiniMap( WormholeLink link, SpriteBatch sb ) {
			var mymod = WormholesMod.Instance;
			if( !link.IsCharted( Main.LocalPlayer ) && !mymod.Config.DebugModeMapCheat ) { return; }

			float scale = Main.mapMinimapScale / 1.5f;
			Texture2D tex = WormholesUI.Tex;

			Rectangle l_rect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, tex.Width, tex.Height );
			Rectangle r_rect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, tex.Width, tex.Height );

			Vector2? l_pos = HudMapHelpers.GetMiniMapPosition( l_rect );
			if( l_pos != null ) {
				Color l_color = link.LeftPortal.BaseColor * Main.mapMinimapAlpha;
				sb.Draw( tex, (Vector2)l_pos, this.TexAnim.Frame, l_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}

			Vector2? r_pos = HudMapHelpers.GetMiniMapPosition( r_rect );
			if( r_pos != null ) {
				Color r_color = link.RightPortal.BaseColor * Main.mapMinimapAlpha;
				sb.Draw( tex, (Vector2)r_pos, this.TexAnim.Frame, r_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}
		}

		public void DrawOverlayMap( WormholeLink link, SpriteBatch sb ) {
			var mymod = WormholesMod.Instance;
			if( !link.IsCharted( Main.LocalPlayer ) && !mymod.Config.DebugModeMapCheat ) { return; }

			float scale = Main.mapOverlayScale / 1.5f;
			Texture2D tex = WormholesUI.Tex;

			Rectangle l_rect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, tex.Width, tex.Height );
			Rectangle r_rect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, tex.Width, tex.Height );

			Vector2? l_pos = HudMapHelpers.GetOverlayMapPosition( l_rect );
			if( l_pos != null ) {
				Color l_color = link.LeftPortal.BaseColor * Main.mapOverlayAlpha;
				sb.Draw( tex, (Vector2)l_pos, this.TexAnim.Frame, l_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}

			Vector2? r_pos = HudMapHelpers.GetOverlayMapPosition( r_rect );
			if( r_pos != null ) {
				Color r_color = link.RightPortal.BaseColor * Main.mapOverlayAlpha;
				sb.Draw( tex, (Vector2)r_pos, this.TexAnim.Frame, r_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}
		}

		public void DrawFullscreenMap( WormholeLink link, SpriteBatch sb ) {
			var mymod = WormholesMod.Instance;
			if( !link.IsCharted( Main.LocalPlayer ) && !mymod.Config.DebugModeMapCheat ) { return; }

			float scale = Main.mapFullscreenScale / 1.5f;
			Texture2D tex = WormholesUI.Tex;

			Rectangle l_rect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, tex.Width, tex.Height );
			Vector2 l_pos = HudMapHelpers.GetFullMapPosition( l_rect );
			sb.Draw( tex, l_pos, this.TexAnim.Frame, link.LeftPortal.BaseColor, 0f, new Vector2 { }, scale, SpriteEffects.None, 1f );

			Rectangle r_rect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, tex.Width, tex.Height );
			Vector2 r_pos = HudMapHelpers.GetFullMapPosition( r_rect );
			sb.Draw( tex, r_pos, this.TexAnim.Frame, link.RightPortal.BaseColor, 0f, new Vector2 { }, scale, SpriteEffects.None, 1f );
		}
	}
}

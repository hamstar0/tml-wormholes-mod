using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Utils;
using Wormholes.Utils;

namespace Wormholes {
	class WormholesUI {
		private Texture2D Tex;
		private SpriteAnimator TexAnim;


		public WormholesUI( WormholesMod mymod ) {
			this.Tex = mymod.GetTexture( "Wormholes/MiniWormhole" );
			this.TexAnim = new SpriteAnimator( 12, 4, this.Tex, Color.White );
		}


		public void Update() {
			this.TexAnim.Animate();
		}
		

		public void DrawMiniMap( WormholeLink link, SpriteBatch sb ) {
			if( !Debug.DEBUGMODE && !link.IsCharted( Main.player[Main.myPlayer] ) ) { return; }
			float scale = Main.mapMinimapScale / 1.5f;

			Rectangle l_rect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, this.Tex.Width, this.Tex.Height );
			Rectangle r_rect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, this.Tex.Width, this.Tex.Height );
				
			Vector2? l_pos = UIHelper.GetMiniMapPosition( l_rect );
			if( l_pos != null ) {
				Color l_color = link.LeftPortal.BaseColor * Main.mapMinimapAlpha;
				sb.Draw( this.Tex, (Vector2)l_pos, this.TexAnim.Frame, l_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}

			Vector2? r_pos = UIHelper.GetMiniMapPosition( r_rect );
			if( r_pos != null ) {
				Color r_color = link.RightPortal.BaseColor * Main.mapMinimapAlpha;
				sb.Draw( this.Tex, (Vector2)r_pos, this.TexAnim.Frame, r_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}
		}

		public void DrawOverlayMap( WormholeLink link, SpriteBatch sb ) {
			if( !Debug.DEBUGMODE && !link.IsCharted( Main.player[Main.myPlayer] ) ) { return; }
			float scale = Main.mapOverlayScale / 1.5f;

			Rectangle l_rect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, this.Tex.Width, this.Tex.Height );
			Rectangle r_rect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, this.Tex.Width, this.Tex.Height );

			Vector2? l_pos = UIHelper.GetOverlayMapPosition( l_rect );
			if( l_pos != null ) {
				Color l_color = link.LeftPortal.BaseColor * Main.mapOverlayAlpha;
				sb.Draw( this.Tex, (Vector2)l_pos, this.TexAnim.Frame, l_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}

			Vector2? r_pos = UIHelper.GetOverlayMapPosition( r_rect );
			if( r_pos != null ) {
				Color r_color = link.RightPortal.BaseColor * Main.mapOverlayAlpha;
				sb.Draw( this.Tex, (Vector2)r_pos, this.TexAnim.Frame, r_color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}
		}

		public void DrawFullscreenMap( WormholeLink link, SpriteBatch sb ) {
			if( !Debug.DEBUGMODE && !link.IsCharted( Main.player[Main.myPlayer] ) ) { return; }
			float scale = Main.mapFullscreenScale / 1.5f;

			Rectangle l_rect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, this.Tex.Width, this.Tex.Height );
			Vector2 l_pos = UIHelper.GetFullMapPosition( l_rect );
			sb.Draw( this.Tex, l_pos, this.TexAnim.Frame, link.LeftPortal.BaseColor, 0f, new Vector2 { }, scale, SpriteEffects.None, 1f );

			Rectangle r_rect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, this.Tex.Width, this.Tex.Height );
			Vector2 r_pos = UIHelper.GetFullMapPosition( r_rect );
			sb.Draw( this.Tex, r_pos, this.TexAnim.Frame, link.RightPortal.BaseColor, 0f, new Vector2 { }, scale, SpriteEffects.None, 1f );
		}
	}
}

﻿using HamstarHelpers.Helpers.HUD;
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
			if( !link.IsCharted( Main.LocalPlayer ) && !WormholesConfig.Instance.DebugModeMapCheat ) { return; }

			float scale = Main.mapMinimapScale / 1.5f;
			Texture2D tex = WormholesUI.Tex;

			Rectangle lRect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, tex.Width, tex.Height );
			Rectangle rRect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, tex.Width, tex.Height );

			var lPosData = HUDMapHelpers.GetMiniMapPositionAsScreenPosition( lRect );
			if( lPosData.IsOnScreen ) {
				Color lColor = link.LeftPortal.BaseColor * Main.mapMinimapAlpha;
				sb.Draw( tex, lPosData.Item1, this.TexAnim.Frame, lColor, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}

			var rPosData = HUDMapHelpers.GetMiniMapPositionAsScreenPosition( rRect );
			if( rPosData.IsOnScreen ) {
				Color rColor = link.RightPortal.BaseColor * Main.mapMinimapAlpha;
				sb.Draw( tex, rPosData.Item1, this.TexAnim.Frame, rColor, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}
		}

		public void DrawOverlayMap( WormholeLink link, SpriteBatch sb ) {
			if( !link.IsCharted( Main.LocalPlayer ) && !WormholesConfig.Instance.DebugModeMapCheat ) { return; }

			float scale = Main.mapOverlayScale / 1.5f;
			Texture2D tex = WormholesUI.Tex;

			Rectangle lRect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, tex.Width, tex.Height );
			Rectangle rRect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, tex.Width, tex.Height );

			var lPosData = HUDMapHelpers.GetOverlayMapPositionAsScreenPosition( lRect );
			if( lPosData.IsOnScreen ) {
				Color lColor = link.LeftPortal.BaseColor * Main.mapOverlayAlpha;
				sb.Draw( tex, (Vector2)lPosData.Item1, this.TexAnim.Frame, lColor, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}

			var rPosData = HUDMapHelpers.GetOverlayMapPositionAsScreenPosition( rRect );
			if( rPosData.IsOnScreen ) {
				Color rColor = link.RightPortal.BaseColor * Main.mapOverlayAlpha;
				sb.Draw( tex, rPosData.Item1, this.TexAnim.Frame, rColor, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			}
		}

		public void DrawFullscreenMap( WormholeLink link, SpriteBatch sb ) {
			if( !link.IsCharted( Main.LocalPlayer ) && !WormholesConfig.Instance.DebugModeMapCheat ) { return; }

			float scale = Main.mapFullscreenScale / 1.5f;
			Texture2D tex = WormholesUI.Tex;

			Rectangle lRect = new Rectangle( (int)link.LeftPortal.Pos.X, (int)link.LeftPortal.Pos.Y, tex.Width, tex.Height );
			var lPosData = HUDMapHelpers.GetFullMapPositionAsScreenPosition( lRect );
			if( lPosData.IsOnScreen ) {
				sb.Draw( tex, lPosData.Item1, this.TexAnim.Frame, link.LeftPortal.BaseColor, 0f, new Vector2 { }, scale, SpriteEffects.None, 1f );
			}

			Rectangle rRect = new Rectangle( (int)link.RightPortal.Pos.X, (int)link.RightPortal.Pos.Y, tex.Width, tex.Height );
			var rPosData = HUDMapHelpers.GetFullMapPositionAsScreenPosition( rRect );
			if( rPosData.IsOnScreen ) {
				sb.Draw( tex, rPosData.Item1, this.TexAnim.Frame, link.RightPortal.BaseColor, 0f, new Vector2 { }, scale, SpriteEffects.None, 1f );
			}
		}
	}
}

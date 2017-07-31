using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Wormholes.Utils;


namespace Wormholes {
	public class WormholePortal {
		private static Texture2D Tex;
		public static int FrameCount { get; private set; }
		public static int Width { get; private set; }
		public static int Height { get; private set; }

		private WormholesMod MyMod;

		private Vector2 _pos;
		
		public Color BaseColor { get; private set; }
		public Vector2 Pos { get { return this._pos; } }
		public Rectangle Rect { get; private set; }

		//public int DustWho { get; private set; }

		private int SoundLoopTimer = 0;
		private SpriteAnimator Animator;

		public bool IsClosed { get; private set; }
		public bool IsMisplaced { get; private set; }



		////////////////

		static WormholePortal() {
			// Clients and single only
			if( Main.netMode != 2 ) {
				Mod mod = ModLoader.GetMod( "Wormholes" );
				WormholePortal.Tex = mod.GetTexture( "Wormholes/Wormhole" );
			}

			WormholePortal.FrameCount = 4;
			WormholePortal.Width = 74;  //WormholePortal.Tex.Width;
			WormholePortal.Height = 128; //WormholePortal.Tex.Height / WormholePortal.FrameCount;
		}

		////////////////

		public WormholePortal( WormholesMod mymod, Vector2 pos, Color color ) {
			this.MyMod = mymod;
			this.IsMisplaced = (pos.X / 16f) < 64f || (pos.X / 16f) > (Main.maxTilesX - 64)
				|| (pos.Y / 16f) < Main.worldSurface || (pos.Y / 16f) > (Main.maxTilesY - 220);

			pos.X = MathHelper.Clamp( pos.X, 160, (Main.maxTilesX-10) * 16 );
			pos.Y = MathHelper.Clamp( pos.Y, 160, (Main.maxTilesY-10) * 16 );
//DebugHelpers.Log( "wall of "+color.ToString()+": "+Main.tile[(int)(pos.X/16f)+2, (int)(pos.Y/16f)+4].wall );

			this._pos = pos;
			this.BaseColor = color;
			
			// Clients and single only
			if( Main.netMode != 2 ) {
				this.Rect = new Rectangle( (int)pos.X, (int)pos.Y, WormholePortal.Width, WormholePortal.Height );
				this.Animator = new SpriteAnimator( 1, WormholePortal.FrameCount, WormholePortal.Tex, color );
			}
		}

		public void Close() {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			
			this.Animator = null;
			this.IsClosed = true;
		}

		public void ChangePosition( Vector2 pos ) {
			this._pos = pos;
			this.Rect = new Rectangle( (int)pos.X, (int)pos.Y, this.Rect.Width, this.Rect.Height );
		}

		////////////////

		public int GetOpenAnimation() {
			// Clients and single only
			if( Main.netMode == 2 ) { return 0; }
			if( this.IsClosed ) { return 0; }

			return this.Animator.AnimatingOpen;
		}

		public void AnimateOpen( int? amt=null ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			if( amt == null ) {
				this.Animator.AnimateOpen();
			} else {
				this.Animator.AnimateOpen( (int)amt );
			}
		}
		
		////////////////

		public void DrawForMe() {
			if( this.IsClosed ) { return; }

			//float zoom = Main.GameZoomTarget;
			//float zoom_scr_wid = (float)Main.screenWidth / zoom;
			//float zoom_scr_hei = (float)Main.screenHeight / zoom;
			//float zoom_world_scr_x = Main.screenPosition.X + ((Main.screenWidth - zoom_scr_wid) / 2);
			//float zoom_world_scr_y = Main.screenPosition.Y + ((Main.screenHeight - zoom_scr_hei) / 2);
			//var zoom_world_scr_pos = new Vector2( zoom_world_scr_x, zoom_world_scr_y );
			//var zoom_world_scr_rect = new Rectangle( (int)zoom_world_scr_x, (int)zoom_world_scr_y, (int)zoom_scr_wid, (int)zoom_scr_hei );

			var world_scr_rect = new Rectangle( (int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight );
			if( !this.Rect.Intersects( world_scr_rect ) ) { return; }
			Vector2 world_scr_pos = Main.screenPosition;

			this.Animator.Animate();

			Vector2 offset = this.Animator.GetPositionOffset();
			Vector2 scr_scr_pos = ((this.Pos - world_scr_pos) + offset);//* zoom;
			//Color color = this.Animator.GetColorFlicker();
			Color color = this.BaseColor;
			Vector2 scale = this.Animator.GetScale();//* zoom;

			Main.spriteBatch.Draw( WormholePortal.Tex, scr_scr_pos, this.Animator.Frame, color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			
			Dust.NewDust( this.Pos, this.Rect.Width, this.Rect.Height, 15, 0, 0, 150, color, 1f );
		}

		public void SoundFX() {
			if( this.IsClosed ) { return; }

			// Loop audio
			if( this.SoundLoopTimer++ > 12 ) {
				Main.PlaySound( SoundID.Item24.WithVolume(this.MyMod.Config.Data.WormholeSoundVolume), this.Pos );
				this.SoundLoopTimer = 0;
			}
		}
		
		public void LightFX() {
			if( this.IsClosed ) { return; }
			if( Main.rand == null ) { return; }

			int x = (int)((this.Pos.X + (WormholePortal.Width / 2)) / 16f);
			int y = (int)((this.Pos.Y + (WormholePortal.Height / 2)) / 16f);

			float flicker_scale = 0.5f + this.MyMod.Config.Data.WormholeLightScale * Main.rand.NextFloat();
			float r = flicker_scale * this.BaseColor.R / 255f;
			float g = flicker_scale * this.BaseColor.G / 255f;
			float b = flicker_scale * this.BaseColor.B / 255f;

			// Emit light
			Lighting.AddLight( x, y, r, g, b );
		}
	}
}

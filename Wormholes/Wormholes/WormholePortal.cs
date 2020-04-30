using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Wormholes.Utils;


namespace Wormholes {
	public class WormholePortal {
		private static Texture2D Tex;
		public static int FrameCount { get; private set; }
		public static int Width { get; private set; }
		public static int Height { get; private set; }



		////////////////

		static WormholePortal() {
			WormholePortal.FrameCount = 4;
			WormholePortal.Width = 74;  //WormholePortal.Tex.Width;
			WormholePortal.Height = 128; //WormholePortal.Tex.Height / WormholePortal.FrameCount;
		}

		internal static void Initialize() {
			// Clients and single only
			if( Main.netMode != 2 ) {
				WormholePortal.Tex = WormholesMod.Instance.GetTexture( "Wormholes/Wormhole" );
			}
		}



		////////////////

		public Color BaseColor { get; private set; }
		public Vector2 Pos { get; private set; }
		public Rectangle Rect { get; private set; }

		//public int DustWho { get; private set; }

		private int SoundLoopTimer = 0;
		private SpriteAnimator Animator;

		public bool IsClosed { get; private set; }
		public bool IsMisplaced { get; private set; }



		////////////////

		public WormholePortal( Vector2 worldPos, Color color ) {
			(int minX, int maxX, int minY, int maxY) bounds = WormholesWorld.GetTileBoundsForWormholes();

			this.IsMisplaced = (worldPos.X / 16f) < 64f || (worldPos.X / 16f) > bounds.maxX
				|| (worldPos.Y / 16f) < Main.worldSurface || (worldPos.Y / 16f) > bounds.maxY;
			
			if( Main.maxTilesX > 160 ) {
				worldPos.X = MathHelper.Clamp( worldPos.X, 160, (Main.maxTilesX - 10) * 16 );
			} else {
				worldPos.X = MathHelper.Clamp( worldPos.X, 0, Main.maxTilesX * 16 );
			}
			if( Main.maxTilesY > 160 ) {
				worldPos.Y = MathHelper.Clamp( worldPos.Y, 160, (Main.maxTilesY - 10) * 16 );
			} else {
				worldPos.Y = MathHelper.Clamp( worldPos.Y, 0, Main.maxTilesY * 16 );
			}
//DebugHelpers.Log( "wall of "+color.ToString()+": "+Main.tile[(int)(pos.X/16f)+2, (int)(pos.Y/16f)+4].wall );

			this.Pos = worldPos;
			this.BaseColor = color;
			
			// Clients and single only
			if( Main.netMode != 2 ) {
				this.Rect = new Rectangle( (int)worldPos.X, (int)worldPos.Y, WormholePortal.Width, WormholePortal.Height );
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
			this.Pos = pos;
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
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			//float zoom = Main.GameZoomTarget;
			//float zoom_scr_wid = (float)Main.screenWidth / zoom;
			//float zoom_scr_hei = (float)Main.screenHeight / zoom;
			//float zoom_world_scr_x = Main.screenPosition.X + ((Main.screenWidth - zoom_scr_wid) / 2);
			//float zoom_world_scr_y = Main.screenPosition.Y + ((Main.screenHeight - zoom_scr_hei) / 2);
			//var zoom_world_scr_pos = new Vector2( zoom_world_scr_x, zoom_world_scr_y );
			//var zoom_world_scr_rect = new Rectangle( (int)zoom_world_scr_x, (int)zoom_world_scr_y, (int)zoom_scr_wid, (int)zoom_scr_hei );

			var worldScrRect = new Rectangle( (int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight );
			if( !this.Rect.Intersects( worldScrRect ) ) { return; }
			Vector2 worldScrPos = Main.screenPosition;

			this.Animator.Animate();

			Vector2 offset = this.Animator.GetPositionOffset();
			Vector2 scrScrPos = ((this.Pos - worldScrPos) + offset);//* zoom;
			//Color color = this.Animator.GetColorFlicker();
			Color color = this.BaseColor;
			Vector2 scale = this.Animator.GetScale();//* zoom;

			Main.spriteBatch.Draw( WormholePortal.Tex, scrScrPos, this.Animator.Frame, color, 0f, new Vector2(), scale, SpriteEffects.None, 1f );
			
			Dust.NewDust( this.Pos, this.Rect.Width, this.Rect.Height, 15, 0, 0, 150, color, 1f );
		}

		public void SoundFX() {
			if( this.IsClosed ) { return; }

			var mymod = WormholesMod.Instance;

			// Loop audio
			if( this.SoundLoopTimer++ > 12 ) {
				Main.PlaySound( SoundID.Item24.WithVolume( mymod.Config.WormholeSoundVolume), this.Pos );
				this.SoundLoopTimer = 0;
			}
		}
		
		public void LightFX() {
			if( this.IsClosed ) { return; }
			if( Main.rand == null ) { return; }

			var mymod = WormholesMod.Instance;

			int x = (int)((this.Pos.X + (WormholePortal.Width / 2)) / 16f);
			int y = (int)((this.Pos.Y + (WormholePortal.Height / 2)) / 16f);

			float flickerScale = 0.5f + mymod.Config.WormholeLightScale * Main.rand.NextFloat();
			float r = flickerScale * this.BaseColor.R / 255f;
			float g = flickerScale * this.BaseColor.G / 255f;
			float b = flickerScale * this.BaseColor.B / 255f;

			// Emit light
			Lighting.AddLight( x, y, r, g, b );
		}
	}
}

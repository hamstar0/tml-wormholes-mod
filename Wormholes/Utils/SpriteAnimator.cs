using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;


namespace Wormholes.Utils {
	public class SpriteAnimator {
		private int FrameRate;
		private int FrameCount;

		private int CurrentFrameDelay;
		private int CurrentFrameStage;
		private int BaseHeight;

		private Rectangle _frame;
		private Color _myColor;
		public Rectangle Frame => this._frame;
		public Color MyColor => this._myColor;

		private float ColorFlicker = 1f;
		private float Scale = 1f;
		private float Bounce = 1f;

		public int AnimatingOpen { get; private set; }



		////////////////

		public SpriteAnimator( int frameRate, int frameCount, Texture2D tex, Color color ) {
			this.FrameRate = frameRate;
			this.FrameCount = frameCount;
			this.BaseHeight = tex.Height / frameCount;
			this.CurrentFrameStage = 0;
			this._frame = new Rectangle( 0, 0, tex.Width, this.BaseHeight );
			this._myColor = color;

			this.ApplyDistortion();
		}

		////////////////

		public void AnimateOpen( int amt = 15 ) {
			this.AnimatingOpen = amt;
		}

		////////////////

		private void ApplyDistortion() {
			if( Main.rand == null ) { return; }

			this.ColorFlicker = Main.rand.NextFloat();
			this.Bounce = Main.rand.NextFloat();
			this.Scale = 0.9375f + (0.0625f * this.Bounce);
		}

		public void Animate() {
			if( this.AnimatingOpen > 0 ) {
				this.AnimatingOpen--;
			}

			if( this.CurrentFrameDelay-- <= 0 ) {
				this.CurrentFrameDelay = this.FrameRate;
				this.CurrentFrameStage = this.CurrentFrameStage >= this.FrameCount - 1 ? 0 : this.CurrentFrameStage + 1;

				this.ApplyDistortion();
			}

			this._frame.Y = this.BaseHeight * this.CurrentFrameStage;
		}

		////////////////


		public Color GetColorFlicker() {
			float alpha = 0.7f + (0.3f * this.ColorFlicker);
			var color = this._myColor;
			color.R = (byte)((float)this._myColor.R * alpha);
			color.G = (byte)((float)this._myColor.G * alpha);
			color.B = (byte)((float)this._myColor.B * alpha);
			color.A = (byte)((float)this._myColor.A * alpha);

			return color;
		}

		public Vector2 GetScale() {
			if( this.AnimatingOpen > 0 ) {
				return new Vector2( this.Scale / 4, this.Scale / (((float)this.AnimatingOpen * 0.5f) + 1) );
			}

			return new Vector2( this.Scale, this.Scale );
		}

		public Vector2 GetPositionOffset() {
			var offset = new Vector2(
				(0.015625f * (float)this.Frame.Width) - (0.03125f * this.Bounce * (float)this.Frame.Width),
				(0.015625f * (float)this.Frame.Height) - (0.03125f * this.Bounce * (float)this.Frame.Height)
			);

			if( this.AnimatingOpen > 0 ) {
				offset.Y += this.AnimatingOpen * 12;
				offset.X += 3 * (this.Frame.Width / 8);
			}

			return offset;
		}
	}
}

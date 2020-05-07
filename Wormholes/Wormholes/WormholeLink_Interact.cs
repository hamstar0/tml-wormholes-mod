using HamstarHelpers.Helpers.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Wormholes.NetProtocols;


namespace Wormholes {
	public partial class WormholeLink {
		internal void UpdateInteractions( Player player, bool isObstructed, out bool isUponPortal ) {
			isUponPortal = false;
			if( this.IsClosed ) { return; }
			
			int side = this.DetectCollision( player.getRect() );

			if( !isObstructed ) {
				if( side == 1 ) {
					this.TeleportToLeft( player );
				} else if( side == -1 ) {
					this.TeleportToRight( player );
				}
			}

			isUponPortal = side != 0;
		}

		internal void UpdateBehavior( Player player ) {
			if( this.IsClosed ) { return; }
			if( Main.myPlayer != player.whoAmI ) { return; }

			if( Main.netMode != 2 ) {   // Not server
				int lOpenAnim = this.LeftPortal.GetOpenAnimation();
				int rOpenAnim = this.RightPortal.GetOpenAnimation();
				if( lOpenAnim > rOpenAnim ) {
					this.LeftPortal.AnimateOpen( rOpenAnim );
				} else if( lOpenAnim < rOpenAnim ) {
					this.RightPortal.AnimateOpen( lOpenAnim );
				}
				
				this.LeftPortal.SoundFX();
				this.RightPortal.SoundFX();
			}
		}

		////////////////

		public bool IsCharted( Player player ) {
			if( this.IsClosed ) { return false; }

			var modplayer = player.GetModPlayer<WormholesPlayer>();

			if( modplayer.MyPortal != null && this.ID == modplayer.MyPortal.ID ) {
				return true;
			}

			return modplayer.ChartedLinks.Contains( this.ID );
		}
		
		public int DetectCollision( Rectangle rect ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return 0; }
			if( this.IsClosed ) { return 0; }

			int onPortal = 0;

			if( this.LeftPortal.Rect.Intersects( rect ) ) {
				onPortal = -1;
			} else if( this.RightPortal.Rect.Intersects( rect ) ) {
				onPortal = 1;
			}

			return onPortal;
		}

		public virtual void ApplyChaosHit() {
			var mymod = WormholesMod.Instance;

			if( Main.netMode == 0 ) {	// Single
				var mngr = ModContent.GetInstance<WormholesWorld>().Wormholes;
				mngr.Reroll( this );
			} else {    // Non-single
				WormholeRerollProtocol.ClientRequestReroll( this.ID );
			}
		}

		////////////////

		protected virtual void TeleportToLeft( Player player ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			Vector2 dest = new Vector2(
				this.LeftPortal.Pos.X + (WormholePortal.Width / 2) - player.width,
				this.LeftPortal.Pos.Y + (WormholePortal.Height / 2) - player.height );
			
			this.Teleport( player, dest );
		}

		protected virtual void TeleportToRight( Player player ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			Vector2 dest = new Vector2(
				this.RightPortal.Pos.X + (WormholePortal.Width / 2) - player.width,
				this.RightPortal.Pos.Y + (WormholePortal.Height / 2) - player.height );

			this.Teleport( player, dest );
		}


		protected virtual void Teleport( Player player, Vector2 dest ) {
			WormholesPlayer myplayer = player.GetModPlayer<WormholesPlayer>();

			if( myplayer.MyPortal == null || (myplayer.MyPortal != null && this.ID != myplayer.MyPortal.ID) ) {
				myplayer.ChartedLinks.Add( this.ID );
			}

			PlayerWarpHelpers.Teleport( player, dest );

			if( player.FindBuffIndex( BuffID.ChaosState ) != -1 ) {
				int def = player.statDefense;
				player.statDefense = 0;
				var dmg = player.Hurt( PlayerDeathReason.ByOther( 13 ), player.statLifeMax2 / 7, 0 );
				player.statDefense = def;
			}

			player.AddBuff( BuffID.VortexDebuff, (int)(60f * 2.5f) );   // Distorted
			player.AddBuff( BuffID.ChaosState, 60 * 10 );   // Chaos State
			
			float velX = player.velocity.X * 3;
			float velY = player.velocity.Y * 3;

			if( velX > 0 && velX < 1 ) { velX = 1; }
			else if( velX < 0 && velX > 1 ) { velX = -1; }
			if( velY > 0 && velY < 1 ) { velY = 1; }
			else if( velY < 0 && velY > 1 ) { velY = -1; }

			for( int i=0; i<24; i++ ) {
				Dust.NewDust( player.position, player.width, player.height, 245, velX, velY );
			}

			//Main.PlaySound( 2, player.position, 100 );
			var snd = SoundID.Item100.WithVolume( WormholesConfig.Instance.WormholeEntrySoundVolume );
			Main.PlaySound( snd, player.position );
		}
	}
}

using HamstarHelpers.PlayerHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Wormholes.NetProtocols;


namespace Wormholes {
	public partial class WormholeLink {
		public void UpdateInteractions( WormholeModContext ctx, Player player, bool is_obstructed, out bool is_upon_portal ) {
			is_upon_portal = false;
			if( this.IsClosed ) { return; }
			
			int side = this.DetectCollision( player.getRect() );

			if( !is_obstructed ) {
				if( side == 1 ) {
					this.TeleportToLeft( ctx, player );
				} else if( side == -1 ) {
					this.TeleportToRight( ctx, player );
				}
			}

			is_upon_portal = side != 0;
		}

		public void UpdateBehavior( WormholeModContext ctx, Player player ) {
			if( this.IsClosed ) { return; }
			if( Main.myPlayer != player.whoAmI ) { return; }

			if( Main.netMode != 2 ) {   // Not server
				int l_open_anim = this.LeftPortal.GetOpenAnimation();
				int r_open_anim = this.RightPortal.GetOpenAnimation();
				if( l_open_anim > r_open_anim ) {
					this.LeftPortal.AnimateOpen( r_open_anim );
				} else if( l_open_anim < r_open_anim ) {
					this.RightPortal.AnimateOpen( l_open_anim );
				}
				
				this.LeftPortal.SoundFX( ctx );
				this.RightPortal.SoundFX( ctx );
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

			int on_portal = 0;

			if( this.LeftPortal.Rect.Intersects( rect ) ) {
				on_portal = -1;
			} else if( this.RightPortal.Rect.Intersects( rect ) ) {
				on_portal = 1;
			}

			return on_portal;
		}
		
		public void ApplyChaosHit( WormholeModContext ctx ) {
			if( Main.netMode == 0 ) {	// Single
				var mngr = ctx.MyMod.GetModWorld<WormholesWorld>().Wormholes;
				mngr.Reroll( this );
			} else {    // Non-single
				WormholeRerollProtocol.ClientRequestReroll( this.ID );
			}
		}

		////////////////

		public void TeleportToLeft( WormholeModContext ctx, Player player ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			Vector2 dest = new Vector2(
				this.LeftPortal.Pos.X + (WormholePortal.Width / 2) - player.width,
				this.LeftPortal.Pos.Y + (WormholePortal.Height / 2) - player.height );
			
			this.Teleport( ctx, player, dest );
		}

		public void TeleportToRight( WormholeModContext ctx, Player player ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			Vector2 dest = new Vector2(
				this.RightPortal.Pos.X + (WormholePortal.Width / 2) - player.width,
				this.RightPortal.Pos.Y + (WormholePortal.Height / 2) - player.height );

			this.Teleport( ctx, player, dest );
		}


		private void Teleport( WormholeModContext ctx, Player player, Vector2 dest ) {
			WormholesPlayer myplayer = player.GetModPlayer<WormholesPlayer>( ctx.MyMod );
			if( myplayer.MyPortal == null || (myplayer.MyPortal != null && this.ID != myplayer.MyPortal.ID) ) {
				myplayer.ChartedLinks.Add( this.ID );
			}

			PlayerHelpers.Teleport( player, dest );

			if( player.FindBuffIndex(88) != -1 ) {
				int def = player.statDefense;
				player.statDefense = 0;
				var dmg = player.Hurt( PlayerDeathReason.ByOther(13), player.statLifeMax2 / 7, 0 );
				player.statDefense = def;
			}

			player.AddBuff( 164, (int)(60f * 2.5f) );   // Distorted
			player.AddBuff( 88, 60 * 10 );   // Chaos State
			
			float vel_x = player.velocity.X * 3;
			float vel_y = player.velocity.Y * 3;

			if( vel_x > 0 && vel_x < 1 ) { vel_x = 1; }
			else if( vel_x < 0 && vel_x > 1 ) { vel_x = -1; }
			if( vel_y > 0 && vel_y < 1 ) { vel_y = 1; }
			else if( vel_y < 0 && vel_y > 1 ) { vel_y = -1; }

			for( int i=0; i<24; i++ ) {
				Dust.NewDust( player.position, player.width, player.height, 245, vel_x, vel_y );
			}

			//Main.PlaySound( 2, player.position, 100 );
			var snd = SoundID.Item100.WithVolume( ctx.MyMod.Config.WormholeEntrySoundVolume );
			Main.PlaySound( snd, player.position );
		}
	}
}

using HamstarHelpers.TileHelpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;


namespace Wormholes.Projectiles {
	public class ChaosBombProjectile : ModProjectile {
		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Chaos Bomb" );

			this.drawOriginOffsetX = 0;
			this.drawOriginOffsetY = -8;
		}

		public override void SetDefaults() {
			this.projectile.width = 22;
			this.projectile.height = 22;
			this.projectile.aiStyle = 16;
			this.projectile.friendly = true;
			this.projectile.penetrate = -1;
			this.projectile.timeLeft = 180;
			this.projectile.damage = 0;
		}

		public override bool? CanCutTiles() {
			return false;
		}

		public override bool CanDamage() {
			return this.projectile.timeLeft <= 3;
		}


		public override void AI() {
			var mymod = (WormholesMod)this.mod;
			var modworld = this.mod.GetModWorld<WormholesWorld>();
			var modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>( this.mod );
			var rect = this.projectile.getRect();
			int rand = Main.rand.Next( mymod.Config.Data.ChaosBombWormholeCloseOdds );
			var proj = this.projectile;
			
			if( proj.owner == Main.myPlayer && proj.timeLeft <= 3 ) {
				proj.tileCollide = false;
				proj.ai[1] = 0f;
				proj.alpha = 255;
				proj.position.X = proj.position.X + (float)(proj.width / 2);
				proj.position.Y = proj.position.Y + (float)(proj.height / 2);
				proj.width = 128;
				proj.height = 128;
				proj.position.X = proj.position.X - (float)(proj.width / 2);
				proj.position.Y = proj.position.Y - (float)(proj.height / 2);
				proj.damage = 100;
				proj.knockBack = 8f;
			}

			// Worldly wormholes
			foreach( WormholeLink link in modworld.Wormholes.Links ) {
				if( link.DetectCollision(rect) != 0 ) {
					proj.Kill();
					if( rand == 0 ) {
						link.ApplyChaosHit( mymod );
						return;
					}
				}
			}

			// Player's own town portal
			if( modplayer.MyPortal != null && !modplayer.MyPortal.IsClosed ) {
				if( modplayer.MyPortal.DetectCollision(rect) != 0 ) {
					proj.Kill();
					modplayer.MyPortal.Close(); // Town portals only close; never re-randomize (for the current version)
					return;
				}
			}
		}


		public override void Kill( int time_left ) {
			var mymod = (WormholesMod)this.mod;
			Projectile proj = this.projectile;
			Main.PlaySound( SoundID.Item14, proj.position );
			var vec = default( Vector2 );

			for( int i = 0; i < 20; i++ ) {
				int dust_id = Dust.NewDust( new Vector2( proj.position.X, proj.position.Y ), proj.width, proj.height, 31, 0f, 0f, 100, default( Color ), 1.5f );
				Main.dust[dust_id].velocity *= 1.4f;
			}

			for( int i = 0; i < 10; i++ ) {
				int dust_id = Dust.NewDust( new Vector2( proj.position.X, proj.position.Y ), proj.width, proj.height, 6, 0f, 0f, 100, default( Color ), 2.5f );
				Main.dust[dust_id].noGravity = true;
				Main.dust[dust_id].velocity *= 5f;
				dust_id = Dust.NewDust( new Vector2( proj.position.X, proj.position.Y ), proj.width, proj.height, 6, 0f, 0f, 100, default( Color ), 1.5f );
				Main.dust[dust_id].velocity *= 3f;
			}

			int gore_id = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[gore_id].velocity *= 0.4f;
			Gore gore_1 = Main.gore[gore_id];
			gore_1.velocity.X = gore_1.velocity.X + 1f;
			Gore gore_2 = Main.gore[gore_id];
			gore_2.velocity.Y = gore_2.velocity.Y + 1f;
			gore_id = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[gore_id].velocity *= 0.4f;
			Gore gore_3 = Main.gore[gore_id];
			gore_3.velocity.X = gore_3.velocity.X - 1f;
			Gore gore_4 = Main.gore[gore_id];
			gore_4.velocity.Y = gore_4.velocity.Y + 1f;
			gore_id = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[gore_id].velocity *= 0.4f;
			Gore gore_5 = Main.gore[gore_id];
			gore_5.velocity.X = gore_5.velocity.X + 1f;
			Gore gore_6 = Main.gore[gore_id];
			gore_6.velocity.Y = gore_6.velocity.Y - 1f;
			gore_id = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[gore_id].velocity *= 0.4f;
			Gore gore_7 = Main.gore[gore_id];
			gore_7.velocity.X = gore_7.velocity.X - 1f;
			Gore gore_8 = Main.gore[gore_id];
			gore_8.velocity.Y = gore_8.velocity.Y - 1f;

			if( time_left == 0 && proj.owner == Main.myPlayer ) {
				int tile_x = (int)this.projectile.Center.X / 16;
				int tile_y = (int)this.projectile.Center.Y / 16;
				int radius = mymod.Config.Data.ChaosBombRadius;
				this.ScatterTiles( tile_x, tile_y, radius, mymod.Config.Data.ChaosBombScatterRadius );
				
				if( Main.netMode != 0 ) {
					NetMessage.SendData( MessageID.KillProjectile, -1, -1, null, proj.identity, (float)proj.owner, 0f, 0f, 0, 0, 0 );
				}
			}
		}


		public void ScatterTiles( int tile_x, int tile_y, int radius, int scatter_radius ) {
			int to_x, to_y, style = 0;
			Tile fro_tile;

			for( int i=tile_x-radius; i<=tile_x+radius; i++ ) {
				for( int j=tile_y-radius; j<=tile_y+radius; j++ ) {
					float x_dist = i - tile_x;
					float y_dist = j - tile_y;
					if( Math.Sqrt( (x_dist*x_dist)+(y_dist*y_dist) ) > radius ) { continue; }	// Crude

					fro_tile = Main.tile[i, j];
					if( fro_tile == null ) { continue; }
					if( !TileHelpers.IsSolid( fro_tile, true, true ) ) { continue; }
					if( TileHelpers.IsWire( fro_tile ) ) { continue; }
					if( fro_tile.lava() ) { continue; }
					if( TileHelpers.IsNotBombable(i, j) ) { continue; }

					var tile_data = TileObjectData.GetTileData( fro_tile );
					if( tile_data != null && (tile_data.Width > 1 || tile_data.Height > 1) ) { continue; }
					
					if( !TileFinderHelpers.FindNearbyRandomAirTile( tile_x, tile_y, scatter_radius, out to_x, out to_y ) ) {
						break;
					}

					try {
						style = TileObjectData.GetTileStyle( fro_tile );
					} catch( Exception _ ) {
						style = 0;
					}

					int old_type = fro_tile.type;
					WorldGen.KillTile( i, j, false, false, true );
					WorldGen.PlaceTile( to_x, to_y, old_type, true, true, this.projectile.owner, style );

					if( Main.netMode != 0 ) {
						NetMessage.SendData( MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0 );
						NetMessage.SendData( MessageID.TileChange, -1, -1, null, 0, (float)to_x, (float)to_y, 0f, 0, 0, 0 );
					}

					Dust.NewDust( new Vector2(i*16, j*16), 0, 0, 15, 0, 0, 150, Color.Cyan, 1f );
					Dust.NewDust( new Vector2(to_x*16, to_y*16), 0, 0, 15, 0, 0, 150, Color.Cyan, 1f );
				}
			}
		}
	}
}

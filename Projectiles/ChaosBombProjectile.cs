using HamstarHelpers.Helpers.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;


namespace Wormholes.Projectiles {
	class ChaosBombProjectile : ModProjectile {
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
			int rand = Main.rand.Next( mymod.Config.ChaosBombWormholeCloseOdds );
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
						link.ApplyChaosHit();
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


		public override void Kill( int timeLeft ) {
			var mymod = (WormholesMod)this.mod;
			Projectile proj = this.projectile;
			Main.PlaySound( SoundID.Item14, proj.position );
			var vec = default( Vector2 );

			for( int i = 0; i < 20; i++ ) {
				int dustId = Dust.NewDust( new Vector2( proj.position.X, proj.position.Y ), proj.width, proj.height, 31, 0f, 0f, 100, default( Color ), 1.5f );
				Main.dust[dustId].velocity *= 1.4f;
			}

			for( int i = 0; i < 10; i++ ) {
				int dustId = Dust.NewDust( new Vector2( proj.position.X, proj.position.Y ), proj.width, proj.height, 6, 0f, 0f, 100, default( Color ), 2.5f );
				Main.dust[dustId].noGravity = true;
				Main.dust[dustId].velocity *= 5f;
				dustId = Dust.NewDust( new Vector2( proj.position.X, proj.position.Y ), proj.width, proj.height, 6, 0f, 0f, 100, default( Color ), 1.5f );
				Main.dust[dustId].velocity *= 3f;
			}

			int goreId = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[goreId].velocity *= 0.4f;
			Gore gore1 = Main.gore[goreId];
			gore1.velocity.X = gore1.velocity.X + 1f;
			Gore gore2 = Main.gore[goreId];
			gore2.velocity.Y = gore2.velocity.Y + 1f;
			goreId = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[goreId].velocity *= 0.4f;
			Gore gore3 = Main.gore[goreId];
			gore3.velocity.X = gore3.velocity.X - 1f;
			Gore gore4 = Main.gore[goreId];
			gore4.velocity.Y = gore4.velocity.Y + 1f;
			goreId = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[goreId].velocity *= 0.4f;
			Gore gore5 = Main.gore[goreId];
			gore5.velocity.X = gore5.velocity.X + 1f;
			Gore gore6 = Main.gore[goreId];
			gore6.velocity.Y = gore6.velocity.Y - 1f;
			goreId = Gore.NewGore( new Vector2( proj.position.X, proj.position.Y ), vec, Main.rand.Next( 61, 64 ), 1f );
			Main.gore[goreId].velocity *= 0.4f;
			Gore gore7 = Main.gore[goreId];
			gore7.velocity.X = gore7.velocity.X - 1f;
			Gore gore8 = Main.gore[goreId];
			gore8.velocity.Y = gore8.velocity.Y - 1f;

			if( timeLeft == 0 && proj.owner == Main.myPlayer ) {
				int tileX = (int)this.projectile.Center.X / 16;
				int tileY = (int)this.projectile.Center.Y / 16;
				int radius = mymod.Config.ChaosBombRadius;
				this.ScatterTiles( tileX, tileY, radius, mymod.Config.ChaosBombScatterRadius );
				
				if( Main.netMode != 0 ) {
					NetMessage.SendData( MessageID.KillProjectile, -1, -1, null, proj.identity, (float)proj.owner, 0f, 0f, 0, 0, 0 );
				}
			}
		}


		public void ScatterTiles( int tileX, int tileY, int radius, int scatterRadius ) {
			int toX, toY, style = 0;
			Tile froTile;

			for( int i=tileX-radius; i<=tileX+radius; i++ ) {
				for( int j=tileY-radius; j<=tileY+radius; j++ ) {
					float xDist = i - tileX;
					float yDist = j - tileY;
					if( Math.Sqrt( (xDist*xDist)+(yDist*yDist) ) > radius ) { continue; }	// Crude

					froTile = Main.tile[i, j];
					if( froTile == null ) { continue; }
					if( !TileHelpers.IsSolid( froTile, true, true ) ) { continue; }
					if( TileHelpers.IsWire( froTile ) ) { continue; }
					if( froTile.lava() ) { continue; }
					if( TileHelpers.IsNotVanillaBombable(i, j) ) { continue; }

					var tileData = TileObjectData.GetTileData( froTile );
					if( tileData != null && (tileData.Width > 1 || tileData.Height > 1) ) { continue; }
					
					if( !TileFinderHelpers.FindNearbyRandomMatch( TilePattern.NonSolid, tileX, tileY, scatterRadius, out toX, out toY ) ) {
						break;
					}

					try {
						style = TileObjectData.GetTileStyle( froTile );
					} catch( Exception ) {
						style = 0;
					}

					int oldType = froTile.type;
					WorldGen.KillTile( i, j, false, false, true );
					WorldGen.PlaceTile( toX, toY, oldType, true, true, this.projectile.owner, style );

					if( Main.netMode != 0 ) {
						NetMessage.SendData( MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0 );
						NetMessage.SendData( MessageID.TileChange, -1, -1, null, 0, (float)toX, (float)toY, 0f, 0, 0, 0 );
					}

					Dust.NewDust( new Vector2(i*16, j*16), 0, 0, 15, 0, 0, 150, Color.Cyan, 1f );
					Dust.NewDust( new Vector2(toX*16, toY*16), 0, 0, 15, 0, 0, 150, Color.Cyan, 1f );
				}
			}
		}
	}
}

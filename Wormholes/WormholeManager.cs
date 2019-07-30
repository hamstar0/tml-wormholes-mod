using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Tiles;
using HamstarHelpers.Helpers.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace Wormholes {
	class WormholeManager {
		public static int PortalCount { get; private set; }
		public IList<WormholeLink> Links { get; private set; }
		public bool WormholesFinishedSpawning { get; private set; }
		
		////////////////

		public static bool ForceRegenWormholes = false;
		private IDictionary<int, int> BlockPortalCountdown = new Dictionary<int, int>();

		

		////////////////

		public WormholeManager() {
			var mymod = WormholesMod.Instance;

			switch( WorldHelpers.GetSize() ) {
			case WorldSize.SubSmall:
				WormholeManager.PortalCount = mymod.Config.TinyWorldPortals;
				break;
			case WorldSize.Small:
				WormholeManager.PortalCount = mymod.Config.SmallWorldPortals;
				break;
			case WorldSize.Medium:
				WormholeManager.PortalCount = mymod.Config.MediumWorldPortals;
				break;
			case WorldSize.Large:
				WormholeManager.PortalCount = mymod.Config.LargeWorldPortals;
				break;
			case WorldSize.SuperLarge:
				WormholeManager.PortalCount = mymod.Config.HugeWorldPortals;
				break;
			}

			this.Links = new List<WormholeLink>( WormholeManager.PortalCount );
		}

		/////////////////

		public bool Load( TagCompound tags ) {
			var mymod = WormholesMod.Instance;

			if( mymod.Config.DisableNaturalWormholes ) { return false; }
			if( !tags.ContainsKey("wormhole_count") ) { return false; }

			int holes = tags.GetInt( "wormhole_count" );
			if( holes == 0 ) { return false; }

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "Loading world ids (" + Main.netMode + "): " + holes );
			}

			int[] wormLeftX = tags.GetIntArray( "wormhole_left_x" );
			int[] wormLeftY = tags.GetIntArray( "wormhole_left_y" );
			int[] wormRightX = tags.GetIntArray( "wormhole_right_x" );
			int[] wormRightY = tags.GetIntArray( "wormhole_right_y" );

			for( int i = 0; i < holes && i < wormLeftX.Length && i < WormholeManager.PortalCount; i++ ) {
				if( i < this.Links.Count && this.Links[i] != null ) {
					this.Links[i].Close();
				}

				string id = tags.GetString( "wormhole_id_" + i );
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "  world load id: " + id + " (" + i + ")" );
				}

				Vector2 posL = new Vector2( wormLeftX[i], wormLeftY[i] );
				Vector2 posR = new Vector2( wormRightX[i], wormRightY[i] );

				var link = new WormholeLink( id, WormholeLink.GetColor( i ), posL, posR );

				// Failsafe against glitched portals
				if( link.IsMisplaced ) {
					LogHelpers.Log( "Found bad portal. " + i + " " + wormLeftX[i] + "," + wormLeftY[i]
						+ " : " + wormRightX[i] + "," + wormRightY[i] );
					WormholeManager.ForceRegenWormholes = true;
					break;
				}

				this.Links.Insert( i, link );
			}
			return true;
		}


		public TagCompound Save() {
			var mymod = WormholesMod.Instance;
			string[] ids = new string[WormholeManager.PortalCount];
			int[] wormLeftX = new int[WormholeManager.PortalCount];
			int[] wormLeftY = new int[WormholeManager.PortalCount];
			int[] wormRightX = new int[WormholeManager.PortalCount];
			int[] wormRightY = new int[WormholeManager.PortalCount];

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "Save world ids (" + Main.netMode + "): " + WormholeManager.PortalCount );
			}

			int i;
			for( i = 0; i < this.Links.Count; i++ ) {
				var link = this.Links[i];
				if( link == null ) { break; }
				if( link.IsClosed ) { continue; }

				ids[i] = link.ID;
				wormLeftX[i] = (int)link.LeftPortal.Pos.X;
				wormLeftY[i] = (int)link.LeftPortal.Pos.Y;
				wormRightX[i] = (int)link.RightPortal.Pos.X;
				wormRightY[i] = (int)link.RightPortal.Pos.Y;
			}
			
			var tags = new TagCompound {
				{"wormhole_count", i},
				{"wormhole_left_x", wormLeftX},
				{"wormhole_left_y", wormLeftY},
				{"wormhole_right_x", wormRightX},
				{"wormhole_right_y", wormRightY}
			};

			for( i = 0; i < this.Links.Count; i++ ) {
				tags.Set( "wormhole_id_" + i, ids[i] );

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "  world save id: " + ids[i] + " (" + i + ") = "
						+ wormLeftX[i] + "," + wormLeftY[i] + " | " + wormRightX[i] + "," + wormRightY[i] );
				}
			}

			return tags;
		}

		/////////////////

		public WormholeLink GetLinkById( string id ) {
			foreach( var link in this.Links ) {
				if( link.ID == id ) {
					return link;
				}
			}
			return null;
		}
		
		public void Reroll( WormholeLink link ) {
			Vector2 randPos1, randPos2;

			do {
				randPos1 = this.GetRandomClearMapPos();
				randPos2 = this.GetRandomClearMapPos();
			} while( Vector2.Distance( randPos1, randPos2 ) < 2048 );

			link.ChangePosition( randPos1, randPos2 );
		}

		/////////////////

		private Vector2 GetRandomClearMapPos() {
			Vector2 randPos;
			int worldX, worldY;
			bool found = false, isEmpty = false;

			do {
				found = true;

				do {
					worldX = Main.rand.Next( 64, Main.maxTilesX - 64 );
					worldY = Main.rand.Next( (int)Main.worldSurface, Main.maxTilesY - 220 );

					isEmpty = true;
					for( int i=worldX; i<worldX+6; i++ ) {
						for( int j=worldY; j<worldY+8; j++ ) {
							Tile tile = Framing.GetTileSafely( i, j );

							bool _;
							isEmpty = !TileHelpers.IsSolid( tile, true, true ) && !tile.lava() && !TileWallHelpers.IsDungeon(tile, out _);
							if( !isEmpty ) { break; }
						}
						if( !isEmpty ) { break; }
					}
				} while( !isEmpty );
				//} while( Collision.SolidCollision( new Vector2(world_x*16f, world_y*16f), WormholePortal.Width, WormholePortal.Height ) );

				randPos = new Vector2( worldX * 16f, worldY * 16f );

				// Not too close to other portals?
				for( int i = 0; i < this.Links.Count; i++ ) {
					var link = this.Links[i];

					float dist = Vector2.Distance( link.LeftPortal.Pos, randPos );
					if( dist < 2048 ) {
						found = false;
						break;
					}

					dist = Vector2.Distance( link.RightPortal.Pos, randPos );
					if( dist < 2048 ) {
						found = false;
						break;
					}
				}
			} while( !found );

			return randPos;
		}


		public WormholeLink CreateRandomWormholePair( Color color ) {
			Vector2 randPos1, randPos2;

			do {
				randPos1 = this.GetRandomClearMapPos();
				randPos2 = this.GetRandomClearMapPos();
			} while( Vector2.Distance( randPos1, randPos2 ) < 2048 );

			return new WormholeLink( color, randPos1, randPos2 );
		}


		//public void AddWormholePair( WormholeLink link ) {
		//	this.Links.Add( link );
		//	WormholesWorld.PortalCount++;
		//}

		
		public void FinishSettingUpWormholes() {
			if( this.WormholesFinishedSpawning ) { return; }

			if( WormholeManager.ForceRegenWormholes ) {
				ErrorLogger.Log( "  Regenerating ALL portals." );
				this.Links = new List<WormholeLink>( WormholeManager.PortalCount );
			}

			for( int i = 0; i < WormholeManager.PortalCount; i++ ) {
				// Skip already-loaded wormholes
				if( i < this.Links.Count && this.Links[i] != null ) { continue; }

				var link = this.CreateRandomWormholePair( WormholeLink.GetColor(i) );
				this.Links.Add( link );
			}
			
			this.WormholesFinishedSpawning = true;
		}


		/////////////////

		public void RunAll( Player player ) {
			var mymod = WormholesMod.Instance;
			int who = player.whoAmI;

			if( !this.BlockPortalCountdown.Keys.Contains( who ) ) {
				this.BlockPortalCountdown[who] = 0;
			}

			bool isUponAPortal = false;
			bool isUponMyPortal = false;
			int blockCountdown = this.BlockPortalCountdown[who];
			WormholeLink townPortal = player.GetModPlayer<WormholesPlayer>( mymod ).MyPortal;

			if( !mymod.Config.DisableNaturalWormholes ) {
				for( int i = 0; i < this.Links.Count; i++ ) {
					WormholeLink link = this.Links[i];
					if( link == null ) { break; }

					link.UpdateInteractions( player, (blockCountdown > 0), out isUponAPortal );
					link.UpdateBehavior( player );

					if( isUponAPortal ) { break; }
				}
			}

			if( townPortal != null ) {
				townPortal.UpdateInteractions( player, (blockCountdown > 0 || isUponAPortal), out isUponMyPortal );
				townPortal.UpdateBehavior( player );
			}

			if( (isUponAPortal || isUponMyPortal) && blockCountdown == 0 ) {
				this.BlockPortalCountdown[who] = 120;
			}
			if( (!isUponAPortal && !isUponMyPortal && blockCountdown > 0) || blockCountdown > 1 ) {
				this.BlockPortalCountdown[who]--;
			}
		}

		public void DrawAll( WormholeLink townPortal ) {
			var mymod = WormholesMod.Instance;

			if( !mymod.Config.DisableNaturalWormholes ) {
				for( int i = 0; i < this.Links.Count; i++ ) {
					WormholeLink link = this.Links[i];
					if( link == null ) { break; }

					link.DrawForMe();
				}
			}
			if( townPortal != null ) {
				townPortal.DrawForMe();
			}
		}
	}
}

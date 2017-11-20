using HamstarHelpers.DebugHelpers;
using HamstarHelpers.TileHelpers;
using HamstarHelpers.WorldHelpers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace Wormholes {
	class WormholeManager {
		public static bool ForceRegenWormholes = false;
		public static int PortalCount { get; private set; }


		////////////////

		private IDictionary<int, int> BlockPortalCountdown = new Dictionary<int, int>();
		public IList<WormholeLink> Links { get; private set; }
		public bool WormholesFinishedSpawning { get; private set; }

		
		////////////////

		public WormholeManager( WormholesMod mymod ) {
			switch( WorldHelpers.GetSize() ) {
			case WorldSize.SubSmall:
				WormholeManager.PortalCount = mymod.Config.Data.TinyWorldPortals;
				break;
			case WorldSize.Small:
				WormholeManager.PortalCount = mymod.Config.Data.SmallWorldPortals;
				break;
			case WorldSize.Medium:
				WormholeManager.PortalCount = mymod.Config.Data.MediumWorldPortals;
				break;
			case WorldSize.Large:
				WormholeManager.PortalCount = mymod.Config.Data.LargeWorldPortals;
				break;
			case WorldSize.SuperLarge:
				WormholeManager.PortalCount = mymod.Config.Data.HugeWorldPortals;
				break;
			}

			this.Links = new List<WormholeLink>( WormholeManager.PortalCount );
		}

		/////////////////

		public bool Load( WormholesMod mymod, TagCompound tags ) {
			if( mymod.Config.Data.DisableNaturalWormholes ) { return false; }
			if( !tags.ContainsKey("wormhole_count") ) { return false; }

			int holes = tags.GetInt( "wormhole_count" );
			if( holes == 0 ) { return false; }

			if( mymod.IsDebugInfoMode() ) {
				DebugHelpers.Log( "Loading world ids (" + Main.netMode + "): " + holes );
			}

			int[] worm_l_x = tags.GetIntArray( "wormhole_left_x" );
			int[] worm_l_y = tags.GetIntArray( "wormhole_left_y" );
			int[] worm_r_x = tags.GetIntArray( "wormhole_right_x" );
			int[] worm_r_y = tags.GetIntArray( "wormhole_right_y" );

			for( int i = 0; i < holes && i < worm_l_x.Length && i < WormholeManager.PortalCount; i++ ) {
				if( i < this.Links.Count && this.Links[i] != null ) {
					this.Links[i].Close();
				}

				string id = tags.GetString( "wormhole_id_" + i );
				if( mymod.IsDebugInfoMode() ) {
					DebugHelpers.Log( "  world load id: " + id + " (" + i + ")" );
				}

				Vector2 pos_l = new Vector2( worm_l_x[i], worm_l_y[i] );
				Vector2 pos_r = new Vector2( worm_r_x[i], worm_r_y[i] );

				var link = new WormholeLink( id, WormholeLink.GetColor( i ), pos_l, pos_r );

				// Failsafe against glitched portals
				if( link.IsMisplaced ) {
					ErrorLogger.Log( "Found bad portal. " + i + " " + worm_l_x[i] + "," + worm_l_y[i]
						+ " : " + worm_r_x[i] + "," + worm_r_y[i] );
					WormholeManager.ForceRegenWormholes = true;
					break;
				}

				this.Links.Insert( i, link );
			}
			return true;
		}


		public TagCompound Save() {
			string[] ids = new string[WormholeManager.PortalCount];
			int[] worm_l_x = new int[WormholeManager.PortalCount];
			int[] worm_l_y = new int[WormholeManager.PortalCount];
			int[] worm_r_x = new int[WormholeManager.PortalCount];
			int[] worm_r_y = new int[WormholeManager.PortalCount];

			if( WormholesMod.Instance.IsDebugInfoMode() ) {
				DebugHelpers.Log( "Save world ids (" + Main.netMode + "): " + WormholeManager.PortalCount );
			}

			int i;
			for( i = 0; i < this.Links.Count; i++ ) {
				var link = this.Links[i];
				if( link == null ) { break; }
				if( link.IsClosed ) { continue; }

				ids[i] = link.ID;
				worm_l_x[i] = (int)link.LeftPortal.Pos.X;
				worm_l_y[i] = (int)link.LeftPortal.Pos.Y;
				worm_r_x[i] = (int)link.RightPortal.Pos.X;
				worm_r_y[i] = (int)link.RightPortal.Pos.Y;
			}
			
			var tags = new TagCompound {
				{"wormhole_count", i},
				{"wormhole_left_x", worm_l_x},
				{"wormhole_left_y", worm_l_y},
				{"wormhole_right_x", worm_r_x},
				{"wormhole_right_y", worm_r_y}
			};

			for( i = 0; i < this.Links.Count; i++ ) {
				tags.Set( "wormhole_id_" + i, ids[i] );
				if( WormholesMod.Instance.IsDebugInfoMode() ) {
					DebugHelpers.Log( "  world save id: " + ids[i] + " (" + i + ") = "
						+ worm_l_x[i] + "," + worm_l_y[i] + " | " + worm_r_x[i] + "," + worm_r_y[i] );
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
			Vector2 rand_pos1, rand_pos2;

			do {
				rand_pos1 = this.GetRandomClearMapPos();
				rand_pos2 = this.GetRandomClearMapPos();
			} while( Vector2.Distance( rand_pos1, rand_pos2 ) < 2048 );

			link.ChangePosition( rand_pos1, rand_pos2 );
		}

		/////////////////

		private Vector2 GetRandomClearMapPos() {
			Vector2 rand_pos;
			int world_x, world_y;
			bool found = false, is_empty = false;

			do {
				found = true;

				do {
					world_x = Main.rand.Next( 64, Main.maxTilesX - 64 );
					world_y = Main.rand.Next( (int)Main.worldSurface, Main.maxTilesY - 220 );

					is_empty = true;
					for( int i=world_x; i<world_x+6; i++ ) {
						for( int j=world_y; j<world_y+8; j++ ) {
							Tile tile = Framing.GetTileSafely( i, j );

							is_empty = !TileHelpers.IsSolid( tile, true, true ) && !tile.lava() && !TileWallHelpers.IsDungeon(tile);
							if( !is_empty ) { break; }
						}
						if( !is_empty ) { break; }
					}
				} while( !is_empty );
				//} while( Collision.SolidCollision( new Vector2(world_x*16f, world_y*16f), WormholePortal.Width, WormholePortal.Height ) );

				rand_pos = new Vector2( world_x * 16f, world_y * 16f );

				// Not too close to other portals?
				for( int i = 0; i < this.Links.Count; i++ ) {
					var link = this.Links[i];

					float dist = Vector2.Distance( link.LeftPortal.Pos, rand_pos );
					if( dist < 2048 ) {
						found = false;
						break;
					}

					dist = Vector2.Distance( link.RightPortal.Pos, rand_pos );
					if( dist < 2048 ) {
						found = false;
						break;
					}
				}
			} while( !found );

			return rand_pos;
		}


		public WormholeLink CreateRandomWormholePair( Color color ) {
			Vector2 rand_pos1, rand_pos2;

			do {
				rand_pos1 = this.GetRandomClearMapPos();
				rand_pos2 = this.GetRandomClearMapPos();
			} while( Vector2.Distance( rand_pos1, rand_pos2 ) < 2048 );

			return new WormholeLink( color, rand_pos1, rand_pos2 );
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

		public void RunAll( WormholeModContext ctx, Player player ) {
			int who = player.whoAmI;
			if( !this.BlockPortalCountdown.Keys.Contains( who ) ) {
				this.BlockPortalCountdown[who] = 0;
			}

			bool is_upon_a_portal = false;
			bool is_upon_my_portal = false;
			int block_countdown = this.BlockPortalCountdown[who];
			WormholeLink town_portal = player.GetModPlayer<WormholesPlayer>( ctx.MyMod ).MyPortal;

			if( !ctx.MyMod.Config.Data.DisableNaturalWormholes ) {
				for( int i = 0; i < this.Links.Count; i++ ) {
					WormholeLink link = this.Links[i];
					if( link == null ) { break; }

					link.UpdateInteractions( ctx, player, (block_countdown > 0), out is_upon_a_portal );
					link.UpdateBehavior( ctx, player );

					if( is_upon_a_portal ) { break; }
				}
			}

			if( town_portal != null ) {
				town_portal.UpdateInteractions( ctx, player, (block_countdown > 0 || is_upon_a_portal), out is_upon_my_portal );
				town_portal.UpdateBehavior( ctx, player );
			}

			if( (is_upon_a_portal || is_upon_my_portal) && block_countdown == 0 ) {
				this.BlockPortalCountdown[who] = 120;
			}
			if( (!is_upon_a_portal && !is_upon_my_portal && block_countdown > 0) || block_countdown > 1 ) {
				this.BlockPortalCountdown[who]--;
			}
		}

		public void DrawAll( WormholeModContext ctx, WormholeLink town_portal ) {
			if( !ctx.MyMod.Config.Data.DisableNaturalWormholes ) {
				for( int i = 0; i < this.Links.Count; i++ ) {
					WormholeLink link = this.Links[i];
					if( link == null ) { break; }

					link.DrawForMe( ctx );
				}
			}
			if( town_portal != null ) {
				town_portal.DrawForMe( ctx );
			}
		}
	}
}

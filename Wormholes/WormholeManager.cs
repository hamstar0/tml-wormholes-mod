using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Utils;


namespace Wormholes {
	public class WormholeManager {
		public static bool ForceRegenWormholes = false;
		public static int PortalCount { get; private set; }

		private IDictionary<int, int> BlockPortalCountdown = new Dictionary<int, int>();
		public IList<WormholeLink> Links { get; private set; }
		public bool WormholesSpawned { get; private set; }

		
		////////////////

		public WormholeManager( WormholesMod mymod ) {
			int size = Main.maxTilesX * Main.maxTilesY;

			if( size <= (4200 * 1200) / 2 ) {
				WormholeManager.PortalCount = mymod.Config.Data.TinyWorldPortals;
			} else if( size <= 4200 * 1200 + 1000 ) {
				WormholeManager.PortalCount = mymod.Config.Data.SmallWorldPortals;
			} else if( size <= 6400 * 1800 + 1000 ) {
				WormholeManager.PortalCount = mymod.Config.Data.MediumWorldPortals;
			} else if( size <= 8400 * 2400 + 1000 ) {
				WormholeManager.PortalCount = mymod.Config.Data.LargeWorldPortals;
			} else {
				WormholeManager.PortalCount = mymod.Config.Data.HugeWorldPortals;
			}

			this.Links = new List<WormholeLink>( WormholeManager.PortalCount );
		}

		/////////////////

		public void Load( WormholesMod mymod, TagCompound tags ) {
			if( mymod.Config.Data.DisableNaturalWormholes ) { return; }

			int holes = tags.GetInt( "wormhole_count" );
			if( holes == 0 ) { return; }

			if( Debug.DEBUGMODE ) {
				ErrorLogger.Log( "Loading world ids (" + Main.netMode + "): " + holes );
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
				if( Debug.DEBUGMODE ) {
					ErrorLogger.Log( "  world load id: " + id + " (" + i + ")" );
				}

				Vector2 pos_l = new Vector2( worm_l_x[i], worm_l_y[i] );
				Vector2 pos_r = new Vector2( worm_r_x[i], worm_r_y[i] );

				var link = new WormholeLink( id, mymod, WormholeLink.GetColor( i ), pos_l, pos_r );

				// Failsafe against glitched portals
				if( link.IsMisplaced ) {
					ErrorLogger.Log( "Found bad portal. " + i + " " + worm_l_x[i] + "," + worm_l_y[i]
						+ " : " + worm_r_x[i] + "," + worm_r_y[i] );
					WormholeManager.ForceRegenWormholes = true;
					break;
				}

				this.Links.Insert( i, link );
			}
		}


		public TagCompound Save() {
			string[] ids = new string[WormholeManager.PortalCount];
			int[] worm_l_x = new int[WormholeManager.PortalCount];
			int[] worm_l_y = new int[WormholeManager.PortalCount];
			int[] worm_r_x = new int[WormholeManager.PortalCount];
			int[] worm_r_y = new int[WormholeManager.PortalCount];

			if( Debug.DEBUGMODE ) {
				ErrorLogger.Log( "Save world ids (" + Main.netMode + "): " + WormholeManager.PortalCount );
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
				if( Debug.DEBUGMODE ) {
					ErrorLogger.Log( "  world save id: " + ids[i] + " (" + i + ") = "
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
							is_empty = TileHelper.IsNonBlocking( Main.tile[i, j] );
							if( !is_empty ) { break; }
						}
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


		private WormholeLink CreateRandomWormholePair( WormholesMod mymod, Color color ) {
			Vector2 rand_pos1, rand_pos2;

			do {
				rand_pos1 = this.GetRandomClearMapPos();
				rand_pos2 = this.GetRandomClearMapPos();
			} while( Vector2.Distance( rand_pos1, rand_pos2 ) < 2048 );

			return new WormholeLink( mymod, color, rand_pos1, rand_pos2 );
		}


		//public void AddWormholePair( WormholeLink link ) {
		//	this.Links.Add( link );
		//	WormholesWorld.PortalCount++;
		//}

		
		public void SetupWormholes( WormholesMod mymod ) {
			if( this.WormholesSpawned ) { return; }

			if( WormholeManager.ForceRegenWormholes ) {
				ErrorLogger.Log( "  Regenerating ALL portals." );
				this.Links = new List<WormholeLink>( WormholeManager.PortalCount );
			}

			for( int i = 0; i < WormholeManager.PortalCount; i++ ) {
				if( i < this.Links.Count && this.Links[i] != null ) {
					continue;
				}

				var link = this.CreateRandomWormholePair( mymod, WormholeLink.GetColor( i ) );
				if( i >= this.Links.Count ) {
					this.Links.Add( link );
				} else {
					this.Links.Insert( i, link );
				}
			}

			this.WormholesSpawned = true;
		}


		/////////////////

		public void RunAll( WormholesMod mymod, Player player ) {
			int who = player.whoAmI;
			if( !this.BlockPortalCountdown.Keys.Contains( who ) ) {
				this.BlockPortalCountdown[who] = 0;
			}

			bool is_upon_a_portal = false;
			bool is_upon_my_portal = false;
			int block_countdown = this.BlockPortalCountdown[who];
			WormholeLink town_portal = player.GetModPlayer<WormholesPlayer>( mymod ).MyPortal;

			if( !mymod.Config.Data.DisableNaturalWormholes ) {
				for( int i = 0; i < this.Links.Count; i++ ) {
					WormholeLink link = this.Links[i];
					if( link == null ) { break; }

					link.UpdateInteractions( player, (block_countdown > 0), out is_upon_a_portal );
					link.UpdateBehavior( player );

					if( is_upon_a_portal ) { break; }
				}
			}

			if( town_portal != null ) {
				town_portal.UpdateInteractions( player, (block_countdown > 0 || is_upon_a_portal), out is_upon_my_portal );
				town_portal.UpdateBehavior( player );
			}

			if( (is_upon_a_portal || is_upon_my_portal) && block_countdown == 0 ) {
				this.BlockPortalCountdown[who] = 120;
			}
			if( (!is_upon_a_portal && !is_upon_my_portal && block_countdown > 0) || block_countdown > 1 ) {
				this.BlockPortalCountdown[who]--;
			}
		}

		public void DrawAll( WormholesMod mymod, WormholeLink town_portal ) {
			if( !mymod.Config.Data.DisableNaturalWormholes ) {
				for( int i = 0; i < this.Links.Count; i++ ) {
					WormholeLink link = this.Links[i];
					if( link == null ) { break; }

					link.DrawForMe();
				}
			}
			if( town_portal != null ) {
				town_portal.DrawForMe();
			}
		}
	}
}

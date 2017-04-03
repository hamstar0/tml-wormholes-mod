using Terraria;
using Terraria.ModLoader;

namespace Utils {
	public class TileHelper {
		public static bool IsNonBlocking( Tile tile, bool lava_blocks=true, bool walls_block=false, bool special_walls_block=true,
										  bool actuations_block=false, bool platforms_block=false, bool wire_blocks=true ) {
			if( tile == null || !tile.active() ) { return true; }

			if( wire_blocks ) {
				if( tile.wire() || tile.wire2() || tile.wire3() || tile.wire4() ) {
					return false;
				}
			}

			// Special case: Lava
			if( lava_blocks && tile.lava() ) { return false; }

			if( tile.wall > 0 ) {
				if( walls_block ) { return false; }
				if( special_walls_block ) {
					// Special case: Lihzahrd Brick Wall
					if( tile.wall == 87 ) { return false; }
					// Special case: Dungeon Walls
					if( (tile.wall >= 7 && tile.wall <= 9) || (tile.wall >= 94 && tile.wall <= 99) ) { return false; }
				}
			}

			if( Main.tileSolid[(int)tile.type] ) {
				// Actuated?
				if( !actuations_block && tile.inActive() ) { return true; }
				// Is a platform?
				if( !platforms_block && Main.tileSolidTop[(int)tile.type] ) { return true; }
				
				return false;
			}

			// Is a non-solid platform?
			if( platforms_block && Main.tileSolidTop[(int)tile.type] ) { return false; }

			return true;
		}


		public static bool FindNearbyRandomAirTile( int tile_x, int tile_y, int radius, out int to_x, out int to_y ) {
			Tile tile = null;
			int wtf = 0;

			do {
				do { to_x = Main.rand.Next( -radius, radius ) + tile_x; }
				while( to_x < 0 || to_x >= Main.mapMaxX );
				do { to_y = Main.rand.Next( -radius, radius ) + tile_y; }
				while( to_y < 0 || to_y >= Main.mapMaxY );

				tile = Main.tile[ to_x, to_y ];
				if( wtf++ > 100 ) {
					return false;
				}
			} while( !TileHelper.IsNonBlocking( tile, false, false, true, true, true, true ) &&
					 (tile.type != 0 || Lighting.Brightness(to_x, to_x) == 0) );

			return true;
		}


		public static bool IsNotBombable( int i, int j ) {
			Tile tile = Main.tile[i, j];

			return !TileLoader.CanExplode(i, j) ||
				Main.tileDungeon[(int)tile.type] ||
				tile.type == 88 ||	// Dresser
				tile.type == 21 ||	// Chest
				tile.type == 26 ||	// Demon Altar
				tile.type == 107 ||	// Cobalt Ore
				tile.type == 108 ||	// Mythril Ore
				tile.type == 111 ||	// Adamantite Ore
				tile.type == 226 ||	// Lihzahrd Brick
				tile.type == 237 ||	// Lihzahrd Altar
				tile.type == 221 ||	// Palladium Ore
				tile.type == 222 ||	// Orichalcum Ore
				tile.type == 223 ||	// Titanium Ore
				tile.type == 211 || // Chlorophyte Ore
				tile.type == 404 ||	// Desert Fossil
				(!Main.hardMode && tile.type == 58);	// Hellstone Ore
		}
	}
}

using Microsoft.Xna.Framework;
using Terraria;


namespace Utils {
	public class PlayerHelper {
		public static void Teleport( Player player, Vector2 pos, int style=-1 ) {
			player.grappling[0] = -1;
			player.grapCount = 0;

			bool is_immune = player.immune;
			int immune_time = player.immuneTime;
			player.Spawn();
			player.immune = is_immune;
			player.immuneTime = immune_time;

			if( Main.netMode <= 1 ) {
				player.Teleport( pos, style );
			} else {
				NetMessage.SendData( 65, -1, -1, "", 0, (float)player.whoAmI, pos.X, pos.Y, style, 0, 0 );
			}
		}


		public static Vector2 GetSpawnPoint( Player player ) {
			var pos = new Vector2();

			if( player.SpawnX >= 0 && player.SpawnY >= 0 ) {
				pos.X = (float)(player.SpawnX * 16 + 8 - player.width / 2);
				pos.Y = (float)(player.SpawnY * 16 - player.height);
			} else {
				pos.X = (float)(Main.spawnTileX * 16 + 8 - player.width / 2);
				pos.Y = (float)(Main.spawnTileY * 16 - player.height);
			}

			return pos;
		}
	}
}

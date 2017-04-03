using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;


namespace Utils {
	public static class Debug {
		public static bool DEBUGMODE = false;
		public static bool Once;
		public static int OnceInAWhile;


		public static void MsgOnce( string msg ) {
			if( Debug.Once ) { return; }
			Debug.Once = true;

			Main.NewText( msg );
		}

		public static void MsgOnceInAWhile( string msg ) {
			if( Debug.OnceInAWhile > 0 ) { return; }
			Debug.OnceInAWhile = 60 * 10;

			Main.NewText( msg );
		}


		public static Dictionary<string, string> Display = new Dictionary<string, string>();

		public static void PrintToBatch( SpriteBatch sb ) {
			int i = 0;

			foreach( string key in Debug.Display.Keys.ToList() ) {
				string msg = key + ":  " + Debug.Display[key];
				sb.DrawString( Main.fontMouseText, msg, new Vector2( 8, (Main.screenHeight - 32) - (i * 24) ), Color.White );

				//Debug.Display[key] = "";
				i++;
			}
		}
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;


namespace Utils {
	public static class DebugHelper {
		public static int DEBUGMODE = 0;	// 1: Log output, 2: Map reveal
		public static bool Once;
		public static int OnceInAWhile;


		public static void MsgOnce( string msg ) {
			if( DebugHelper.Once ) { return; }
			DebugHelper.Once = true;

			Main.NewText( msg );
		}

		public static void MsgOnceInAWhile( string msg ) {
			if( DebugHelper.OnceInAWhile > 0 ) { return; }
			DebugHelper.OnceInAWhile = 60 * 10;

			Main.NewText( msg );
		}


		public static Dictionary<string, string> Display = new Dictionary<string, string>();

		public static void PrintToBatch( SpriteBatch sb ) {
			int i = 0;

			foreach( string key in DebugHelper.Display.Keys.ToList() ) {
				string msg = key + ":  " + DebugHelper.Display[key];
				sb.DrawString( Main.fontMouseText, msg, new Vector2( 8, (Main.screenHeight - 32) - (i * 24) ), Color.White );

				//Debug.Display[key] = "";
				i++;
			}
		}
	}
}

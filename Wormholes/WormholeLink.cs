using HamstarHelpers.PlayerHelpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Wormholes.NetProtocol;

namespace Wormholes {
	public class WormholeLink {
		static WormholeLink() {
			WormholeLink.PortalColors = new Color[] {
				new Color( 0xFF, 0x00, 0x00 ),
				new Color( 0x00, 0xFF, 0x00 ),
				new Color( 0xFF, 0xFF, 0x00 ),
				new Color( 0x00, 0x00, 0xFF ),
				new Color( 0xFF, 0x00, 0xFF ),
				//new Color( 0x00, 0xFF, 0xFF ),	// Reserved for Town Portals

				new Color( 0xFF, 0x3F, 0x00 ),
				new Color( 0xFF, 0x00, 0x3F ),
				new Color( 0x3F, 0xFF, 0x00 ),
				new Color( 0x00, 0xFF, 0x3F ),
				new Color( 0xFF, 0xFF, 0x3F ),
				new Color( 0x3F, 0x00, 0xFF ),
				new Color( 0x00, 0x3F, 0xFF ),
				new Color( 0xFF, 0x3F, 0xFF ),
				//new Color( 0x3F, 0xFF, 0xFF ),	// Too similar to Town Portals

				new Color( 0xFF, 0x7F, 0x00 ),
				new Color( 0xFF, 0x00, 0x7F ),
				new Color( 0x7F, 0xFF, 0x00 ),
				new Color( 0x00, 0xFF, 0x7F ),
				new Color( 0x7F, 0x00, 0xFF ),
				new Color( 0x00, 0x7F, 0xFF ),

				new Color( 0xFF, 0x7F, 0x3F ),
				new Color( 0xFF, 0x3F, 0x7F ),
				new Color( 0x7F, 0xFF, 0x3F ),
				new Color( 0x3F, 0xFF, 0x7F ),
				new Color( 0xFF, 0xFF, 0x7F ),
				new Color( 0x7F, 0x3F, 0xFF ),
				new Color( 0x3F, 0x7F, 0xFF ),
				new Color( 0xFF, 0x7F, 0xFF ),
				//new Color( 0x7F, 0xFF, 0xFF ),	// Too similar to Town Portals

				new Color( 0xFF, 0xBF, 0x00 ),
				new Color( 0xFF, 0x00, 0xBF ),
				new Color( 0xBF, 0xFF, 0x00 ),
				new Color( 0x00, 0xFF, 0xBF ),
				new Color( 0xBF, 0x00, 0xFF ),
				new Color( 0x00, 0xBF, 0xFF ),

				new Color( 0xFF, 0xBF, 0x3F ),
				new Color( 0xFF, 0x3F, 0xBF ),
				new Color( 0xBF, 0xFF, 0x3F ),
				new Color( 0x3F, 0xFF, 0xBF ),
				new Color( 0xBF, 0x3F, 0xFF ),
				new Color( 0x3F, 0xBF, 0xFF ),

				new Color( 0xFF, 0xBF, 0x7F ),
				new Color( 0xFF, 0x7F, 0xBF ),
				new Color( 0xBF, 0xFF, 0x7F ),
				new Color( 0x7F, 0xFF, 0xBF ),
				//new Color( 0xFF, 0xFF, 0xBF ),	// Too bright
				new Color( 0xBF, 0x7F, 0xFF ),
				new Color( 0x7F, 0xBF, 0xFF ),
				//new Color( 0xFF, 0xBF, 0xFF ),	// Too bright
				//new Color( 0xBF, 0xFF, 0xFF ),	// Too bright
				new Color( 0xBF, 0xBF, 0xBF )

				/*Color.AliceBlue,			// R:240 G:248 B:255
				Color.AntiqueWhite,			// R:250 G:235 B:215
				Color.Aqua,					// R:0 G:255 B:255
				Color.Aquamarine,			// R:127 G:255 B:212
				Color.Azure,				// R:240 G:255 B:255
				Color.Beige,				// R:245 G:245 B:220
				Color.Bisque,				// R:255 G:228 B:196
				Color.BlanchedAlmond,		// R:255 G:235 B:205
				Color.Blue,					// R:0 G:0 B:255
				Color.BlueViolet,			// R:138 G:43 B:226
				//Color.Brown,				// R:165 G:42 B:42
				Color.BurlyWood,			// R:222 G:184 B:135
				//Color.CadetBlue,			// R:95 G:158 B:160
				Color.Chartreuse,			// R:127 G:255 B:0
				Color.Chocolate,			// R:210 G:105 B:30
				Color.Coral,				// R:255 G:127 B:80
				Color.CornflowerBlue,		// R:100 G:149 B:237
				Color.Cornsilk,				// R:255 G:248 B:220
				Color.Crimson,				// R:220 G:20 B:60
				////Color.Cyan,					// R:0 G:255 B:255
				//Color.DarkBlue,				// R:0 G:0 B:139
				//Color.DarkCyan,				// R:0 G:139 B:139
				//Color.DarkGoldenrod,		// R:184 G:134 B:11
				//Color.DarkGray,				// R:169 G:169 B:169
				//Color.DarkGreen,			// R:0 G:100 B:0
				//Color.DarkKhaki,			// R:189 G:183 B:107
				//Color.DarkMagenta,			// R:139 G:0 B:139
				//Color.DarkOliveGreen,		// R:85 G:107 B:47
				Color.DarkOrange,			// R:255 G:140 B:0
				Color.DarkOrchid,			// R:153 G:50 B:204
				//Color.DarkRed,				// R:139 G:0 B:0
				Color.DarkSalmon,			// R:233 G:150 B:122
				//Color.DarkSeaGreen,			// R:143 G:188 B:139
				//Color.DarkSlateBlue,		// R:72 G:61 B:139
				//Color.DarkSlateGray,		// R:47 G:79 B:79
				Color.DarkTurquoise,		// R:0 G:206 B:209
				Color.DarkViolet,			// R:148 G:0 B:211
				Color.DeepPink,				// R:255 G:20 B:147
				Color.DeepSkyBlue,			// R:0 G:191 B:255
				//Color.DimGray,				// R:105 G:105 B:105
				Color.DodgerBlue,			// R:30 G:144 B:255
				//Color.Firebrick,			// R:178 G:34 B:34
				Color.FloralWhite,			// R:255 G:250 B:240
				//Color.ForestGreen,			// R:34 G:139 B:34
				Color.Fuchsia,				// R:255 G:0 B:255
				Color.Gainsboro,			// R:220 G:220 B:220
				Color.GhostWhite,			// R:248 G:248 B:255
				Color.Gold,					// R:255 G:215 B:0
				Color.Goldenrod,			// R:218 G:165 B:32
				//Color.Gray,					// R:128 G:128 B:128
				//Color.Green,				// R:0 G:128 B:0
				Color.GreenYellow,			// R:173 G:255 B:47
				Color.Honeydew,				// R:240 G:255 B:240
				Color.HotPink,				// R:255 G:105 B:180
				//Color.IndianRed,			// R:205 G:92 B:92
				//Color.Indigo,				// R:75 G:0 B:130
				Color.Ivory,				// R:255 G:255 B:240
				Color.Khaki,				// R:240 G:230 B:140
				Color.Lavender,				// R:230 G:230 B:250
				Color.LavenderBlush,		// R:255 G:240 B:245
				Color.LawnGreen,			// R:124 G:252 B:0
				Color.LemonChiffon,			// R:255 G:250 B:205
				Color.LightBlue,			// R:173 G:216 B:230
				Color.LightCoral,			// R:240 G:128 B:128
				Color.LightCyan,			// R:224 G:255 B:255
				Color.LightGoldenrodYellow,	// R:250 G:250 B:210
				Color.LightGray,			// R:211 G:211 B:211
				Color.LightGreen,			// R:144 G:238 B:144
				Color.LightPink,			// R:255 G:182 B:193
				Color.LightSalmon,			// R:255 G:160 B:122
				//Color.LightSeaGreen,		// R:32 G:178 B:170
				Color.LightSkyBlue,			// R:135 G:206 B:250
				//Color.LightSlateGray,		// R:119 G:136 B:153
				Color.LightSteelBlue,		// R:176 G:196 B:222
				Color.LightYellow,			// R:255 G:255 B:224
				Color.Lime,					// R:0 G:255 B:0
				//Color.LimeGreen,			// R:50 G:205 B:50
				Color.Linen,				// R:250 G:240 B:230
				Color.Magenta,				// R:255 G:0 B:255
				//Color.Maroon,				// R:128 G:0 B:0
				Color.MediumAquamarine,		// R:102 G:205 B:170
				//Color.MediumBlue,			// R:0 G:0 B:205
				Color.MediumOrchid,			// R:186 G:85 B:211
				Color.MediumPurple,			// R:147 G:112 B:219
				//Color.MediumSeaGreen,		// R:60 G:179 B:113
				Color.MediumSlateBlue,		// R:123 G:104 B:238
				Color.MediumSpringGreen,	// R:0 G:250 B:154
				Color.MediumTurquoise,		// R:72 G:209 B:204
				//Color.MediumVioletRed,		// R:199 G:21 B:133
				//Color.MidnightBlue,			// R:25 G:25 B:112
				Color.MintCream,			// R:245 G:255 B:250
				Color.MistyRose,			// R:255 G:228 B:225
				Color.Moccasin,				// R:255 G:228 B:181
				Color.NavajoWhite,			// R:255 G:222 B:173
				//Color.Navy,					// R:0 G:0 B:128
				Color.OldLace,				// R:253 G:245 B:230
				//Color.Olive,				// R:128 G:128 B:0
				//Color.OliveDrab,			// R:107 G:142 B:35
				Color.Orange,				// R:255 G:165 B:0
				Color.OrangeRed,			// R:255 G:69 B:0
				Color.Orchid,				// R:218 G:112 B:214
				Color.PaleGoldenrod,		// R:238 G:232 B:170
				Color.PaleGreen,			// R:152 G:251 B:152
				Color.PaleTurquoise,		// R:175 G:238 B:238
				Color.PaleVioletRed,		// R:219 G:112 B:147
				Color.PapayaWhip,			// R:255 G:239 B:213
				Color.PeachPuff,			// R:255 G:218 B:185
				Color.Peru,					// R:205 G:133 B:63
				Color.Pink,					// R:255 G:192 B:203
				Color.Plum,					// R:221 G:160 B:221
				Color.PowderBlue,			// R:176 G:224 B:230
				//Color.Purple,				// R:128 G:0 B:128
				Color.Red,					// R:255 G:0 B:0
				Color.RosyBrown,			// R:188 G:143 B:143
				Color.RoyalBlue,			// R:65 G:105 B:225
				//Color.SaddleBrown,			// R:139 G:69 B:19
				Color.Salmon,				// R:250 G:128 B:114
				Color.SandyBrown,			// R:244 G:164 B:96
				//Color.SeaGreen,				// R:46 G:139 B:87
				Color.SeaShell,				// R:255 G:245 B:238
				//Color.Sienna,				// R:160 G:82 B:45
				//Color.Silver,				// R:192 G:192 B:192
				Color.SkyBlue,				// R:135 G:206 B:235
				Color.SlateBlue,			// R:106 G:90 B:205
				//Color.SlateGray,			// R:112 G:128 B:144
				Color.Snow,					// R:255 G:250 B:250
				Color.SpringGreen,			// R:0 G:255 B:127
				//Color.SteelBlue,			// R:70 G:130 B:180
				Color.Tan,					// R:210 G:180 B:140
				//Color.Teal,					// R:0 G:128 B:128
				Color.Thistle,				// R:216 G:191 B:216
				Color.Tomato,				// R:255 G:99 B:71
				Color.Turquoise,			// R:64 G:224 B:208
				Color.Violet,				// R:238 G:130 B:238
				Color.Wheat,				// R:245 G:222 B:179
				Color.White,				// R:255 G:255 B:255
				Color.WhiteSmoke,			// R:245 G:245 B:245
				Color.Yellow,				// R:255 G:255 B:0
				Color.YellowGreen			// R:154 G:205 B:50*/
			};
		}

		////////////////

		public static Color[] PortalColors { get; private set; }
		//private static bool IsShuffled = false;



		////////////////

		public static Color GetColor( int i ) {
			/*if( !WormholeLink.IsShuffled ) {
				Random rnd = new Random( Main.worldID );
				WormholeLink.PortalColors = WormholeLink.PortalColors.OrderBy( x => rnd.Next() ).ToArray();
				WormholeLink.IsShuffled = true;
			}*/
			Color color = WormholeLink.PortalColors[i % WormholeLink.PortalColors.Length];

			return color;
		}

		////////////////


		
		public string ID { get; private set; }
		private WormholesMod MyMod;
		private Color NodeColor;
		public WormholePortal LeftPortal { get; private set; }
		public WormholePortal RightPortal { get; private set; }

		public bool IsClosed { get; private set; }

		public bool IsMisplaced { get; private set; }


		////////////////

		public WormholeLink( WormholesMod mymod, Color color, Vector2 left_node_pos, Vector2 right_node_pos ) {
			this.ID = Guid.NewGuid().ToString("D");
			this.MyMod = mymod;
			this.NodeColor = color;
			this.LeftPortal = new WormholePortal( mymod, left_node_pos, color );
			this.RightPortal = new WormholePortal( mymod, right_node_pos, color );

			this.IsMisplaced = this.LeftPortal.IsMisplaced || this.RightPortal.IsMisplaced;
		}

		public WormholeLink( string id, WormholesMod mymod, Color color, Vector2 left_node_pos, Vector2 right_node_pos ) {
			this.ID = id;
			this.MyMod = mymod;
			this.NodeColor = color;
			this.LeftPortal = new WormholePortal( mymod, left_node_pos, color );
			this.RightPortal = new WormholePortal( mymod, right_node_pos, color );

			this.IsMisplaced = this.LeftPortal.IsMisplaced || this.RightPortal.IsMisplaced;
		}

		////////////////

		public void Close() {
			if( !this.IsClosed ) {
				this.LeftPortal.Close();
				this.RightPortal.Close();
				this.LeftPortal = null;
				this.RightPortal = null;
			}

			this.IsClosed = true;
		}

		public void ChangePosition( Vector2 right_pos, Vector2 left_pos ) {
			this.RightPortal.ChangePosition( right_pos );
			this.LeftPortal.ChangePosition( left_pos );
		}

		////////////////

		public void UpdateInteractions( Player player, bool is_obstructed, out bool is_upon_portal ) {
			is_upon_portal = false;
			if( this.IsClosed ) { return; }
			
			int which = this.DetectCollision( player.getRect() );

			if( !is_obstructed ) {
				if( which == 1 ) {
					this.TeleportToLeft( player );
				} else if( which == -1 ) {
					this.TeleportToRight( player );
				}
			}

			is_upon_portal = which != 0;
		}

		public void UpdateBehavior( Player player ) {
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
				
				this.LeftPortal.SoundFX();
				this.RightPortal.SoundFX();
			}
		}

		////////////////

		public bool IsCharted( Player player ) {
			if( this.IsClosed ) { return false; }

			WormholesPlayer modplayer = player.GetModPlayer<WormholesPlayer>( this.MyMod );

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
		
		public void ApplyChaosHit( WormholesMod mymod ) {
			if( Main.netMode == 0 ) {	// Single
				var mngr = mymod.GetModWorld<WormholesWorld>().Wormholes;
				mngr.Reroll( this );
			} else {	// Non-single
				ClientPacketHandlers.SendWormholeRerollRequestViaClient( this.MyMod, this.ID );
			}
		}

		////////////////

		public void DrawForMe() {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			this.LeftPortal.DrawForMe();
			this.RightPortal.DrawForMe();

			this.LeftPortal.LightFX();
			this.RightPortal.LightFX();
		}

		////////////////

		public void TeleportToLeft( Player player ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			Vector2 dest = new Vector2(
				this.LeftPortal.Pos.X + (WormholePortal.Width / 2) - player.width,
				this.LeftPortal.Pos.Y + (WormholePortal.Height / 2) - player.height );
			
			this.Teleport( player, dest );
		}

		public void TeleportToRight( Player player ) {
			// Clients and single only
			if( Main.netMode == 2 ) { return; }
			if( this.IsClosed ) { return; }

			Vector2 dest = new Vector2(
				this.RightPortal.Pos.X + (WormholePortal.Width / 2) - player.width,
				this.RightPortal.Pos.Y + (WormholePortal.Height / 2) - player.height );

			this.Teleport( player, dest );
		}


		private void Teleport( Player player, Vector2 dest ) {
			WormholesPlayer info = player.GetModPlayer<WormholesPlayer>( this.MyMod );
			if( info.MyPortal == null || (info.MyPortal != null && this.ID != info.MyPortal.ID) ) {
				info.ChartedLinks.Add( this.ID );
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
			var snd = SoundID.Item100.WithVolume( this.MyMod.Config.Data.WormholeEntrySoundVolume );
			Main.PlaySound( snd, player.position );
		}
	}
}

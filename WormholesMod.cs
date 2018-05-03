using HamstarHelpers.Utilities.Config;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Wormholes {
	class WormholesMod : Mod {
		public static WormholesMod Instance { get; private set; }

		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-wormholes-mod"; } }

		public static string ConfigFileRelativePath {
			get { return ConfigurationDataBase.RelativePath + Path.DirectorySeparatorChar + WormholesConfigData.ConfigFileName; }
		}
		public static void ReloadConfigFromFile() {
			if( Main.netMode != 0 ) {
				throw new Exception( "Cannot reload configs outside of single player." );
			}
			WormholesMod.Instance.JsonConfig.LoadFile();
		}


		////////////////
		
		public JsonConfig<WormholesConfigData> JsonConfig { get; private set; }
		public WormholesConfigData Config { get { return this.JsonConfig.Data; } }

		private WormholesUI UI;


		////////////////

		public WormholesMod() : base() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
			
			this.JsonConfig = new JsonConfig<WormholesConfigData>( WormholesConfigData.ConfigFileName,
				ConfigurationDataBase.RelativePath, new WormholesConfigData() );
		}

		////////////////

		public override void Load() {
			WormholesMod.Instance = this;

			var hamhelpmod = ModLoader.GetMod( "HamstarHelpers" );
			var min_vers = new Version( 1, 2, 0 );
			if( hamhelpmod.Version < min_vers ) {
				throw new Exception( "Hamstar Helpers must be version " + min_vers.ToString() + " or greater." );
			}

			this.LoadConfig();

			// Clients and single only
			if( Main.netMode != 2 ) {
				WormholePortal.Initialize();
				WormholesUI.Initialize();

				this.UI = new WormholesUI();
			}
		}

		private void LoadConfig() {
			var old_config = new JsonConfig<WormholesConfigData>( "Wormholes 1.3.12.json", "", new WormholesConfigData() );
			// Update old config to new location
			if( old_config.LoadFile() ) {
				old_config.DestroyFile();
				old_config.SetFilePath( this.JsonConfig.FileName, ConfigurationDataBase.RelativePath );
				this.JsonConfig = old_config;
			}
			
			if( !this.JsonConfig.LoadFile() ) {
				this.JsonConfig.SaveFile();
			}

			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Wormholes updated to " + WormholesConfigData.ConfigVersion.ToString() );
				this.JsonConfig.SaveFile();
			}
		}

		public override void Unload() {
			WormholesMod.Instance = null;
		}
		
		////////////////

		public override void AddRecipeGroups() {
			RecipeGroup evac_grp = new RecipeGroup( () => Lang.misc[37] + " Evac Potion", new int[] {
				ItemID.RecallPotion, ItemID.WormholePotion
			} );
			RecipeGroup book_grp = new RecipeGroup( () => Lang.misc[37] + " Basic Book", new int[] {
				ItemID.Book, ItemID.SpellTome
			} );

			RecipeGroup.RegisterGroup( "WormholesMod:EvacPotions", evac_grp );
			RecipeGroup.RegisterGroup( "WormholesMod:BasicBooks", book_grp );
		}


		////////////////

		public override object Call( params object[] args ) {
			if( args.Length == 0 ) { throw new Exception( "Undefined call type." ); }

			string call_type = args[0] as string;
			if( args == null ) { throw new Exception( "Invalid call type." ); }

			var new_args = new object[args.Length - 1];
			Array.Copy( args, 1, new_args, 0, args.Length - 1 );

			return WormholesAPI.Call( call_type, new_args );
		}


		////////////////

		public override void PostDrawInterface( SpriteBatch sb ) {
			// Clients and single only (redundant?)
			if( Main.netMode == 2 ) { return; }
			
			try {
				if( !Main.mapFullscreen && (Main.mapStyle == 1 || Main.mapStyle == 2) ) {
					this.DrawMiniMap( sb );
				}
			} catch( Exception e ) {
				ErrorLogger.Log( "PostDrawInterface: " + e.ToString() );
				throw e;
			}
		}
		
		public override void PostDrawFullscreenMap( ref string mouseText ) {
			// Clients and single only (redundant?)
			if( Main.netMode == 2 ) { return; }

			try {
				this.DrawFullMap( Main.spriteBatch );
			} catch( Exception e ) {
				ErrorLogger.Log( "PostDrawFullscreenMap: " + e.ToString() );
				throw e;
			}
		}


		////////////////

		private void DrawMiniMap( SpriteBatch sb ) {
			this.UI.Update();

			WormholesWorld modworld = this.GetModWorld<WormholesWorld>();
			WormholesPlayer curr_modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>( this );

			if( !this.Config.DisableNaturalWormholes ) {
				if( modworld.Wormholes != null ) {
					for( int i = 0; i < modworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = modworld.Wormholes.Links[i];
						if( link == null ) { break; }

						if( Main.mapStyle == 1 ) {
							this.UI.DrawMiniMap( link, sb );
						} else {
							this.UI.DrawOverlayMap( link, sb );
						}
					}
				}
			}
			
			if( curr_modplayer.MyPortal != null ) {
				if( Main.mapStyle == 1 ) {
					this.UI.DrawMiniMap( curr_modplayer.MyPortal, sb );
				} else {
					this.UI.DrawOverlayMap( curr_modplayer.MyPortal, sb );
				}
			}
		}


		private void DrawFullMap( SpriteBatch sb ) {
			this.UI.Update();

			WormholesWorld modworld = this.GetModWorld<WormholesWorld>();
			WormholesPlayer curr_modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>( this );

			if( !this.Config.DisableNaturalWormholes ) {
				if( modworld.Wormholes != null ) {
					for( int i = 0; i < modworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = modworld.Wormholes.Links[i];
						if( link == null ) { break; }

						this.UI.DrawFullscreenMap( link, sb );
					}
				}
			}

			if( curr_modplayer.MyPortal != null ) {
				this.UI.DrawFullscreenMap( curr_modplayer.MyPortal, sb );
			}
		}
	}
}

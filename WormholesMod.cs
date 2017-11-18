using HamstarHelpers.Utilities.Config;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Wormholes.NetProtocol;


namespace Wormholes {
	public class WormholeModContext {
		internal WormholesMod MyMod;
		internal WormholeModContext( WormholesMod mymod ) { this.MyMod = mymod; }
	}



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
			WormholesMod.Instance.Config.LoadFile();
		}


		////////////////

		public WormholeModContext Context { get; private set; }
		public JsonConfig<WormholesConfigData> Config { get; private set; }
		private WormholesUI UI;


		////////////////

		public WormholesMod() : base() {
			this.Context = new WormholeModContext( this );

			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
			
			this.Config = new JsonConfig<WormholesConfigData>( WormholesConfigData.ConfigFileName,
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
				old_config.SetFilePath( this.Config.FileName, ConfigurationDataBase.RelativePath );
				this.Config = old_config;
			}
			
			if( !this.Config.LoadFile() ) {
				this.Config.SaveFile();
			}

			if( this.Config.Data.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Wormholes updated to " + WormholesConfigData.ConfigVersion.ToString() );
				this.Config.SaveFile();
			}
		}

		public override void Unload() {
			WormholesMod.Instance = null;
		}
		
		////////////////

		public override void AddRecipeGroups() {
			RecipeGroup group = new RecipeGroup( () => Lang.misc[37] + " Evac Potion", new int[] { 2350, 2997 } );
			RecipeGroup.RegisterGroup( "WormholesMod:EvacPotions", group );
		}


		////////////////

		public override void HandlePacket( BinaryReader reader, int player_who ) {
			if( Main.netMode == 1 ) {   // Client
				ClientPacketHandlers.HandlePacket( this, reader );
			} else if( Main.netMode == 2 ) {    // Server
				ServerPacketHandlers.HandlePacket( this, reader, player_who );
			}
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

			MyWorld modworld = this.GetModWorld<MyWorld>();
			MyPlayer curr_modplayer = Main.player[Main.myPlayer].GetModPlayer<MyPlayer>( this );

			if( !this.Config.Data.DisableNaturalWormholes ) {
				if( modworld.Wormholes != null ) {
					for( int i = 0; i < modworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = modworld.Wormholes.Links[i];
						if( link == null ) { break; }

						if( Main.mapStyle == 1 ) {
							this.UI.DrawMiniMap( this.Context, link, sb );
						} else {
							this.UI.DrawOverlayMap( this.Context, link, sb );
						}
					}
				}
			}
			
			if( curr_modplayer.MyPortal != null ) {
				if( Main.mapStyle == 1 ) {
					this.UI.DrawMiniMap( this.Context, curr_modplayer.MyPortal, sb );
				} else {
					this.UI.DrawOverlayMap( this.Context, curr_modplayer.MyPortal, sb );
				}
			}
		}


		private void DrawFullMap( SpriteBatch sb ) {
			this.UI.Update();

			MyWorld modworld = this.GetModWorld<MyWorld>();
			MyPlayer curr_modplayer = Main.player[Main.myPlayer].GetModPlayer<MyPlayer>( this );

			if( !this.Config.Data.DisableNaturalWormholes ) {
				if( modworld.Wormholes != null ) {
					for( int i = 0; i < modworld.Wormholes.Links.Count; i++ ) {
						WormholeLink link = modworld.Wormholes.Links[i];
						if( link == null ) { break; }

						this.UI.DrawFullscreenMap( this.Context, link, sb );
					}
				}
			}

			if( curr_modplayer.MyPortal != null ) {
				this.UI.DrawFullscreenMap( this.Context, curr_modplayer.MyPortal, sb );
			}
		}


		////////////////

		public bool IsDebugInfoMode() {
			return (this.Config.Data.DEBUGFLAGS & 1) != 0;
		}

		public bool IsDebugWormholeViewMode() {
			return (this.Config.Data.DEBUGFLAGS & 2) != 0;
		}

		public bool IsDebuResetMode() {
			return (this.Config.Data.DEBUGFLAGS & 4) != 0;
		}
	}
}

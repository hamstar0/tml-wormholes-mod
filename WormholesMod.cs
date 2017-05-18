using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Utils;


namespace Wormholes {
	public class ConfigurationData {
		public string VersionSinceUpdate = "";
		public int TinyWorldPortals = 4;    // SmallWorldPortals / 2
		public int SmallWorldPortals = 8;  // 4200 x 1200 = 5040000
		public int MediumWorldPortals = 14; // 6400 x 1800 = 11520000
		public int LargeWorldPortals = 20;  // 8400 x 2400 = 20160000
		public int HugeWorldPortals = 27;
		public bool CraftableTownPortalScrolls = true;
		public int TownPortalDuration = 60 * 60;    // 1 hour
		public float WormholeSoundVolume = 0.45f;
		public float WormholeLightScale = 1.25f;
		public float WormholeEntrySoundVolume = 0.9f;
		public float TeleportItemDelayMultiplier = 3f;
		public bool DisableNaturalWormholes = false;
		public int ChaosBombWormholeCloseOdds = 5;
		public int ChaosBombRadius = 4;
		public int ChaosBombScatterRadius = 32;
		public int DEBUGFLAGS = 0;
	}



	public class WormholesMod : Mod {
		public static readonly Version ConfigVersion = new Version( 1, 6, 2 );
		public JsonConfig<ConfigurationData> Config { get; private set; }

		private WormholesUI UI;


		public WormholesMod() : base() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
			
			string filename = "Wormholes Config.json";
			this.Config = new JsonConfig<ConfigurationData>( filename, "Mod Configs", new ConfigurationData() );
		}

		public override void Load() {
			var old_config = new JsonConfig<ConfigurationData>( "Wormholes 1.3.12.json", "", new ConfigurationData() );
			// Update old config to new location
			if( old_config.LoadFile() ) {
				old_config.DestroyFile();
				old_config.SetFilePath( this.Config.FileName, "Mod Configs" );
				this.Config = old_config;
			} else if( !this.Config.LoadFile() ) {
				this.Config.SaveFile();
			} else {
				Version vers_since = this.Config.Data.VersionSinceUpdate != "" ?
					new Version( this.Config.Data.VersionSinceUpdate ) :
					new Version();

				if( vers_since < WormholesMod.ConfigVersion ) {
					ErrorLogger.Log( "Wormholes config updated to " + WormholesMod.ConfigVersion.ToString() );

					if( vers_since < new Version( 1, 5, 0 ) ) {
						this.Config.Data.SmallWorldPortals = new ConfigurationData().SmallWorldPortals;
						this.Config.Data.MediumWorldPortals = new ConfigurationData().MediumWorldPortals;
						this.Config.Data.LargeWorldPortals = new ConfigurationData().LargeWorldPortals;
						this.Config.Data.HugeWorldPortals = new ConfigurationData().HugeWorldPortals;
					}
					if( vers_since < new Version( 1, 6, 0 ) ) {
						this.Config.Data.WormholeSoundVolume = new ConfigurationData().WormholeSoundVolume;
					}

					this.Config.Data.VersionSinceUpdate = WormholesMod.ConfigVersion.ToString();
					this.Config.SaveFile();
				}
			}

			DebugHelper.DEBUGMODE = this.Config.Data.DEBUGFLAGS;

			// Clients and single only
			if( Main.netMode != 2 ) {
				this.UI = new WormholesUI( this );
			}
		}



		////////////////

		public override void HandlePacket( BinaryReader reader, int whoAmI ) {
			WormholesNetProtocol.RoutePacket( this, reader );
		}

		public override void AddRecipeGroups() {
			RecipeGroup group = new RecipeGroup( () => Lang.misc[37] + " Evac Potion", new int[] { 2350, 2997 } );
			RecipeGroup.RegisterGroup( "WormholesMod:EvacPotions", group );
		}

		////////////////

		public override void PostDrawInterface( SpriteBatch sb ) {
			// Clients and single only (redundant?)
			if( Main.netMode == 2 ) { return; }
			
			try {
				if( !Main.mapFullscreen && (Main.mapStyle == 1 || Main.mapStyle == 2) ) {
					this.DrawMiniMap( sb );
				}

				DebugHelper.PrintToBatch( sb );
				DebugHelper.Once = false;
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

			if( !this.Config.Data.DisableNaturalWormholes ) {
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
				if( Main.mapStyle == 1 ) { this.UI.DrawMiniMap( curr_modplayer.MyPortal, sb ); }
				else { this.UI.DrawOverlayMap( curr_modplayer.MyPortal, sb ); }
			}
		}


		private void DrawFullMap( SpriteBatch sb ) {
			this.UI.Update();

			WormholesWorld modworld = this.GetModWorld<WormholesWorld>();
			WormholesPlayer curr_modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>( this );

			if( !this.Config.Data.DisableNaturalWormholes ) {
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

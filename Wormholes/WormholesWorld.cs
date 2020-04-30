using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using HamstarHelpers.Helpers.Debug;


namespace Wormholes {
	class WormholesWorld : ModWorld {
		public static (int minTileX, int maxTileX, int minTileY, int maxTileY) GetTileBoundsForWormholes() {
			int minTileX = 64;
			int maxTileX = Main.maxTilesX - minTileX;
			if( Main.maxTilesX < 64 ) {
				minTileX = 0;
				maxTileX = Main.maxTilesX;
			}

			int minTileY = (int)Main.worldSurface;
			int maxTileY = Main.maxTilesY - 220;
			if( Main.maxTilesY <= 220 || maxTileY <= minTileX ) {
				minTileY = 0;
				maxTileY = Main.maxTilesY;
			}

			return (minTileX, maxTileX, minTileY, maxTileY);
		}

		////////////////



		public WormholeManager Wormholes { get; private set; }
		public string ID { get; private set; }

		public bool HasCorrectID { get; private set; }



		////////////////

		public override void Initialize() {
			var mymod = (WormholesMod)this.mod;

			this.Wormholes = new WormholeManager();
			this.ID = Guid.NewGuid().ToString( "D" );
			this.HasCorrectID = false;

			if( mymod.Config.DebugModeReset ) {
				WormholeManager.ForceRegenWormholes = true;
			}
		}


		public override void Load( TagCompound tags ) {
			if( tags.ContainsKey( "world_id" ) ) {
				string id = tags.GetString( "world_id" );
				this.ID = id;

				var mymod = (WormholesMod)this.mod;
				if( this.Wormholes.Load( tags ) ) {
					this.Wormholes.FinishSettingUpWormholes();
				}
			}

			this.HasCorrectID = true;
			WormholeManager.ForceRegenWormholes = false;
		}

		public override TagCompound Save() {
			var tags = this.Wormholes.Save();
			if( this.HasCorrectID ) {
				tags.Set( "world_id", this.ID );
			}
			return tags;
		}


		public override void NetSend( BinaryWriter writer ) {
			writer.Write( this.HasCorrectID );
			writer.Write( this.ID );
		}

		public override void NetReceive( BinaryReader reader ) {
			bool hasCorrectId = reader.ReadBoolean();
			string id = reader.ReadString();

			if( hasCorrectId ) {
				this.HasCorrectID = hasCorrectId;
				this.ID = id;

				var modplayer = Main.player[Main.myPlayer].GetModPlayer<WormholesPlayer>();
				if( modplayer.HasEnteredWorld && !modplayer.HasLoadedTownPortals ) {
					modplayer.ReopenTownPortal();
				}
			}
		}

		////////////////

		public void SetupWormholes() {
			this.Wormholes.FinishSettingUpWormholes();
			this.HasCorrectID = true;
		}

		public override void ModifyWorldGenTasks( List<GenPass> tasks, ref float totalWeight ) {
			tasks.Add( new PassLegacy( "Place Wormholes", delegate ( GenerationProgress progress ) {
				this.SetupWormholes();
			} ) );
		}

		////////////////

		public override void PostDrawTiles() {
			if( this.Wormholes == null ) { return; }

			Player player = Main.player[Main.myPlayer];
			var myplayer = player.GetModPlayer<WormholesPlayer>();
			var mymod = (WormholesMod)this.mod;
			
			//Main.spriteBatch.Begin();
			RasterizerState rasterizer = Main.gameMenu || (double)Main.player[Main.myPlayer].gravDir == 1.0 ? RasterizerState.CullCounterClockwise : RasterizerState.CullClockwise;
			Main.spriteBatch.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, rasterizer, (Effect)null, Main.GameViewMatrix.TransformationMatrix );
			
			try {
				this.Wormholes.DrawAll( myplayer.MyPortal );
			} catch( Exception e ) {
				LogHelpers.Log( "PostDrawTiles: " + e.ToString() );
				throw e;
			}
			Main.spriteBatch.End();
		}
	}
}

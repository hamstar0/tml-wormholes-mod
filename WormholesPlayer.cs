using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.PlayerHelpers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Wormholes.Items;
using Wormholes.NetProtocols;


namespace Wormholes {
	class WormholesPlayer : ModPlayer {
		public WormholeLink MyPortal { get; internal set; }
		public ISet<string> ChartedLinks { get; private set; }

		private IDictionary<string, Vector2> TownPortalRightPositions = new Dictionary<string, Vector2>();
		private IDictionary<string, Vector2> TownPortalLeftPositions = new Dictionary<string, Vector2>();

		public bool HasEnteredWorld { get; private set; }
		public bool HasLoadedTownPortals { get; private set; }



		////////////////

		public override bool CloneNewInstances => false;

		public override void Initialize() {
			this.ChartedLinks = new HashSet<string>();
		}

		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );
			var myclone = (WormholesPlayer)clone;
			
			myclone.MyPortal = this.MyPortal;
			myclone.ChartedLinks = this.ChartedLinks;
			myclone.TownPortalRightPositions = this.TownPortalRightPositions;
			myclone.TownPortalLeftPositions = this.TownPortalLeftPositions;
			myclone.HasEnteredWorld = this.HasEnteredWorld;
			myclone.HasLoadedTownPortals = this.HasLoadedTownPortals;
		}


		////////////////

		public override void Load( TagCompound tag ) {
			var modworld = this.mod.GetModWorld<WormholesWorld>();
			int wormholes = tag.GetInt( "wormholes_count" );

			this.TownPortalRightPositions = new Dictionary<string, Vector2>();
			this.TownPortalLeftPositions = new Dictionary<string, Vector2>();

			for( int i = 0; i < wormholes; i++ ) {
				string id = tag.GetString( "wormhole_id_"+i );
				if( id == "" ) { continue; }
				this.ChartedLinks.Add( id );
			}

			int my_portal_count = tag.GetInt( "my_town_portal_count" );
			for( int i=0; i<my_portal_count; i++ ) {
				string world_id = tag.GetString( "my_town_portal_id_" + i );
				float right_x = tag.GetFloat( "my_town_right_portal_x_" + i );
				float right_y = tag.GetFloat( "my_town_right_portal_y_" + i );
				float left_x = tag.GetFloat( "my_town_left_portal_x_" + i );
				float left_y = tag.GetFloat( "my_town_left_portal_y_" + i );
				var right = new Vector2( right_x, right_y );
				var left = new Vector2( left_x, left_y );

				this.TownPortalRightPositions[ world_id ] = right;
				this.TownPortalLeftPositions[ world_id ] = left;
			}
		}

		public override TagCompound Save() {
			var modworld = this.mod.GetModWorld<WormholesWorld>();
			string world_id = modworld.ID;
			var tags = new TagCompound {
				{ "wormholes_count", (int)this.ChartedLinks.Count }
			};
			
			int i = 0;
			foreach( string id in this.ChartedLinks ) {
				var link = modworld.Wormholes.GetLinkById( id );
				if( link != null && link.IsClosed ) { continue; }

				tags.Set( "wormhole_id_" + i++, (string)id );
			}

			if( this.MyPortal != null ) {
				if( this.MyPortal.IsClosed ) {
					if( this.TownPortalRightPositions.Keys.Contains(world_id) ) {
						this.TownPortalRightPositions.Remove( world_id );
						this.TownPortalLeftPositions.Remove( world_id );
					}
				} else {
					this.TownPortalRightPositions[world_id] = this.MyPortal.RightPortal.Pos;
					this.TownPortalLeftPositions[world_id] = this.MyPortal.LeftPortal.Pos;
				}
			}

			tags.Set( "my_town_portal_count", this.TownPortalRightPositions.Count );
			i = 0;
			foreach( string id in this.TownPortalRightPositions.Keys ) {
				tags.Set( "my_town_portal_id_"+i, id );
				tags.Set( "my_town_right_portal_x_" + i, this.TownPortalRightPositions[id].X );
				tags.Set( "my_town_right_portal_y_" + i, this.TownPortalRightPositions[id].Y );
				tags.Set( "my_town_left_portal_x_" + i, this.TownPortalLeftPositions[id].X );
				tags.Set( "my_town_left_portal_y_" + i, this.TownPortalLeftPositions[id].Y );
//ErrorLogger.Log( "save for "+id+": "+this.TownPortalLeftPositions[id]+", "+this.TownPortalRightPositions[id] );
				i++;
			}

			return tags;
		}

		////////////////
		
		public override void SyncPlayer( int to_who, int from_who, bool new_player ) {
			var mymod = (WormholesMod)this.mod;

			if( Main.netMode == 2 ) {
				if( to_who == -1 && from_who == this.player.whoAmI ) {
					this.OnServerConnect();
				}
			}
		}

		public override void OnEnterWorld( Player player ) {
			if( player.whoAmI != Main.myPlayer ) { return; }
			if( this.player.whoAmI != Main.myPlayer ) { return; }

			var mymod = (WormholesMod)this.mod;

			if( Main.netMode == 0 ) {
				if( !mymod.ConfigJson.LoadFile() ) {
					mymod.ConfigJson.SaveFile();
					ErrorLogger.Log( "Wormholes config " + mymod.Version.ToString() + " created (ModPlayer.OnEnterWorld())." );
				}
			}

			if( mymod.Config.DebugModeInfo ) {
				ErrorLogger.Log( "Wormholes.WormholesPlayer.OnEnterWorld - " + player.name + " joined (" + PlayerIdentityHelpers.GetProperUniqueId( player ) + ")" );
			}

			if( Main.netMode == 0 ) {
				this.OnSingleConnect();
			}
			if( Main.netMode == 1 ) {
				this.OnCurrentClientConnect();
			}
		}

		////////////////

		private void OnLocalConnect() {
			var mymod = (WormholesMod)this.mod;
			var myworld = this.mod.GetModWorld<WormholesWorld>();

			if( myworld.HasCorrectID ) {
				this.ReopenTownPortal();
			} else {
				this.HasLoadedTownPortals = false;
			}
		}


		private void OnSingleConnect() {
			var myworld = this.mod.GetModWorld<WormholesWorld>();

			this.OnLocalConnect();

			myworld.SetupWormholes();

			this.HasEnteredWorld = true;
		}

		private void OnCurrentClientConnect() {
			this.OnLocalConnect();

			PacketProtocolRequestToServer.QuickRequestToServer<SettingsAndWormholesProtocol>( -1 );

			this.HasEnteredWorld = true;
		}

		private void OnServerConnect() { }


		////////////////

		public void ReopenTownPortal() {
			var mymod = (WormholesMod)this.mod;
			var modworld = this.mod.GetModWorld<WormholesWorld>();
			string world_id = modworld.ID;

			if( this.TownPortalRightPositions.Keys.Contains( world_id ) ) {
				Vector2 r_pos = this.TownPortalRightPositions[world_id];
				Vector2 l_pos = this.TownPortalLeftPositions[world_id];
				TownPortalScrollItem.OpenPortal( this.player, r_pos, l_pos );
			}

			this.HasLoadedTownPortals = true;
		}
		

		////////////////

		public override void PreUpdate() {
			if( Main.netMode == 2 ) { return; } // Not server

			var mymod = (WormholesMod)this.mod;
			WormholesWorld modworld = mymod.GetModWorld<WormholesWorld>();
			if( modworld.Wormholes == null ) { return; }

			modworld.Wormholes.RunAll( this.player );
		}
	}
}

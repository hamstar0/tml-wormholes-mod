using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Wormholes.Items;
using Wormholes.NetProtocol;


namespace Wormholes {
	class MyPlayer : ModPlayer {
		public WormholeLink MyPortal;
		public ISet<string> ChartedLinks { get; private set; }

		private IDictionary<string, Vector2> TownPortalRightPositions = new Dictionary<string, Vector2>();
		private IDictionary<string, Vector2> TownPortalLeftPositions = new Dictionary<string, Vector2>();

		public bool HasEnteredWorld { get; private set; }
		public bool HasLoadedTownPortals { get; private set; }
		

		////////////////

		public override void Initialize() {
			this.ChartedLinks = new HashSet<string>();
		}

		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );
			var myclone = (MyPlayer)clone;
			
			myclone.MyPortal = this.MyPortal;
			myclone.ChartedLinks = this.ChartedLinks;
			myclone.TownPortalRightPositions = this.TownPortalRightPositions;
			myclone.TownPortalLeftPositions = this.TownPortalLeftPositions;
			myclone.HasEnteredWorld = this.HasEnteredWorld;
			myclone.HasLoadedTownPortals = this.HasLoadedTownPortals;
		}


		public override void Load( TagCompound tag ) {
			var modworld = this.mod.GetModWorld<MyWorld>();
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
			var modworld = this.mod.GetModWorld<MyWorld>();
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
		
		public override void OnEnterWorld( Player player ) {
			if( Main.netMode == 2 ) { return; }   // Not server

			if( player.whoAmI == this.player.whoAmI ) { // Current player
				var mymod = (WormholesMod)this.mod;
				var modworld = this.mod.GetModWorld<MyWorld>();

				if( !mymod.Config.LoadFile() ) {
					mymod.Config.SaveFile();
				}
				
				if( modworld.HasCorrectID ) {
					this.ReopenTownPortal();
				} else {
					this.HasLoadedTownPortals = false;
				}

				if( Main.netMode == 1 ) {    // Client
					ClientPacketHandlers.SendWormholesAndSettingsRequestViaClient( mymod, player );
				} else if( Main.netMode == 0 ) {  // Single
					modworld.SetupWormholes();
				}

				this.HasEnteredWorld = true;
			}
		}

		public void ReopenTownPortal() {
			var mymod = (WormholesMod)this.mod;
			var modworld = this.mod.GetModWorld<MyWorld>();
			string world_id = modworld.ID;

			if( this.TownPortalRightPositions.Keys.Contains( world_id ) ) {
				Vector2 r_pos = this.TownPortalRightPositions[world_id];
				Vector2 l_pos = this.TownPortalLeftPositions[world_id];
				TownPortalScrollItem.OpenPortal( mymod, this.player, r_pos, l_pos );
			}

			this.HasLoadedTownPortals = true;
		}
			
		////////////////

		public override void PreUpdate() {
			if( Main.netMode == 2 ) { return; } // Not server

			var mymod = (WormholesMod)this.mod;
			MyWorld modworld = mymod.GetModWorld<MyWorld>();
			if( modworld.Wormholes == null ) { return; }

			modworld.Wormholes.RunAll( mymod.Context, this.player );
		}
	}
}

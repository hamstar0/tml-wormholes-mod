using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Wormholes.Items;

namespace Wormholes {
	public class WormholesPlayer : ModPlayer {
		public WormholeLink MyPortal;
		public ISet<string> ChartedLinks { get; private set; }

		private IDictionary<string, Vector2> MyPortalsRightPositions = new Dictionary<string, Vector2>();
		private IDictionary<string, Vector2> MyPortalsLeftPositions = new Dictionary<string, Vector2>();

		public bool HasEnteredWorld { get; private set; }
		public bool HasLoadedTownPortals { get; private set; }
		

		////////////////

		public override void Initialize() {
			this.ChartedLinks = new HashSet<string>();
		}

		public override void clientClone( ModPlayer clone ) {
			base.clientClone( clone );
			var myclone = (WormholesPlayer)clone;
			
			myclone.MyPortal = this.MyPortal;
			myclone.ChartedLinks = this.ChartedLinks;
			myclone.MyPortalsRightPositions = this.MyPortalsRightPositions;
			myclone.MyPortalsLeftPositions = this.MyPortalsLeftPositions;
			myclone.HasEnteredWorld = this.HasEnteredWorld;
			myclone.HasLoadedTownPortals = this.HasLoadedTownPortals;
		}


		public override void Load( TagCompound tag ) {
			int wormholes = tag.GetInt( "wormholes_count" );
			
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

				this.MyPortalsRightPositions[ world_id ] = new Vector2( right_x, right_y );
				this.MyPortalsLeftPositions[ world_id ] = new Vector2( left_x, left_y );
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
					if( this.MyPortalsRightPositions.Keys.Contains(world_id) ) {
						this.MyPortalsRightPositions.Remove( world_id );
						this.MyPortalsLeftPositions.Remove( world_id );
					}
				} else {
					this.MyPortalsRightPositions[world_id] = this.MyPortal.RightPortal.Pos;
					this.MyPortalsLeftPositions[world_id] = this.MyPortal.LeftPortal.Pos;
				}
			}

			tags.Set( "my_town_portal_count", this.MyPortalsRightPositions.Count );
			i = 0;
			foreach( string id in this.MyPortalsRightPositions.Keys ) {
				tags.Set( "my_town_portal_id_"+i, id );
				tags.Set( "my_town_right_portal_x_" + i, this.MyPortalsRightPositions[id].X );
				tags.Set( "my_town_right_portal_y_" + i, this.MyPortalsRightPositions[id].Y );
				tags.Set( "my_town_left_portal_x_" + i, this.MyPortalsLeftPositions[id].X );
				tags.Set( "my_town_left_portal_y_" + i, this.MyPortalsLeftPositions[id].Y );
				i++;
			}

			return tags;
		}

		////////////////
		
		public override void OnEnterWorld( Player player ) {
			if( Main.netMode == 2 ) { return; }   // Not server

			if( player.whoAmI == this.player.whoAmI ) { // Current player
				var mymod = (WormholesMod)this.mod;
				var modworld = this.mod.GetModWorld<WormholesWorld>();
				string world_id = modworld.ID;

				if( !mymod.Config.LoadFile() ) {
					mymod.Config.SaveFile();
				}
				
				if( modworld.HasCorrectID ) {
					this.ReopenTownPortal();
				} else {
					this.HasLoadedTownPortals = false;
				}

				if( Main.netMode == 1 ) {    // Client
					WormholesNetProtocol.SendWormholesAndSettingsRequestViaClient( mymod, player );
				} else if( Main.netMode == 0 ) {  // Single
					modworld.Wormholes.SetupWormholes( mymod );
				}

				this.HasEnteredWorld = true;
			}
		}

		public void ReopenTownPortal() {
			var mymod = (WormholesMod)this.mod;
			var modworld = this.mod.GetModWorld<WormholesWorld>();
			string world_id = modworld.ID;

			if( this.MyPortalsRightPositions.Keys.Contains( world_id ) ) {
				Vector2 r_pos = this.MyPortalsRightPositions[world_id];
				Vector2 l_pos = this.MyPortalsLeftPositions[world_id];
				TownPortalScrollItem.OpenPortal( mymod, this.player, r_pos, l_pos );
			}

			this.HasLoadedTownPortals = true;
		}
			
		////////////////

		public override void PreUpdate() {
			if( Main.netMode == 2 ) { return; } // Not server

			var mymod = (WormholesMod)this.mod;
			WormholesWorld modworld = mymod.GetModWorld<WormholesWorld>();
			if( modworld.Wormholes == null ) { return; }

			modworld.Wormholes.RunAll( mymod, this.player );
		}
	}
}

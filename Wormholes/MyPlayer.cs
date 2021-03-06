﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.Debug;
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
			int wormholes = tag.GetInt( "wormholes_count" );

			this.TownPortalRightPositions = new Dictionary<string, Vector2>();
			this.TownPortalLeftPositions = new Dictionary<string, Vector2>();

			for( int i = 0; i < wormholes; i++ ) {
				string id = tag.GetString( "wormhole_id_"+i );
				if( id == "" ) { continue; }
				this.ChartedLinks.Add( id );
			}

			int myPortalCount = tag.GetInt( "my_town_portal_count" );
			for( int i=0; i<myPortalCount; i++ ) {
				string worldId = tag.GetString( "my_town_portal_id_" + i );
				float rightX = tag.GetFloat( "my_town_right_portal_x_" + i );
				float rightY = tag.GetFloat( "my_town_right_portal_y_" + i );
				float leftX = tag.GetFloat( "my_town_left_portal_x_" + i );
				float leftY = tag.GetFloat( "my_town_left_portal_y_" + i );
				var right = new Vector2( rightX, rightY );
				var left = new Vector2( leftX, leftY );

				this.TownPortalRightPositions[ worldId ] = right;
				this.TownPortalLeftPositions[ worldId ] = left;
			}
		}

		public override TagCompound Save() {
			var modworld = ModContent.GetInstance<WormholesWorld>();
			string worldId = modworld.ID;
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
					if( this.TownPortalRightPositions.Keys.Contains(worldId) ) {
						this.TownPortalRightPositions.Remove( worldId );
						this.TownPortalLeftPositions.Remove( worldId );
					}
				} else {
					this.TownPortalRightPositions[worldId] = this.MyPortal.RightPortal.Pos;
					this.TownPortalLeftPositions[worldId] = this.MyPortal.LeftPortal.Pos;
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
		
		private void OnLocalConnect() {
			var mymod = (WormholesMod)this.mod;
			var myworld = ModContent.GetInstance<WormholesWorld>();

			if( myworld.HasCorrectID ) {
				this.ReopenTownPortal();
			} else {
				this.HasLoadedTownPortals = false;
			}
		}


		internal void OnSingleConnect() {
			var myworld = ModContent.GetInstance<WormholesWorld>();

			this.OnLocalConnect();

			myworld.SetupWormholes();

			this.HasEnteredWorld = true;
		}

		internal void OnCurrentClientConnect() {
			this.OnLocalConnect();

			WormholesProtocol.QuickRequest();

			this.HasEnteredWorld = true;
		}

		internal void OnServerConnect() { }


		////////////////

		public void ReopenTownPortal() {
			var mymod = (WormholesMod)this.mod;
			var modworld = ModContent.GetInstance<WormholesWorld>();
			string worldId = modworld.ID;

			if( this.TownPortalRightPositions.Keys.Contains( worldId ) ) {
				Vector2 rPos = this.TownPortalRightPositions[worldId];
				Vector2 lPos = this.TownPortalLeftPositions[worldId];
				TownPortalScrollItem.OpenPortal( this.player, rPos, lPos );
			}

			this.HasLoadedTownPortals = true;
		}
		

		////////////////

		public override void PreUpdate() {
			if( Main.netMode == 2 ) { return; } // Not server

			var mymod = (WormholesMod)this.mod;
			WormholesWorld modworld = ModContent.GetInstance<WormholesWorld>();
			if( modworld.Wormholes == null ) { return; }

			modworld.Wormholes.RunAll( this.player );
		}
	}
}

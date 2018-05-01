using HamstarHelpers.Utilities.Network;
using Microsoft.Xna.Framework;


namespace Wormholes.NetProtocols {
	class SettingsAndWormholesProtocol : PacketProtocol {
		public WormholesConfigData ModSettings;
		public string[] Ids;
		public float[] RightPosX;
		public float[] RightPosY;
		public float[] LeftPosX;
		public float[] LeftPosY;



		////////////////

		public override void SetServerDefaults() {
			var mymod = WormholesMod.Instance;
			var modworld = mymod.GetModWorld<WormholesWorld>();

			// Be sure our wormholes are ready to send (if not already)
			modworld.SetupWormholes();

			this.ModSettings = mymod.Config;

			int count = WormholeManager.PortalCount;

			this.Ids = new string[count];
			this.RightPosX = new float[count];
			this.RightPosY = new float[count];
			this.LeftPosX = new float[count];
			this.LeftPosY = new float[count];

			for( int i=0; i<count; i++ ) {
				var link = modworld.Wormholes.Links[i];

				this.Ids[i] = link.ID;
				this.RightPosX[i] = link.RightPortal.Pos.X;
				this.RightPosY[i] = link.RightPortal.Pos.Y;
				this.LeftPosX[i] = link.LeftPortal.Pos.X;
				this.LeftPosY[i] = link.LeftPortal.Pos.Y;
			}
		}

		////////////////

		protected override void ReceiveWithClient() {
			var mymod = WormholesMod.Instance;
			var myworld = mymod.GetModWorld<WormholesWorld>();
			
			mymod.JsonConfig.SetData( this.ModSettings );

			for( int i=0; i<this.Ids.Length; i++ ) {
				var pos_r = new Vector2( this.RightPosX[i], this.RightPosY[i] );
				var pos_l = new Vector2( this.LeftPosX[i], this.LeftPosY[i] );

				var link = new WormholeLink( this.Ids[i], WormholeLink.GetColor(i), pos_l, pos_r );
				myworld.Wormholes.Links.Insert( i, link );
			}
		}
	}
}

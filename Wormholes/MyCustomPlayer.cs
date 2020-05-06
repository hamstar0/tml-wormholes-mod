using Terraria;
using HamstarHelpers.Classes.PlayerData;
using HamstarHelpers.Helpers.TModLoader;


namespace Wormholes {
	class WormholesCustomPlayer : CustomPlayerData {
		protected override void OnEnter( object data ) {
			var myplayer = TmlHelpers.SafelyGetModPlayer<WormholesPlayer>( this.Player );

			if( Main.netMode == 0 ) {
				myplayer.OnSingleConnect();
			}
			if( Main.netMode == 1 && this.PlayerWho == Main.myPlayer ) {
				myplayer.OnCurrentClientConnect();
			}
			if( Main.netMode == 2 ) {
				myplayer.OnServerConnect();
			}
		}
	}
}

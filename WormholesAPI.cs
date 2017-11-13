namespace Wormholes {
	public static class WormholesAPI {
		public static WormholesConfigData GetModSettings() {
			return WormholesMod.Instance.Config.Data;
		}
	}
}

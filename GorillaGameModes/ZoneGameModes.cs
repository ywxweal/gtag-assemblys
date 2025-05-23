using System;

namespace GorillaGameModes
{
	// Token: 0x02000ABC RID: 2748
	[Serializable]
	public struct ZoneGameModes
	{
		// Token: 0x040044B5 RID: 17589
		public GTZone[] zone;

		// Token: 0x040044B6 RID: 17590
		public GameModeType[] modes;

		// Token: 0x040044B7 RID: 17591
		public GameModeType[] privateModes;
	}
}

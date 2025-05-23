using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x02000D89 RID: 3465
	[Flags]
	public enum EEdCosBrowserPartsFilter
	{
		// Token: 0x040059DD RID: 23005
		None = 0,
		// Token: 0x040059DE RID: 23006
		NoParts = 1,
		// Token: 0x040059DF RID: 23007
		Holdable = 2,
		// Token: 0x040059E0 RID: 23008
		Functional = 4,
		// Token: 0x040059E1 RID: 23009
		Wardrobe = 8,
		// Token: 0x040059E2 RID: 23010
		Store = 16,
		// Token: 0x040059E3 RID: 23011
		FirstPerson = 32,
		// Token: 0x040059E4 RID: 23012
		All = 63
	}
}

using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x02000D89 RID: 3465
	[Flags]
	public enum EEdCosBrowserPartsFilter
	{
		// Token: 0x040059DE RID: 23006
		None = 0,
		// Token: 0x040059DF RID: 23007
		NoParts = 1,
		// Token: 0x040059E0 RID: 23008
		Holdable = 2,
		// Token: 0x040059E1 RID: 23009
		Functional = 4,
		// Token: 0x040059E2 RID: 23010
		Wardrobe = 8,
		// Token: 0x040059E3 RID: 23011
		Store = 16,
		// Token: 0x040059E4 RID: 23012
		FirstPerson = 32,
		// Token: 0x040059E5 RID: 23013
		All = 63
	}
}

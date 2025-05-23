using System;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class FlagCauldronColorer : MonoBehaviour
{
	// Token: 0x04000D36 RID: 3382
	public FlagCauldronColorer.ColorMode mode;

	// Token: 0x04000D37 RID: 3383
	public Transform colorPoint;

	// Token: 0x020001CA RID: 458
	public enum ColorMode
	{
		// Token: 0x04000D39 RID: 3385
		None,
		// Token: 0x04000D3A RID: 3386
		Red,
		// Token: 0x04000D3B RID: 3387
		Green,
		// Token: 0x04000D3C RID: 3388
		Blue,
		// Token: 0x04000D3D RID: 3389
		Black,
		// Token: 0x04000D3E RID: 3390
		Clear
	}
}

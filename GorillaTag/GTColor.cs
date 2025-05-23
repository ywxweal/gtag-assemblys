using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D11 RID: 3345
	public static class GTColor
	{
		// Token: 0x060053D8 RID: 21464 RVA: 0x00196828 File Offset: 0x00194A28
		public static Color RandomHSV(GTColor.HSVRanges ranges)
		{
			return Color.HSVToRGB(Random.Range(ranges.h.x, ranges.h.y), Random.Range(ranges.s.x, ranges.s.y), Random.Range(ranges.v.x, ranges.v.y));
		}

		// Token: 0x02000D12 RID: 3346
		[Serializable]
		public struct HSVRanges
		{
			// Token: 0x060053D9 RID: 21465 RVA: 0x0019688B File Offset: 0x00194A8B
			public HSVRanges(float hMin = 0f, float hMax = 1f, float sMin = 0f, float sMax = 1f, float vMin = 0f, float vMax = 1f)
			{
				this.h = new Vector2(hMin, hMax);
				this.s = new Vector2(sMin, sMax);
				this.v = new Vector2(vMin, vMax);
			}

			// Token: 0x040056D3 RID: 22227
			public Vector2 h;

			// Token: 0x040056D4 RID: 22228
			public Vector2 s;

			// Token: 0x040056D5 RID: 22229
			public Vector2 v;
		}
	}
}

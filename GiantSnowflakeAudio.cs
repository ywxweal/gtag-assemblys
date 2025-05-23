using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005D6 RID: 1494
public class GiantSnowflakeAudio : MonoBehaviour
{
	// Token: 0x0600246F RID: 9327 RVA: 0x000B72D0 File Offset: 0x000B54D0
	private void Start()
	{
		foreach (GiantSnowflakeAudio.SnowflakeScaleOverride snowflakeScaleOverride in this.audioOverrides)
		{
			if (base.transform.lossyScale.x < snowflakeScaleOverride.scaleMax)
			{
				base.GetComponent<GorillaSurfaceOverride>().overrideIndex = snowflakeScaleOverride.newOverrideIndex;
			}
		}
	}

	// Token: 0x0400298E RID: 10638
	public List<GiantSnowflakeAudio.SnowflakeScaleOverride> audioOverrides;

	// Token: 0x020005D7 RID: 1495
	[Serializable]
	public struct SnowflakeScaleOverride
	{
		// Token: 0x0400298F RID: 10639
		public float scaleMax;

		// Token: 0x04002990 RID: 10640
		[GorillaSoundLookup]
		public int newOverrideIndex;
	}
}

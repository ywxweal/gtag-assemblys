using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000247 RID: 583
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestGorillaSlot : MonoBehaviour
{
	// Token: 0x06000D58 RID: 3416 RVA: 0x00045D33 File Offset: 0x00043F33
	private void Start()
	{
		this.localStartPosition = base.transform.localPosition;
	}

	// Token: 0x040010EB RID: 4331
	public PerfTestGorillaSlot.SlotType slotType;

	// Token: 0x040010EC RID: 4332
	public Vector3 localStartPosition;

	// Token: 0x02000248 RID: 584
	public enum SlotType
	{
		// Token: 0x040010EE RID: 4334
		VR_PLAYER,
		// Token: 0x040010EF RID: 4335
		DUMMY
	}
}

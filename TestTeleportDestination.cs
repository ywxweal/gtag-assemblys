using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200024B RID: 587
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class TestTeleportDestination : MonoBehaviour
{
	// Token: 0x06000D5D RID: 3421 RVA: 0x00045D54 File Offset: 0x00043F54
	private void OnDrawGizmosSelected()
	{
		Debug.DrawRay(base.transform.position, base.transform.forward * 2f, Color.magenta);
	}

	// Token: 0x040010F0 RID: 4336
	public GTZone[] zones;

	// Token: 0x040010F1 RID: 4337
	public GameObject teleportTransform;
}

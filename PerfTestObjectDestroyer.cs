using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200024A RID: 586
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestObjectDestroyer : MonoBehaviour
{
	// Token: 0x06000D5B RID: 3419 RVA: 0x00045D46 File Offset: 0x00043F46
	private void Start()
	{
		Object.DestroyImmediate(base.gameObject, true);
	}
}

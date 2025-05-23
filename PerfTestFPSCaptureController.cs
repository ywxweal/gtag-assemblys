using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000245 RID: 581
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestFPSCaptureController : MonoBehaviour
{
	// Token: 0x040010E4 RID: 4324
	[SerializeField]
	private SerializablePerformanceReport<ScenePerformanceData> performanceSummary;
}

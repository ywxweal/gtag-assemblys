using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C2 RID: 450
public class DevInspectorScanner : MonoBehaviour
{
	// Token: 0x04000D2E RID: 3374
	public Text hintTextOutput;

	// Token: 0x04000D2F RID: 3375
	public float scanDistance = 10f;

	// Token: 0x04000D30 RID: 3376
	public float scanAngle = 30f;

	// Token: 0x04000D31 RID: 3377
	public LayerMask scanLayerMask;

	// Token: 0x04000D32 RID: 3378
	public string targetComponentName;

	// Token: 0x04000D33 RID: 3379
	public float rayPerDegree = 10f;
}

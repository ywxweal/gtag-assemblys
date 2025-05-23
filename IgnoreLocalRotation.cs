using System;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class IgnoreLocalRotation : MonoBehaviour
{
	// Token: 0x060009C9 RID: 2505 RVA: 0x0003426F File Offset: 0x0003246F
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}

using System;
using UnityEngine;

// Token: 0x020008F1 RID: 2289
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x0600378C RID: 14220 RVA: 0x0010BFF9 File Offset: 0x0010A1F9
	private void LateUpdate()
	{
		Physics.SyncTransforms();
	}
}

using System;
using UnityEngine;

// Token: 0x020008F1 RID: 2289
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x0600378D RID: 14221 RVA: 0x0010C0D1 File Offset: 0x0010A2D1
	private void LateUpdate()
	{
		Physics.SyncTransforms();
	}
}

using System;
using UnityEngine;

// Token: 0x020004A1 RID: 1185
public class GorillaTriggerBoxTeleport : GorillaTriggerBox
{
	// Token: 0x06001CB7 RID: 7351 RVA: 0x0008BA39 File Offset: 0x00089C39
	public override void OnBoxTriggered()
	{
		this.cameraOffest.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
		this.cameraOffest.transform.position = this.teleportLocation;
	}

	// Token: 0x04001FF3 RID: 8179
	public Vector3 teleportLocation;

	// Token: 0x04001FF4 RID: 8180
	public GameObject cameraOffest;
}

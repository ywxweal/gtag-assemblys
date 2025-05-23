using System;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public class LocomotionController : MonoBehaviour
{
	// Token: 0x060011B8 RID: 4536 RVA: 0x000552F0 File Offset: 0x000534F0
	private void Start()
	{
		if (this.CameraRig == null)
		{
			this.CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		}
	}

	// Token: 0x040013EB RID: 5099
	public OVRCameraRig CameraRig;

	// Token: 0x040013EC RID: 5100
	public CapsuleCollider CharacterController;

	// Token: 0x040013ED RID: 5101
	public SimpleCapsuleWithStickMovement PlayerController;
}

using System;
using UnityEngine;

// Token: 0x02000347 RID: 839
public class SelectivePassthroughExperience : MonoBehaviour
{
	// Token: 0x060013D0 RID: 5072 RVA: 0x000600BC File Offset: 0x0005E2BC
	private void Update()
	{
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
		bool flag = OVRInput.GetActiveController() == OVRInput.Controller.LTouch || OVRInput.GetActiveController() == OVRInput.Controller.RTouch || OVRInput.GetActiveController() == OVRInput.Controller.Touch;
		this.leftMaskObject.SetActive(flag);
		this.rightMaskObject.SetActive(flag);
		if (flag)
		{
			Vector3 vector = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward * 0.1f;
			Vector3 vector2 = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward * 0.1f;
			this.leftMaskObject.transform.position = vector;
			this.rightMaskObject.transform.position = vector2;
			return;
		}
		if (OVRInput.GetActiveController() != OVRInput.Controller.LHand && OVRInput.GetActiveController() != OVRInput.Controller.RHand)
		{
			OVRInput.GetActiveController();
		}
	}

	// Token: 0x04001602 RID: 5634
	public GameObject leftMaskObject;

	// Token: 0x04001603 RID: 5635
	public GameObject rightMaskObject;
}

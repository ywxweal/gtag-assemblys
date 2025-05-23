using System;
using UnityEngine;

// Token: 0x0200033C RID: 828
public class OverlayPassthrough : MonoBehaviour
{
	// Token: 0x06001394 RID: 5012 RVA: 0x0005F27C File Offset: 0x0005D47C
	private void Start()
	{
		GameObject gameObject = GameObject.Find("OVRCameraRig");
		if (gameObject == null)
		{
			Debug.LogError("Scene does not contain an OVRCameraRig");
			return;
		}
		this.passthroughLayer = gameObject.GetComponent<OVRPassthroughLayer>();
		if (this.passthroughLayer == null)
		{
			Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
		}
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x0005F2CC File Offset: 0x0005D4CC
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
		{
			this.passthroughLayer.hidden = !this.passthroughLayer.hidden;
		}
		float x = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).x;
		this.passthroughLayer.textureOpacity = x * 0.5f + 0.5f;
	}

	// Token: 0x040015C6 RID: 5574
	private OVRPassthroughLayer passthroughLayer;
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020002E0 RID: 736
public class HandedInputSelector : MonoBehaviour
{
	// Token: 0x060011A0 RID: 4512 RVA: 0x00054AE6 File Offset: 0x00052CE6
	private void Start()
	{
		this.m_CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		this.m_InputModule = Object.FindObjectOfType<OVRInputModule>();
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x00054AFE File Offset: 0x00052CFE
	private void Update()
	{
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			this.SetActiveController(OVRInput.Controller.LTouch);
			return;
		}
		this.SetActiveController(OVRInput.Controller.RTouch);
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x00054B18 File Offset: 0x00052D18
	private void SetActiveController(OVRInput.Controller c)
	{
		Transform transform;
		if (c == OVRInput.Controller.LTouch)
		{
			transform = this.m_CameraRig.leftHandAnchor;
		}
		else
		{
			transform = this.m_CameraRig.rightHandAnchor;
		}
		this.m_InputModule.rayTransform = transform;
	}

	// Token: 0x040013D2 RID: 5074
	private OVRCameraRig m_CameraRig;

	// Token: 0x040013D3 RID: 5075
	private OVRInputModule m_InputModule;
}

using System;
using UnityEngine;

// Token: 0x02000382 RID: 898
public class LocalizedHaptics : MonoBehaviour
{
	// Token: 0x060014CA RID: 5322 RVA: 0x000654D8 File Offset: 0x000636D8
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x000654F0 File Offset: 0x000636F0
	private void Update()
	{
		float num = ((OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, this.m_controller) > 0.5f) ? 1f : 0f);
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Thumb, 0f, num, this.m_controller);
		float num2 = ((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller) > 0.5f) ? 1f : 0f);
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Index, 0f, num2, this.m_controller);
		float num3 = ((OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller) > 0.5f) ? 1f : 0f);
		OVRInput.SetControllerLocalizedVibration(OVRInput.HapticsLocation.Hand, 0f, num3, this.m_controller);
	}

	// Token: 0x0400171A RID: 5914
	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	// Token: 0x0400171B RID: 5915
	private OVRInput.Controller m_controller;
}

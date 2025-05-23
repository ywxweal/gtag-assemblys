using System;
using TMPro;
using UnityEngine;

// Token: 0x02000388 RID: 904
public class UiDeviceInspector : MonoBehaviour
{
	// Token: 0x060014E0 RID: 5344 RVA: 0x00065BB3 File Offset: 0x00063DB3
	private void Start()
	{
		this.m_controller = ((this.m_handedness == OVRInput.Handedness.LeftHanded) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x00065BC8 File Offset: 0x00063DC8
	private void Update()
	{
		string text = UiDeviceInspector.ToDeviceModel() + " [" + UiDeviceInspector.ToHandednessString(this.m_handedness) + "]";
		this.m_title.SetText(text, true);
		string text2 = (OVRInput.IsControllerConnected(this.m_controller) ? "<color=#66ff87>o</color>" : "<color=#ff8991>x</color>");
		string text3 = ((OVRInput.GetControllerOrientationTracked(this.m_controller) && OVRInput.GetControllerPositionTracked(this.m_controller)) ? "<color=#66ff87>o</color>" : "<color=#ff8991>x</color>");
		this.m_status.SetText(string.Concat(new string[] { "Connected [", text2, "] Tracked [", text3, "]" }), true);
		this.m_thumbRestTouch.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, this.m_controller));
		this.m_indexTrigger.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller));
		this.m_gripTrigger.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller));
		this.m_thumbRestForce.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryThumbRestForce, this.m_controller));
		this.m_stylusTipForce.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryStylusForce, this.m_controller));
		this.m_indexCurl1d.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerCurl, this.m_controller));
		this.m_indexSlider1d.SetValue(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTriggerSlide, this.m_controller));
		this.m_ax.SetValue(OVRInput.Get(OVRInput.Button.One, this.m_controller));
		this.m_axTouch.SetValue(OVRInput.Get(OVRInput.Touch.One, this.m_controller));
		this.m_by.SetValue(OVRInput.Get(OVRInput.Button.Two, this.m_controller));
		this.m_byTouch.SetValue(OVRInput.Get(OVRInput.Touch.Two, this.m_controller));
		this.m_indexTouch.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, this.m_controller));
		this.m_thumbstick.SetValue(OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, this.m_controller), OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller));
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x00065DC7 File Offset: 0x00063FC7
	private static string ToDeviceModel()
	{
		return "Touch";
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x00065DCE File Offset: 0x00063FCE
	private static string ToHandednessString(OVRInput.Handedness handedness)
	{
		if (handedness == OVRInput.Handedness.LeftHanded)
		{
			return "L";
		}
		if (handedness != OVRInput.Handedness.RightHanded)
		{
			return "-";
		}
		return "R";
	}

	// Token: 0x04001736 RID: 5942
	[Header("Settings")]
	[SerializeField]
	private OVRInput.Handedness m_handedness = OVRInput.Handedness.LeftHanded;

	// Token: 0x04001737 RID: 5943
	[Header("Left Column Components")]
	[SerializeField]
	private TextMeshProUGUI m_title;

	// Token: 0x04001738 RID: 5944
	[SerializeField]
	private TextMeshProUGUI m_status;

	// Token: 0x04001739 RID: 5945
	[SerializeField]
	private UiBoolInspector m_thumbRestTouch;

	// Token: 0x0400173A RID: 5946
	[SerializeField]
	private UiAxis1dInspector m_thumbRestForce;

	// Token: 0x0400173B RID: 5947
	[SerializeField]
	private UiAxis1dInspector m_indexTrigger;

	// Token: 0x0400173C RID: 5948
	[SerializeField]
	private UiAxis1dInspector m_gripTrigger;

	// Token: 0x0400173D RID: 5949
	[SerializeField]
	private UiAxis1dInspector m_stylusTipForce;

	// Token: 0x0400173E RID: 5950
	[SerializeField]
	private UiAxis1dInspector m_indexCurl1d;

	// Token: 0x0400173F RID: 5951
	[SerializeField]
	private UiAxis1dInspector m_indexSlider1d;

	// Token: 0x04001740 RID: 5952
	[Header("Right Column Components")]
	[SerializeField]
	private UiBoolInspector m_ax;

	// Token: 0x04001741 RID: 5953
	[SerializeField]
	private UiBoolInspector m_axTouch;

	// Token: 0x04001742 RID: 5954
	[SerializeField]
	private UiBoolInspector m_by;

	// Token: 0x04001743 RID: 5955
	[SerializeField]
	private UiBoolInspector m_byTouch;

	// Token: 0x04001744 RID: 5956
	[SerializeField]
	private UiBoolInspector m_indexTouch;

	// Token: 0x04001745 RID: 5957
	[SerializeField]
	private UiAxis2dInspector m_thumbstick;

	// Token: 0x04001746 RID: 5958
	private OVRInput.Controller m_controller;
}

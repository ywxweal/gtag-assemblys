using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020003DB RID: 987
[Serializable]
public class VRMapThumb : VRMap
{
	// Token: 0x060017C6 RID: 6086 RVA: 0x00073F7C File Offset: 0x0007217C
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		if (this.vrTargetNode == XRNode.LeftHand)
		{
			this.primaryButtonPress = ControllerInputPoller.instance.leftControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.leftControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		else
		{
			this.primaryButtonPress = ControllerInputPoller.instance.rightControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.rightControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
		}
		if (this.primaryButtonPress || this.secondaryButtonPress)
		{
			this.calcT = 1f;
		}
		else if (this.primaryButtonTouch || this.secondaryButtonTouch)
		{
			this.calcT = 0.1f;
		}
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x00074070 File Offset: 0x00072270
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.myTempInt = (int)(this.currentAngle1 * 10.1f);
			if (this.myTempInt != this.lastAngle1)
			{
				this.lastAngle1 = this.myTempInt;
				this.fingerBone1.localRotation = this.angle1Table[this.lastAngle1];
			}
			this.myTempInt = (int)(this.currentAngle2 * 10.1f);
			if (this.myTempInt != this.lastAngle2)
			{
				this.lastAngle2 = this.myTempInt;
				this.fingerBone2.localRotation = this.angle2Table[this.lastAngle2];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle1), Quaternion.Euler(this.closedAngle1), this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle2), Quaternion.Euler(this.closedAngle2), this.calcT), lerpValue);
		}
	}

	// Token: 0x04001A8E RID: 6798
	public InputFeatureUsage inputAxis;

	// Token: 0x04001A8F RID: 6799
	public bool primaryButtonTouch;

	// Token: 0x04001A90 RID: 6800
	public bool primaryButtonPress;

	// Token: 0x04001A91 RID: 6801
	public bool secondaryButtonTouch;

	// Token: 0x04001A92 RID: 6802
	public bool secondaryButtonPress;

	// Token: 0x04001A93 RID: 6803
	public Transform fingerBone1;

	// Token: 0x04001A94 RID: 6804
	public Transform fingerBone2;

	// Token: 0x04001A95 RID: 6805
	public Vector3 closedAngle1;

	// Token: 0x04001A96 RID: 6806
	public Vector3 closedAngle2;

	// Token: 0x04001A97 RID: 6807
	public Vector3 startingAngle1;

	// Token: 0x04001A98 RID: 6808
	public Vector3 startingAngle2;

	// Token: 0x04001A99 RID: 6809
	public Quaternion[] angle1Table;

	// Token: 0x04001A9A RID: 6810
	public Quaternion[] angle2Table;

	// Token: 0x04001A9B RID: 6811
	private float currentAngle1;

	// Token: 0x04001A9C RID: 6812
	private float currentAngle2;

	// Token: 0x04001A9D RID: 6813
	private int lastAngle1;

	// Token: 0x04001A9E RID: 6814
	private int lastAngle2;

	// Token: 0x04001A9F RID: 6815
	private InputDevice tempDevice;

	// Token: 0x04001AA0 RID: 6816
	private int myTempInt;
}

using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020003D9 RID: 985
[Serializable]
public class VRMapIndex : VRMap
{
	// Token: 0x060017C0 RID: 6080 RVA: 0x00073AC4 File Offset: 0x00071CC4
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.triggerValue = ControllerInputPoller.TriggerFloat(this.vrTargetNode);
		this.triggerTouch = ControllerInputPoller.TriggerTouch(this.vrTargetNode);
		this.calcT = 0.1f * this.triggerTouch;
		this.calcT += 0.9f * this.triggerValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x00073B34 File Offset: 0x00071D34
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.currentAngle3 = Mathf.Lerp(this.currentAngle3, this.calcT, lerpValue);
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
			}
			this.myTempInt = (int)(this.currentAngle3 * 10.1f);
			if (this.myTempInt != this.lastAngle3)
			{
				this.lastAngle3 = this.myTempInt;
				this.fingerBone3.localRotation = this.angle3Table[this.lastAngle3];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle1), Quaternion.Euler(this.closedAngle1), this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle2), Quaternion.Euler(this.closedAngle2), this.calcT), lerpValue);
			this.fingerBone3.localRotation = Quaternion.Lerp(this.fingerBone3.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle3), Quaternion.Euler(this.closedAngle3), this.calcT), lerpValue);
		}
	}

	// Token: 0x04001A5F RID: 6751
	public InputFeatureUsage inputAxis;

	// Token: 0x04001A60 RID: 6752
	public float triggerTouch;

	// Token: 0x04001A61 RID: 6753
	public float triggerValue;

	// Token: 0x04001A62 RID: 6754
	public Transform fingerBone1;

	// Token: 0x04001A63 RID: 6755
	public Transform fingerBone2;

	// Token: 0x04001A64 RID: 6756
	public Transform fingerBone3;

	// Token: 0x04001A65 RID: 6757
	public float closedAngles;

	// Token: 0x04001A66 RID: 6758
	public Vector3 closedAngle1;

	// Token: 0x04001A67 RID: 6759
	public Vector3 closedAngle2;

	// Token: 0x04001A68 RID: 6760
	public Vector3 closedAngle3;

	// Token: 0x04001A69 RID: 6761
	public Vector3 startingAngle1;

	// Token: 0x04001A6A RID: 6762
	public Vector3 startingAngle2;

	// Token: 0x04001A6B RID: 6763
	public Vector3 startingAngle3;

	// Token: 0x04001A6C RID: 6764
	private int lastAngle1;

	// Token: 0x04001A6D RID: 6765
	private int lastAngle2;

	// Token: 0x04001A6E RID: 6766
	private int lastAngle3;

	// Token: 0x04001A6F RID: 6767
	private InputDevice myInputDevice;

	// Token: 0x04001A70 RID: 6768
	public Quaternion[] angle1Table;

	// Token: 0x04001A71 RID: 6769
	public Quaternion[] angle2Table;

	// Token: 0x04001A72 RID: 6770
	public Quaternion[] angle3Table;

	// Token: 0x04001A73 RID: 6771
	private float currentAngle1;

	// Token: 0x04001A74 RID: 6772
	private float currentAngle2;

	// Token: 0x04001A75 RID: 6773
	private float currentAngle3;

	// Token: 0x04001A76 RID: 6774
	private int myTempInt;
}

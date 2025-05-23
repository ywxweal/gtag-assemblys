using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020003DA RID: 986
[Serializable]
public class VRMapMiddle : VRMap
{
	// Token: 0x060017C3 RID: 6083 RVA: 0x00073D4D File Offset: 0x00071F4D
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.gripValue = ControllerInputPoller.GripFloat(this.vrTargetNode);
		this.calcT = 1f * this.gripValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x00073D88 File Offset: 0x00071F88
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

	// Token: 0x04001A77 RID: 6775
	public InputFeatureUsage inputAxis;

	// Token: 0x04001A78 RID: 6776
	public float gripValue;

	// Token: 0x04001A79 RID: 6777
	public Transform fingerBone1;

	// Token: 0x04001A7A RID: 6778
	public Transform fingerBone2;

	// Token: 0x04001A7B RID: 6779
	public Transform fingerBone3;

	// Token: 0x04001A7C RID: 6780
	public float closedAngles;

	// Token: 0x04001A7D RID: 6781
	public Vector3 closedAngle1;

	// Token: 0x04001A7E RID: 6782
	public Vector3 closedAngle2;

	// Token: 0x04001A7F RID: 6783
	public Vector3 closedAngle3;

	// Token: 0x04001A80 RID: 6784
	public Vector3 startingAngle1;

	// Token: 0x04001A81 RID: 6785
	public Vector3 startingAngle2;

	// Token: 0x04001A82 RID: 6786
	public Vector3 startingAngle3;

	// Token: 0x04001A83 RID: 6787
	public Quaternion[] angle1Table;

	// Token: 0x04001A84 RID: 6788
	public Quaternion[] angle2Table;

	// Token: 0x04001A85 RID: 6789
	public Quaternion[] angle3Table;

	// Token: 0x04001A86 RID: 6790
	private int lastAngle1;

	// Token: 0x04001A87 RID: 6791
	private int lastAngle2;

	// Token: 0x04001A88 RID: 6792
	private int lastAngle3;

	// Token: 0x04001A89 RID: 6793
	private float currentAngle1;

	// Token: 0x04001A8A RID: 6794
	private float currentAngle2;

	// Token: 0x04001A8B RID: 6795
	private float currentAngle3;

	// Token: 0x04001A8C RID: 6796
	private InputDevice tempDevice;

	// Token: 0x04001A8D RID: 6797
	private int myTempInt;
}

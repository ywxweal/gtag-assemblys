using System;
using UnityEngine;

// Token: 0x0200040A RID: 1034
public class ManipulatableLever : ManipulatableObject
{
	// Token: 0x06001929 RID: 6441 RVA: 0x00079E93 File Offset: 0x00078093
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x00079EA8 File Offset: 0x000780A8
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = this.leverGrip.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x00079EE8 File Offset: 0x000780E8
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 position = hand.transform.position;
		Vector3 vector = Vector3.Normalize(this.localSpace.MultiplyPoint3x4(position) - base.transform.localPosition);
		Vector3 eulerAngles = Quaternion.LookRotation(Vector3.forward, vector).eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		eulerAngles.z = Mathf.Clamp(eulerAngles.z, this.minAngle, this.maxAngle);
		base.transform.localEulerAngles = eulerAngles;
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x00079FA0 File Offset: 0x000781A0
	public void SetValue(float value)
	{
		float num = Mathf.Lerp(this.minAngle, this.maxAngle, value);
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.z = num;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x00079FE0 File Offset: 0x000781E0
	public void SetNotch(int notchValue)
	{
		if (this.notches == null)
		{
			return;
		}
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (leverNotch.value == notchValue)
			{
				this.SetValue(Mathf.Lerp(leverNotch.minAngleValue, leverNotch.maxAngleValue, 0.5f));
				return;
			}
		}
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x0007A038 File Offset: 0x00078238
	public float GetValue()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.z > 180f)
		{
			localEulerAngles.z -= 360f;
		}
		else if (localEulerAngles.z < -180f)
		{
			localEulerAngles.z += 360f;
		}
		return Mathf.InverseLerp(this.minAngle, this.maxAngle, localEulerAngles.z);
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x0007A0A4 File Offset: 0x000782A4
	public int GetNotch()
	{
		if (this.notches == null)
		{
			return 0;
		}
		float value = this.GetValue();
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (value >= leverNotch.minAngleValue && value <= leverNotch.maxAngleValue)
			{
				return leverNotch.value;
			}
		}
		return 0;
	}

	// Token: 0x04001BFB RID: 7163
	[SerializeField]
	private float breakDistance = 0.2f;

	// Token: 0x04001BFC RID: 7164
	[SerializeField]
	private Transform leverGrip;

	// Token: 0x04001BFD RID: 7165
	[SerializeField]
	private float maxAngle = 22.5f;

	// Token: 0x04001BFE RID: 7166
	[SerializeField]
	private float minAngle = -22.5f;

	// Token: 0x04001BFF RID: 7167
	[SerializeField]
	private ManipulatableLever.LeverNotch[] notches;

	// Token: 0x04001C00 RID: 7168
	private Matrix4x4 localSpace;

	// Token: 0x0200040B RID: 1035
	[Serializable]
	public class LeverNotch
	{
		// Token: 0x04001C01 RID: 7169
		public float minAngleValue;

		// Token: 0x04001C02 RID: 7170
		public float maxAngleValue;

		// Token: 0x04001C03 RID: 7171
		public int value;
	}
}

using System;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class HandMeshUI : MonoBehaviour
{
	// Token: 0x0600137D RID: 4989 RVA: 0x0005DEE4 File Offset: 0x0005C0E4
	private void Start()
	{
		this.SetSliderValue(0, (float)this.rightMask.radialDivisions, false);
		this.SetSliderValue(1, this.rightMask.borderSize, false);
		this.SetSliderValue(2, this.rightMask.fingerTaper, false);
		this.SetSliderValue(3, this.rightMask.fingerTipLength, false);
		this.SetSliderValue(4, this.rightMask.webOffset, false);
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x0005DF54 File Offset: 0x0005C154
	private void Update()
	{
		this.CheckForHands();
		Vector3 position = this.rightHand.Bones[20].Transform.position;
		Vector3 position2 = this.leftHand.Bones[20].Transform.position;
		if (this.rightHeldKnob >= 0)
		{
			Vector3 vector = this.knobs[this.rightHeldKnob].transform.parent.InverseTransformPoint(position);
			this.SetSliderValue(this.rightHeldKnob, Mathf.Clamp01(vector.x * 10f), true);
			if (vector.z < -0.02f)
			{
				this.rightHeldKnob = -1;
			}
		}
		else
		{
			for (int i = 0; i < this.knobs.Length; i++)
			{
				if (Vector3.Distance(position, this.knobs[i].transform.position) <= 0.02f && this.leftHeldKnob != i)
				{
					this.rightHeldKnob = i;
					break;
				}
			}
		}
		if (this.leftHeldKnob >= 0)
		{
			Vector3 vector2 = this.knobs[this.leftHeldKnob].transform.parent.InverseTransformPoint(position2);
			this.SetSliderValue(this.leftHeldKnob, Mathf.Clamp01(vector2.x * 10f), true);
			if (vector2.z < -0.02f)
			{
				this.leftHeldKnob = -1;
				return;
			}
		}
		else
		{
			for (int j = 0; j < this.knobs.Length; j++)
			{
				if (Vector3.Distance(position2, this.knobs[j].transform.position) <= 0.02f && this.rightHeldKnob != j)
				{
					this.leftHeldKnob = j;
					return;
				}
			}
		}
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x0005E0E8 File Offset: 0x0005C2E8
	private void SetSliderValue(int sliderID, float value, bool isNormalized)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = 0.1f;
		string text = "";
		switch (sliderID)
		{
		case 0:
			num = 2f;
			num2 = 16f;
			text = "{0, 0:0}";
			break;
		case 1:
			num = 0f;
			num2 = 0.05f;
			text = "{0, 0:0.000}";
			break;
		case 2:
			num = 0f;
			num2 = 0.3333f;
			text = "{0, 0:0.00}";
			break;
		case 3:
			num = 0.5f;
			num2 = 1.5f;
			text = "{0, 0:0.00}";
			break;
		case 4:
			num = 0f;
			num2 = 1f;
			text = "{0, 0:0.00}";
			break;
		}
		float num4 = (isNormalized ? (value * (num2 - num) + num) : value);
		float num5 = (isNormalized ? value : ((value - num) / (num2 - num)));
		this.knobs[sliderID].transform.localPosition = Vector3.right * num5 * num3;
		this.readouts[sliderID].text = string.Format(text, num4);
		switch (sliderID)
		{
		case 0:
			this.rightMask.radialDivisions = (int)num4;
			this.leftMask.radialDivisions = (int)num4;
			return;
		case 1:
			this.rightMask.borderSize = num4;
			this.leftMask.borderSize = num4;
			return;
		case 2:
			this.rightMask.fingerTaper = num4;
			this.leftMask.fingerTaper = num4;
			return;
		case 3:
			this.rightMask.fingerTipLength = num4;
			this.leftMask.fingerTipLength = num4;
			return;
		case 4:
			this.rightMask.webOffset = num4;
			this.leftMask.webOffset = num4;
			return;
		default:
			return;
		}
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x0005E28C File Offset: 0x0005C48C
	private void CheckForHands()
	{
		bool flag = OVRInput.GetActiveController() == OVRInput.Controller.Hands || OVRInput.GetActiveController() == OVRInput.Controller.LHand || OVRInput.GetActiveController() == OVRInput.Controller.RHand;
		if (base.transform.GetChild(0).gameObject.activeSelf)
		{
			if (!flag)
			{
				base.transform.GetChild(0).gameObject.SetActive(false);
				this.leftHeldKnob = -1;
				this.rightHeldKnob = -1;
				return;
			}
		}
		else if (flag)
		{
			base.transform.GetChild(0).gameObject.SetActive(true);
			base.transform.position = (this.rightHand.Bones[20].Transform.position + this.rightHand.Bones[20].Transform.position) * 0.5f;
			base.transform.position += (base.transform.position - Camera.main.transform.position).normalized * 0.1f;
			base.transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z));
		}
	}

	// Token: 0x040015A6 RID: 5542
	public SphereCollider[] knobs;

	// Token: 0x040015A7 RID: 5543
	public TextMesh[] readouts;

	// Token: 0x040015A8 RID: 5544
	private int rightHeldKnob = -1;

	// Token: 0x040015A9 RID: 5545
	private int leftHeldKnob = -1;

	// Token: 0x040015AA RID: 5546
	public OVRSkeleton leftHand;

	// Token: 0x040015AB RID: 5547
	public OVRSkeleton rightHand;

	// Token: 0x040015AC RID: 5548
	public HandMeshMask leftMask;

	// Token: 0x040015AD RID: 5549
	public HandMeshMask rightMask;
}

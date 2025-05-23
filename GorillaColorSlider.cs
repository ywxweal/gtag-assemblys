using System;
using UnityEngine;

// Token: 0x020006D5 RID: 1749
public class GorillaColorSlider : MonoBehaviour
{
	// Token: 0x06002B92 RID: 11154 RVA: 0x000D6F59 File Offset: 0x000D5159
	private void Start()
	{
		if (!this.setRandomly)
		{
			this.startingLocation = base.transform.position;
		}
	}

	// Token: 0x06002B93 RID: 11155 RVA: 0x000D6F74 File Offset: 0x000D5174
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float num3 = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(num3, this.startingLocation.y, this.startingLocation.z);
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x000D7014 File Offset: 0x000D5214
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x000D7070 File Offset: 0x000D5270
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
			else
			{
				base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
		}
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x0400318F RID: 12687
	public bool setRandomly;

	// Token: 0x04003190 RID: 12688
	public float zRange;

	// Token: 0x04003191 RID: 12689
	public float maxValue;

	// Token: 0x04003192 RID: 12690
	public float minValue;

	// Token: 0x04003193 RID: 12691
	public Vector3 startingLocation;

	// Token: 0x04003194 RID: 12692
	public int valueIndex;

	// Token: 0x04003195 RID: 12693
	public float valueImReporting;

	// Token: 0x04003196 RID: 12694
	public GorillaTriggerBox gorilla;

	// Token: 0x04003197 RID: 12695
	private float startingZ;
}

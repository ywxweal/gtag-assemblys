using System;
using UnityEngine;

// Token: 0x020006EC RID: 1772
public class GorillaTurnSlider : MonoBehaviour
{
	// Token: 0x06002C22 RID: 11298 RVA: 0x000D9945 File Offset: 0x000D7B45
	private void Awake()
	{
		this.startingLocation = base.transform.position;
		this.SetPosition(this.gorillaTurn.currentSpeed);
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x000023F4 File Offset: 0x000005F4
	private void FixedUpdate()
	{
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x000D996C File Offset: 0x000D7B6C
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float num3 = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(num3, this.startingLocation.y, this.startingLocation.z);
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x000D99F0 File Offset: 0x000D7BF0
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x000D9A4C File Offset: 0x000D7C4C
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
				return;
			}
			base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
		}
	}

	// Token: 0x04003256 RID: 12886
	public float zRange;

	// Token: 0x04003257 RID: 12887
	public float maxValue;

	// Token: 0x04003258 RID: 12888
	public float minValue;

	// Token: 0x04003259 RID: 12889
	public GorillaTurning gorillaTurn;

	// Token: 0x0400325A RID: 12890
	private float startingZ;

	// Token: 0x0400325B RID: 12891
	public Vector3 startingLocation;
}

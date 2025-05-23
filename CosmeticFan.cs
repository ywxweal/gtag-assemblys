using System;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class CosmeticFan : MonoBehaviour
{
	// Token: 0x060008FB RID: 2299 RVA: 0x00030AAF File Offset: 0x0002ECAF
	private void Start()
	{
		this.spinUpRate = this.maxSpeed / this.spinUpDuration;
		this.spinDownRate = this.maxSpeed / this.spinDownDuration;
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00030AD8 File Offset: 0x0002ECD8
	public void Run()
	{
		this.targetSpeed = this.maxSpeed;
		if (this.spinUpDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinUpRate;
		}
		else
		{
			this.currentSpeed = this.maxSpeed;
		}
		base.enabled = true;
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00030B26 File Offset: 0x0002ED26
	public void Stop()
	{
		this.targetSpeed = 0f;
		if (this.spinDownDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinDownRate;
			return;
		}
		this.currentSpeed = 0f;
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00030B5F File Offset: 0x0002ED5F
	public void InstantStop()
	{
		this.targetSpeed = 0f;
		this.currentSpeed = 0f;
		base.enabled = false;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00030B80 File Offset: 0x0002ED80
	private void Update()
	{
		this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.targetSpeed, this.currentAccelRate * Time.deltaTime);
		base.transform.localRotation = base.transform.localRotation * Quaternion.AngleAxis(this.currentSpeed * Time.deltaTime, this.axis);
		if (this.currentSpeed == 0f && this.targetSpeed == 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04000AA8 RID: 2728
	[SerializeField]
	private Vector3 axis;

	// Token: 0x04000AA9 RID: 2729
	[SerializeField]
	private float spinUpDuration = 0.3f;

	// Token: 0x04000AAA RID: 2730
	[SerializeField]
	private float spinDownDuration = 0.3f;

	// Token: 0x04000AAB RID: 2731
	[SerializeField]
	private float maxSpeed = 360f;

	// Token: 0x04000AAC RID: 2732
	private float currentSpeed;

	// Token: 0x04000AAD RID: 2733
	private float targetSpeed;

	// Token: 0x04000AAE RID: 2734
	private float currentAccelRate;

	// Token: 0x04000AAF RID: 2735
	private float spinUpRate;

	// Token: 0x04000AB0 RID: 2736
	private float spinDownRate;
}

using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class LocalActivateOnDateRange : MonoBehaviour
{
	// Token: 0x06000B55 RID: 2901 RVA: 0x0003CA60 File Offset: 0x0003AC60
	private void Awake()
	{
		GameObject[] array = this.gameObjectsToActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x0003CA8B File Offset: 0x0003AC8B
	private void OnEnable()
	{
		this.InitActiveTimes();
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0003CA94 File Offset: 0x0003AC94
	private void InitActiveTimes()
	{
		this.activationTime = new DateTime(this.activationYear, this.activationMonth, this.activationDay, this.activationHour, this.activationMinute, this.activationSecond, DateTimeKind.Utc);
		this.deactivationTime = new DateTime(this.deactivationYear, this.deactivationMonth, this.deactivationDay, this.deactivationHour, this.deactivationMinute, this.deactivationSecond, DateTimeKind.Utc);
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0003CB04 File Offset: 0x0003AD04
	private void LateUpdate()
	{
		DateTime utcNow = DateTime.UtcNow;
		this.dbgTimeUntilActivation = (this.activationTime - utcNow).TotalSeconds;
		this.dbgTimeUntilDeactivation = (this.deactivationTime - utcNow).TotalSeconds;
		bool flag = utcNow >= this.activationTime && utcNow <= this.deactivationTime;
		if (flag != this.isActive)
		{
			GameObject[] array = this.gameObjectsToActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			this.isActive = flag;
		}
	}

	// Token: 0x04000DE7 RID: 3559
	[Header("Activation Date and Time (UTC)")]
	public int activationYear = 2023;

	// Token: 0x04000DE8 RID: 3560
	public int activationMonth = 4;

	// Token: 0x04000DE9 RID: 3561
	public int activationDay = 1;

	// Token: 0x04000DEA RID: 3562
	public int activationHour = 7;

	// Token: 0x04000DEB RID: 3563
	public int activationMinute;

	// Token: 0x04000DEC RID: 3564
	public int activationSecond;

	// Token: 0x04000DED RID: 3565
	[Header("Deactivation Date and Time (UTC)")]
	public int deactivationYear = 2023;

	// Token: 0x04000DEE RID: 3566
	public int deactivationMonth = 4;

	// Token: 0x04000DEF RID: 3567
	public int deactivationDay = 2;

	// Token: 0x04000DF0 RID: 3568
	public int deactivationHour = 7;

	// Token: 0x04000DF1 RID: 3569
	public int deactivationMinute;

	// Token: 0x04000DF2 RID: 3570
	public int deactivationSecond;

	// Token: 0x04000DF3 RID: 3571
	public GameObject[] gameObjectsToActivate;

	// Token: 0x04000DF4 RID: 3572
	private bool isActive;

	// Token: 0x04000DF5 RID: 3573
	private DateTime activationTime;

	// Token: 0x04000DF6 RID: 3574
	private DateTime deactivationTime;

	// Token: 0x04000DF7 RID: 3575
	[DebugReadout]
	public double dbgTimeUntilActivation;

	// Token: 0x04000DF8 RID: 3576
	[DebugReadout]
	public double dbgTimeUntilDeactivation;
}

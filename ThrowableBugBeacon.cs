using System;
using UnityEngine;

// Token: 0x02000A0C RID: 2572
public class ThrowableBugBeacon : MonoBehaviour
{
	// Token: 0x14000073 RID: 115
	// (add) Token: 0x06003D69 RID: 15721 RVA: 0x00123F60 File Offset: 0x00122160
	// (remove) Token: 0x06003D6A RID: 15722 RVA: 0x00123F94 File Offset: 0x00122194
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall;

	// Token: 0x14000074 RID: 116
	// (add) Token: 0x06003D6B RID: 15723 RVA: 0x00123FC8 File Offset: 0x001221C8
	// (remove) Token: 0x06003D6C RID: 15724 RVA: 0x00123FFC File Offset: 0x001221FC
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss;

	// Token: 0x14000075 RID: 117
	// (add) Token: 0x06003D6D RID: 15725 RVA: 0x00124030 File Offset: 0x00122230
	// (remove) Token: 0x06003D6E RID: 15726 RVA: 0x00124064 File Offset: 0x00122264
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock;

	// Token: 0x14000076 RID: 118
	// (add) Token: 0x06003D6F RID: 15727 RVA: 0x00124098 File Offset: 0x00122298
	// (remove) Token: 0x06003D70 RID: 15728 RVA: 0x001240CC File Offset: 0x001222CC
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock;

	// Token: 0x14000077 RID: 119
	// (add) Token: 0x06003D71 RID: 15729 RVA: 0x00124100 File Offset: 0x00122300
	// (remove) Token: 0x06003D72 RID: 15730 RVA: 0x00124134 File Offset: 0x00122334
	public static event ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	// Token: 0x170005FF RID: 1535
	// (get) Token: 0x06003D73 RID: 15731 RVA: 0x00124167 File Offset: 0x00122367
	public ThrowableBug.BugName BugName
	{
		get
		{
			return this.bugName;
		}
	}

	// Token: 0x17000600 RID: 1536
	// (get) Token: 0x06003D74 RID: 15732 RVA: 0x0012416F File Offset: 0x0012236F
	public float Range
	{
		get
		{
			return this.range;
		}
	}

	// Token: 0x06003D75 RID: 15733 RVA: 0x00124177 File Offset: 0x00122377
	public void Call()
	{
		if (ThrowableBugBeacon.OnCall != null)
		{
			ThrowableBugBeacon.OnCall(this);
		}
	}

	// Token: 0x06003D76 RID: 15734 RVA: 0x0012418B File Offset: 0x0012238B
	public void Dismiss()
	{
		if (ThrowableBugBeacon.OnDismiss != null)
		{
			ThrowableBugBeacon.OnDismiss(this);
		}
	}

	// Token: 0x06003D77 RID: 15735 RVA: 0x0012419F File Offset: 0x0012239F
	public void Lock()
	{
		if (ThrowableBugBeacon.OnLock != null)
		{
			ThrowableBugBeacon.OnLock(this);
		}
	}

	// Token: 0x06003D78 RID: 15736 RVA: 0x001241B3 File Offset: 0x001223B3
	public void Unlock()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x06003D79 RID: 15737 RVA: 0x001241C7 File Offset: 0x001223C7
	public void ChangeSpeedMultiplier(float f)
	{
		if (ThrowableBugBeacon.OnChangeSpeedMultiplier != null)
		{
			ThrowableBugBeacon.OnChangeSpeedMultiplier(this, f);
		}
	}

	// Token: 0x06003D7A RID: 15738 RVA: 0x001241B3 File Offset: 0x001223B3
	private void OnDisable()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x0400415A RID: 16730
	[SerializeField]
	private float range;

	// Token: 0x0400415B RID: 16731
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x02000A0D RID: 2573
	// (Invoke) Token: 0x06003D7D RID: 15741
	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	// Token: 0x02000A0E RID: 2574
	// (Invoke) Token: 0x06003D81 RID: 15745
	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);
}

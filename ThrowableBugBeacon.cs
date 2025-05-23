using System;
using UnityEngine;

// Token: 0x02000A0C RID: 2572
public class ThrowableBugBeacon : MonoBehaviour
{
	// Token: 0x14000073 RID: 115
	// (add) Token: 0x06003D68 RID: 15720 RVA: 0x00123E88 File Offset: 0x00122088
	// (remove) Token: 0x06003D69 RID: 15721 RVA: 0x00123EBC File Offset: 0x001220BC
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall;

	// Token: 0x14000074 RID: 116
	// (add) Token: 0x06003D6A RID: 15722 RVA: 0x00123EF0 File Offset: 0x001220F0
	// (remove) Token: 0x06003D6B RID: 15723 RVA: 0x00123F24 File Offset: 0x00122124
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss;

	// Token: 0x14000075 RID: 117
	// (add) Token: 0x06003D6C RID: 15724 RVA: 0x00123F58 File Offset: 0x00122158
	// (remove) Token: 0x06003D6D RID: 15725 RVA: 0x00123F8C File Offset: 0x0012218C
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock;

	// Token: 0x14000076 RID: 118
	// (add) Token: 0x06003D6E RID: 15726 RVA: 0x00123FC0 File Offset: 0x001221C0
	// (remove) Token: 0x06003D6F RID: 15727 RVA: 0x00123FF4 File Offset: 0x001221F4
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock;

	// Token: 0x14000077 RID: 119
	// (add) Token: 0x06003D70 RID: 15728 RVA: 0x00124028 File Offset: 0x00122228
	// (remove) Token: 0x06003D71 RID: 15729 RVA: 0x0012405C File Offset: 0x0012225C
	public static event ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	// Token: 0x170005FF RID: 1535
	// (get) Token: 0x06003D72 RID: 15730 RVA: 0x0012408F File Offset: 0x0012228F
	public ThrowableBug.BugName BugName
	{
		get
		{
			return this.bugName;
		}
	}

	// Token: 0x17000600 RID: 1536
	// (get) Token: 0x06003D73 RID: 15731 RVA: 0x00124097 File Offset: 0x00122297
	public float Range
	{
		get
		{
			return this.range;
		}
	}

	// Token: 0x06003D74 RID: 15732 RVA: 0x0012409F File Offset: 0x0012229F
	public void Call()
	{
		if (ThrowableBugBeacon.OnCall != null)
		{
			ThrowableBugBeacon.OnCall(this);
		}
	}

	// Token: 0x06003D75 RID: 15733 RVA: 0x001240B3 File Offset: 0x001222B3
	public void Dismiss()
	{
		if (ThrowableBugBeacon.OnDismiss != null)
		{
			ThrowableBugBeacon.OnDismiss(this);
		}
	}

	// Token: 0x06003D76 RID: 15734 RVA: 0x001240C7 File Offset: 0x001222C7
	public void Lock()
	{
		if (ThrowableBugBeacon.OnLock != null)
		{
			ThrowableBugBeacon.OnLock(this);
		}
	}

	// Token: 0x06003D77 RID: 15735 RVA: 0x001240DB File Offset: 0x001222DB
	public void Unlock()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x06003D78 RID: 15736 RVA: 0x001240EF File Offset: 0x001222EF
	public void ChangeSpeedMultiplier(float f)
	{
		if (ThrowableBugBeacon.OnChangeSpeedMultiplier != null)
		{
			ThrowableBugBeacon.OnChangeSpeedMultiplier(this, f);
		}
	}

	// Token: 0x06003D79 RID: 15737 RVA: 0x001240DB File Offset: 0x001222DB
	private void OnDisable()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	// Token: 0x04004159 RID: 16729
	[SerializeField]
	private float range;

	// Token: 0x0400415A RID: 16730
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x02000A0D RID: 2573
	// (Invoke) Token: 0x06003D7C RID: 15740
	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	// Token: 0x02000A0E RID: 2574
	// (Invoke) Token: 0x06003D80 RID: 15744
	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);
}

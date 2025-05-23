using System;
using UnityEngine;

// Token: 0x020003C8 RID: 968
public class RotationAnimation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06001699 RID: 5785 RVA: 0x0006CD9C File Offset: 0x0006AF9C
	// (set) Token: 0x0600169A RID: 5786 RVA: 0x0006CDA4 File Offset: 0x0006AFA4
	public bool TickRunning { get; set; }

	// Token: 0x0600169B RID: 5787 RVA: 0x0006CDB0 File Offset: 0x0006AFB0
	public void Tick()
	{
		Vector3 vector = Vector3.zero;
		vector.x = this.amplitude.x * this.x.Evaluate((Time.time - this.baseTime) * this.period.x % 1f);
		vector.y = this.amplitude.y * this.y.Evaluate((Time.time - this.baseTime) * this.period.y % 1f);
		vector.z = this.amplitude.z * this.z.Evaluate((Time.time - this.baseTime) * this.period.z % 1f);
		if (this.releaseSet)
		{
			float num = this.release.Evaluate(Time.time - this.releaseTime);
			vector *= num;
			if (num < Mathf.Epsilon)
			{
				base.enabled = false;
			}
		}
		base.transform.localRotation = Quaternion.Euler(vector) * this.baseRotation;
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x0006CECA File Offset: 0x0006B0CA
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x0006CEDD File Offset: 0x0006B0DD
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.releaseSet = false;
		this.baseTime = Time.time;
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x0006CEF7 File Offset: 0x0006B0F7
	public void ReleaseToDisable()
	{
		this.releaseSet = true;
		this.releaseTime = Time.time;
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x0006CF0B File Offset: 0x0006B10B
	public void CancelRelease()
	{
		this.releaseSet = false;
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x0006CF14 File Offset: 0x0006B114
	private void OnDisable()
	{
		base.transform.localRotation = this.baseRotation;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x040018EE RID: 6382
	[SerializeField]
	private AnimationCurve x;

	// Token: 0x040018EF RID: 6383
	[SerializeField]
	private AnimationCurve y;

	// Token: 0x040018F0 RID: 6384
	[SerializeField]
	private AnimationCurve z;

	// Token: 0x040018F1 RID: 6385
	[SerializeField]
	private AnimationCurve attack;

	// Token: 0x040018F2 RID: 6386
	[SerializeField]
	private AnimationCurve release;

	// Token: 0x040018F3 RID: 6387
	[SerializeField]
	private Vector3 amplitude = Vector3.one;

	// Token: 0x040018F4 RID: 6388
	[SerializeField]
	private Vector3 period = Vector3.one;

	// Token: 0x040018F5 RID: 6389
	private Quaternion baseRotation;

	// Token: 0x040018F6 RID: 6390
	private float baseTime;

	// Token: 0x040018F7 RID: 6391
	private float releaseTime;

	// Token: 0x040018F8 RID: 6392
	private bool releaseSet;
}

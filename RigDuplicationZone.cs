using System;
using UnityEngine;

// Token: 0x020003C6 RID: 966
public class RigDuplicationZone : MonoBehaviour
{
	// Token: 0x14000046 RID: 70
	// (add) Token: 0x0600168A RID: 5770 RVA: 0x0006CBF4 File Offset: 0x0006ADF4
	// (remove) Token: 0x0600168B RID: 5771 RVA: 0x0006CC28 File Offset: 0x0006AE28
	public static event RigDuplicationZone.RigDuplicationZoneAction OnEnabled;

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x0600168C RID: 5772 RVA: 0x0006CC5B File Offset: 0x0006AE5B
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x0006CC63 File Offset: 0x0006AE63
	private void OnEnable()
	{
		RigDuplicationZone.OnEnabled += this.RigDuplicationZone_OnEnabled;
		if (RigDuplicationZone.OnEnabled != null)
		{
			RigDuplicationZone.OnEnabled(this);
		}
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x0006CC88 File Offset: 0x0006AE88
	private void OnDisable()
	{
		RigDuplicationZone.OnEnabled -= this.RigDuplicationZone_OnEnabled;
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x0006CC9B File Offset: 0x0006AE9B
	private void RigDuplicationZone_OnEnabled(RigDuplicationZone z)
	{
		if (z == this)
		{
			return;
		}
		if (z.id != this.id)
		{
			return;
		}
		this.setOtherZone(z);
		z.setOtherZone(this);
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x0006CCC9 File Offset: 0x0006AEC9
	private void setOtherZone(RigDuplicationZone z)
	{
		this.otherZone = z;
		this.offsetToOtherZone = z.transform.position - base.transform.position;
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x0006CCF4 File Offset: 0x0006AEF4
	private void OnTriggerEnter(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = true;
			return;
		}
		component.SetDuplicationZone(this);
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x0006CD2C File Offset: 0x0006AF2C
	private void OnTriggerExit(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = false;
			return;
		}
		component.ClearDuplicationZone(this);
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x06001693 RID: 5779 RVA: 0x0006CD61 File Offset: 0x0006AF61
	public Vector3 VisualOffsetForRigs
	{
		get
		{
			if (!this.otherZone.playerInZone)
			{
				return Vector3.zero;
			}
			return this.offsetToOtherZone;
		}
	}

	// Token: 0x040018EA RID: 6378
	private RigDuplicationZone otherZone;

	// Token: 0x040018EB RID: 6379
	[SerializeField]
	private string id;

	// Token: 0x040018EC RID: 6380
	private bool playerInZone;

	// Token: 0x040018ED RID: 6381
	private Vector3 offsetToOtherZone;

	// Token: 0x020003C7 RID: 967
	// (Invoke) Token: 0x06001696 RID: 5782
	public delegate void RigDuplicationZoneAction(RigDuplicationZone z);
}

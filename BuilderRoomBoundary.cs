using System;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200050F RID: 1295
public class BuilderRoomBoundary : GorillaTriggerBox
{
	// Token: 0x06001F5A RID: 8026 RVA: 0x0009D59C File Offset: 0x0009B79C
	private void Awake()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter += this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit += this.OnExitedBoundary;
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x0009D610 File Offset: 0x0009B810
	private void OnDestroy()
	{
		foreach (SizeChangerTrigger sizeChangerTrigger in this.enableOnEnterTrigger)
		{
			sizeChangerTrigger.OnEnter -= this.OnEnteredBoundary;
		}
		this.disableOnExitTrigger.OnExit -= this.OnExitedBoundary;
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x0009D684 File Offset: 0x0009B884
	public void OnEnteredBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.rigRef.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (builderTable.isTableMutable)
		{
			this.rigRef.EnableBuilderResizeWatch(true);
		}
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x0009D700 File Offset: 0x0009B900
	public void OnExitedBoundary(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null || !this.rigRef.isOfflineVRRig)
		{
			return;
		}
		this.rigRef.EnableBuilderResizeWatch(false);
	}

	// Token: 0x04002334 RID: 9012
	[SerializeField]
	private List<SizeChangerTrigger> enableOnEnterTrigger;

	// Token: 0x04002335 RID: 9013
	[SerializeField]
	private SizeChangerTrigger disableOnExitTrigger;

	// Token: 0x04002336 RID: 9014
	private VRRig rigRef;
}

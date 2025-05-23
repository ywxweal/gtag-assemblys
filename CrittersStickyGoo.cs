using System;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class CrittersStickyGoo : CrittersActor
{
	// Token: 0x060002A7 RID: 679 RVA: 0x0001089F File Offset: 0x0000EA9F
	public override void Initialize()
	{
		base.Initialize();
		this.readyToDisable = false;
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x000108B0 File Offset: 0x0000EAB0
	public bool CanAffect(Vector3 position)
	{
		return (base.transform.position - position).magnitude < this.range;
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x000108DE File Offset: 0x0000EADE
	public void EffectApplied(CrittersPawn critter)
	{
		if (this.destroyOnApply)
		{
			this.readyToDisable = true;
		}
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, this.actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00010920 File Offset: 0x0000EB20
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (this.readyToDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return flag;
	}

	// Token: 0x04000321 RID: 801
	[Header("Sticky Goo")]
	public float range = 1f;

	// Token: 0x04000322 RID: 802
	public float slowModifier = 0.3f;

	// Token: 0x04000323 RID: 803
	public float slowDuration = 3f;

	// Token: 0x04000324 RID: 804
	public bool destroyOnApply = true;

	// Token: 0x04000325 RID: 805
	private bool readyToDisable;
}

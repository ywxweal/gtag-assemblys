using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000412 RID: 1042
[NetworkBehaviourWeaved(0)]
public abstract class NetworkHoldableObject : NetworkComponent, IHoldableObject
{
	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06001967 RID: 6503 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001968 RID: 6504
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06001969 RID: 6505
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x0600196A RID: 6506
	public abstract void DropItemCleanup();

	// Token: 0x0600196B RID: 6507 RVA: 0x0007B0AC File Offset: 0x000792AC
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void ReadDataFusion()
	{
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void WriteDataFusion()
	{
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x00013963 File Offset: 0x00011B63
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x0001396B File Offset: 0x00011B6B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x00013973 File Offset: 0x00011B73
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}

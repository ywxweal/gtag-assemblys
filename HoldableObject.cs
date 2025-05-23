using System;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public abstract class HoldableObject : MonoBehaviour, IHoldableObject
{
	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x060018EB RID: 6379 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060018EC RID: 6380
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x060018ED RID: 6381
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x060018EE RID: 6382
	public abstract void DropItemCleanup();

	// Token: 0x060018EF RID: 6383 RVA: 0x00078E64 File Offset: 0x00077064
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x00013963 File Offset: 0x00011B63
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x0001396B File Offset: 0x00011B6B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x00013973 File Offset: 0x00011B73
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}
}

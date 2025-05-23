using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020003E4 RID: 996
public class CosmeticBoundaryTrigger : GorillaTriggerBox
{
	// Token: 0x06001807 RID: 6151 RVA: 0x00075068 File Offset: 0x00073268
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		if (CosmeticBoundaryTrigger.sinceLastTryOnEvent.HasElapsed(0.5f, true))
		{
			GorillaTelemetry.PostShopEvent(this.rigRef, GTShopEventType.item_try_on, this.rigRef.tryOnSet.items);
		}
		this.rigRef.inTryOnRoom = true;
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x00075114 File Offset: 0x00073314
	public void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		this.rigRef.inTryOnRoom = false;
		if (this.rigRef.isOfflineVRRig)
		{
			this.rigRef.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
		}
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x04001AE7 RID: 6887
	public VRRig rigRef;

	// Token: 0x04001AE8 RID: 6888
	private static TimeSince sinceLastTryOnEvent = 0f;
}

using System;
using Fusion;
using UnityEngine;

// Token: 0x020005EC RID: 1516
[NetworkBehaviourWeaved(0)]
internal class VrrigReliableSerializer : GorillaWrappedSerializer
{
	// Token: 0x0600254E RID: 9550 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnBeforeDespawn()
	{
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000BA478 File Offset: 0x000B8678
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (wrappedInfo.punInfo.Sender != wrappedInfo.punInfo.photonView.Owner || wrappedInfo.punInfo.photonView.IsRoomView)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(wrappedInfo.Sender, out rigContainer))
		{
			outTargetObject = rigContainer.gameObject;
			outTargetType = typeof(VRRigReliableState);
			return true;
		}
		return false;
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x000B838D File Offset: 0x000B658D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000B8399 File Offset: 0x000B6599
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}

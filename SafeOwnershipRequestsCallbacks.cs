using System;
using UnityEngine;

// Token: 0x020005EA RID: 1514
public class SafeOwnershipRequestsCallbacks : MonoBehaviour, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06002522 RID: 9506 RVA: 0x000B985C File Offset: 0x000B7A5C
	private void Awake()
	{
		this._requestableOwnershipGuard.AddCallbackTarget(this);
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000023F4 File Offset: 0x000005F4
	void IRequestableOwnershipGuardCallbacks.OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
	}

	// Token: 0x06002524 RID: 9508 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000023F4 File Offset: 0x000005F4
	void IRequestableOwnershipGuardCallbacks.OnMyOwnerLeft()
	{
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x000023F4 File Offset: 0x000005F4
	void IRequestableOwnershipGuardCallbacks.OnMyCreatorLeft()
	{
	}

	// Token: 0x040029F0 RID: 10736
	[SerializeField]
	private RequestableOwnershipGuard _requestableOwnershipGuard;
}

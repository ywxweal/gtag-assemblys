using System;

// Token: 0x02000215 RID: 533
public interface IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06000C7A RID: 3194
	void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer);

	// Token: 0x06000C7B RID: 3195
	bool OnOwnershipRequest(NetPlayer fromPlayer);

	// Token: 0x06000C7C RID: 3196
	void OnMyOwnerLeft();

	// Token: 0x06000C7D RID: 3197
	bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer);

	// Token: 0x06000C7E RID: 3198
	void OnMyCreatorLeft();
}

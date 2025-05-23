using System;

// Token: 0x02000210 RID: 528
public enum NetworkingState
{
	// Token: 0x04000EE9 RID: 3817
	IsOwner,
	// Token: 0x04000EEA RID: 3818
	IsBlindClient,
	// Token: 0x04000EEB RID: 3819
	IsClient,
	// Token: 0x04000EEC RID: 3820
	ForcefullyTakingOver,
	// Token: 0x04000EED RID: 3821
	RequestingOwnership,
	// Token: 0x04000EEE RID: 3822
	RequestingOwnershipWaitingForSight,
	// Token: 0x04000EEF RID: 3823
	ForcefullyTakingOverWaitingForSight
}

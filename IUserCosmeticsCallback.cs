using System;

// Token: 0x02000938 RID: 2360
internal interface IUserCosmeticsCallback
{
	// Token: 0x06003944 RID: 14660
	bool OnGetUserCosmetics(string cosmetics);

	// Token: 0x170005A5 RID: 1445
	// (get) Token: 0x06003945 RID: 14661
	// (set) Token: 0x06003946 RID: 14662
	bool PendingUpdate { get; set; }
}

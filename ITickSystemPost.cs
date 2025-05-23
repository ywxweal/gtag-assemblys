using System;

// Token: 0x0200094C RID: 2380
internal interface ITickSystemPost
{
	// Token: 0x170005B9 RID: 1465
	// (get) Token: 0x060039BD RID: 14781
	// (set) Token: 0x060039BE RID: 14782
	bool PostTickRunning { get; set; }

	// Token: 0x060039BF RID: 14783
	void PostTick();
}

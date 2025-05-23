using System;

// Token: 0x0200094A RID: 2378
internal interface ITickSystemPre
{
	// Token: 0x170005B7 RID: 1463
	// (get) Token: 0x060039B7 RID: 14775
	// (set) Token: 0x060039B8 RID: 14776
	bool PreTickRunning { get; set; }

	// Token: 0x060039B9 RID: 14777
	void PreTick();
}

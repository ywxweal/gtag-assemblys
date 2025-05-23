using System;

// Token: 0x0200094B RID: 2379
internal interface ITickSystemTick
{
	// Token: 0x170005B8 RID: 1464
	// (get) Token: 0x060039BA RID: 14778
	// (set) Token: 0x060039BB RID: 14779
	bool TickRunning { get; set; }

	// Token: 0x060039BC RID: 14780
	void Tick();
}

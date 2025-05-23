using System;

// Token: 0x0200094B RID: 2379
internal interface ITickSystemTick
{
	// Token: 0x170005B8 RID: 1464
	// (get) Token: 0x060039BB RID: 14779
	// (set) Token: 0x060039BC RID: 14780
	bool TickRunning { get; set; }

	// Token: 0x060039BD RID: 14781
	void Tick();
}

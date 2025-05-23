using System;

// Token: 0x0200092B RID: 2347
public interface IFXEffectContext<T> where T : IFXEffectContextObject
{
	// Token: 0x170005A0 RID: 1440
	// (get) Token: 0x0600391E RID: 14622
	T effectContext { get; }

	// Token: 0x170005A1 RID: 1441
	// (get) Token: 0x0600391F RID: 14623
	FXSystemSettings settings { get; }
}

using System;

// Token: 0x02000532 RID: 1330
[Flags]
public enum CosmeticCritterAction
{
	// Token: 0x0400244D RID: 9293
	None = 0,
	// Token: 0x0400244E RID: 9294
	RPC = 1,
	// Token: 0x0400244F RID: 9295
	Spawn = 2,
	// Token: 0x04002450 RID: 9296
	Despawn = 4,
	// Token: 0x04002451 RID: 9297
	SpawnLinked = 8,
	// Token: 0x04002452 RID: 9298
	ShadeHeartbeat = 16
}

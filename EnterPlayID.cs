using System;

// Token: 0x020009AF RID: 2479
public struct EnterPlayID
{
	// Token: 0x06003B5E RID: 15198 RVA: 0x0011B5D8 File Offset: 0x001197D8
	[OnEnterPlay_Run]
	private static void NextID()
	{
		EnterPlayID.currentID++;
	}

	// Token: 0x06003B5F RID: 15199 RVA: 0x0011B5E8 File Offset: 0x001197E8
	public static EnterPlayID GetCurrent()
	{
		return new EnterPlayID
		{
			id = EnterPlayID.currentID
		};
	}

	// Token: 0x170005DA RID: 1498
	// (get) Token: 0x06003B60 RID: 15200 RVA: 0x0011B60A File Offset: 0x0011980A
	public bool IsCurrent
	{
		get
		{
			return this.id == EnterPlayID.currentID;
		}
	}

	// Token: 0x04003FD9 RID: 16345
	private static int currentID = 1;

	// Token: 0x04003FDA RID: 16346
	private int id;
}

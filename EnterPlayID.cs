using System;

// Token: 0x020009AF RID: 2479
public struct EnterPlayID
{
	// Token: 0x06003B5F RID: 15199 RVA: 0x0011B6B0 File Offset: 0x001198B0
	[OnEnterPlay_Run]
	private static void NextID()
	{
		EnterPlayID.currentID++;
	}

	// Token: 0x06003B60 RID: 15200 RVA: 0x0011B6C0 File Offset: 0x001198C0
	public static EnterPlayID GetCurrent()
	{
		return new EnterPlayID
		{
			id = EnterPlayID.currentID
		};
	}

	// Token: 0x170005DA RID: 1498
	// (get) Token: 0x06003B61 RID: 15201 RVA: 0x0011B6E2 File Offset: 0x001198E2
	public bool IsCurrent
	{
		get
		{
			return this.id == EnterPlayID.currentID;
		}
	}

	// Token: 0x04003FDA RID: 16346
	private static int currentID = 1;

	// Token: 0x04003FDB RID: 16347
	private int id;
}

using System;
using Photon.Realtime;

// Token: 0x020003A3 RID: 931
public class LegacyWorldTargetItem
{
	// Token: 0x060015C7 RID: 5575 RVA: 0x0006A0FC File Offset: 0x000682FC
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x0006A112 File Offset: 0x00068312
	public void Invalidate()
	{
		this.itemIdx = -1;
		this.owner = null;
	}

	// Token: 0x0400183A RID: 6202
	public Player owner;

	// Token: 0x0400183B RID: 6203
	public int itemIdx;
}

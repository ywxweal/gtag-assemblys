using System;
using System.Collections.Generic;

// Token: 0x02000150 RID: 336
[Serializable]
public class FoundAllocatorsMapped
{
	// Token: 0x04000A4C RID: 2636
	public string path;

	// Token: 0x04000A4D RID: 2637
	public List<ViewsAndAllocator> allocators = new List<ViewsAndAllocator>();

	// Token: 0x04000A4E RID: 2638
	public List<FoundAllocatorsMapped> subGroups = new List<FoundAllocatorsMapped>();
}

using System;
using System.Collections.Generic;
using GorillaTag;

// Token: 0x020009DE RID: 2526
public class PooledList<T> : ObjectPoolEvents
{
	// Token: 0x06003C5F RID: 15455 RVA: 0x000023F4 File Offset: 0x000005F4
	void ObjectPoolEvents.OnTaken()
	{
	}

	// Token: 0x06003C60 RID: 15456 RVA: 0x0012052B File Offset: 0x0011E72B
	void ObjectPoolEvents.OnReturned()
	{
		this.List.Clear();
	}

	// Token: 0x0400406C RID: 16492
	public List<T> List = new List<T>();
}

using System;
using UnityEngine;

// Token: 0x02000954 RID: 2388
internal abstract class TickSystemMono : MonoBehaviour, ITickSystem, ITickSystemPre, ITickSystemTick, ITickSystemPost
{
	// Token: 0x170005BA RID: 1466
	// (get) Token: 0x060039DB RID: 14811 RVA: 0x001164CB File Offset: 0x001146CB
	// (set) Token: 0x060039DC RID: 14812 RVA: 0x001164D3 File Offset: 0x001146D3
	public bool PreTickRunning { get; set; }

	// Token: 0x170005BB RID: 1467
	// (get) Token: 0x060039DD RID: 14813 RVA: 0x001164DC File Offset: 0x001146DC
	// (set) Token: 0x060039DE RID: 14814 RVA: 0x001164E4 File Offset: 0x001146E4
	public bool TickRunning { get; set; }

	// Token: 0x170005BC RID: 1468
	// (get) Token: 0x060039DF RID: 14815 RVA: 0x001164ED File Offset: 0x001146ED
	// (set) Token: 0x060039E0 RID: 14816 RVA: 0x001164F5 File Offset: 0x001146F5
	public bool PostTickRunning { get; set; }

	// Token: 0x060039E1 RID: 14817 RVA: 0x001164FE File Offset: 0x001146FE
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickSystemCallBack(this);
	}

	// Token: 0x060039E2 RID: 14818 RVA: 0x00116506 File Offset: 0x00114706
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickSystemCallback(this);
	}

	// Token: 0x060039E3 RID: 14819 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PreTick()
	{
	}

	// Token: 0x060039E4 RID: 14820 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void Tick()
	{
	}

	// Token: 0x060039E5 RID: 14821 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PostTick()
	{
	}
}

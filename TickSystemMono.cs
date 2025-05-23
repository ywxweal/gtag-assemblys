using System;
using UnityEngine;

// Token: 0x02000954 RID: 2388
internal abstract class TickSystemMono : MonoBehaviour, ITickSystem, ITickSystemPre, ITickSystemTick, ITickSystemPost
{
	// Token: 0x170005BA RID: 1466
	// (get) Token: 0x060039DC RID: 14812 RVA: 0x001165A3 File Offset: 0x001147A3
	// (set) Token: 0x060039DD RID: 14813 RVA: 0x001165AB File Offset: 0x001147AB
	public bool PreTickRunning { get; set; }

	// Token: 0x170005BB RID: 1467
	// (get) Token: 0x060039DE RID: 14814 RVA: 0x001165B4 File Offset: 0x001147B4
	// (set) Token: 0x060039DF RID: 14815 RVA: 0x001165BC File Offset: 0x001147BC
	public bool TickRunning { get; set; }

	// Token: 0x170005BC RID: 1468
	// (get) Token: 0x060039E0 RID: 14816 RVA: 0x001165C5 File Offset: 0x001147C5
	// (set) Token: 0x060039E1 RID: 14817 RVA: 0x001165CD File Offset: 0x001147CD
	public bool PostTickRunning { get; set; }

	// Token: 0x060039E2 RID: 14818 RVA: 0x001165D6 File Offset: 0x001147D6
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickSystemCallBack(this);
	}

	// Token: 0x060039E3 RID: 14819 RVA: 0x001165DE File Offset: 0x001147DE
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickSystemCallback(this);
	}

	// Token: 0x060039E4 RID: 14820 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PreTick()
	{
	}

	// Token: 0x060039E5 RID: 14821 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void Tick()
	{
	}

	// Token: 0x060039E6 RID: 14822 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PostTick()
	{
	}
}

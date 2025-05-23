using System;
using UnityEngine;

// Token: 0x02000955 RID: 2389
internal abstract class TickSystemPreTickMono : MonoBehaviour, ITickSystemPre
{
	// Token: 0x170005BD RID: 1469
	// (get) Token: 0x060039E8 RID: 14824 RVA: 0x001165E6 File Offset: 0x001147E6
	// (set) Token: 0x060039E9 RID: 14825 RVA: 0x001165EE File Offset: 0x001147EE
	public bool PreTickRunning { get; set; }

	// Token: 0x060039EA RID: 14826 RVA: 0x001165F7 File Offset: 0x001147F7
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPreTickCallback(this);
	}

	// Token: 0x060039EB RID: 14827 RVA: 0x001165FF File Offset: 0x001147FF
	public void OnDisable()
	{
		TickSystem<object>.RemovePreTickCallback(this);
	}

	// Token: 0x060039EC RID: 14828 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PreTick()
	{
	}
}

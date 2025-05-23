using System;
using UnityEngine;

// Token: 0x02000955 RID: 2389
internal abstract class TickSystemPreTickMono : MonoBehaviour, ITickSystemPre
{
	// Token: 0x170005BD RID: 1469
	// (get) Token: 0x060039E7 RID: 14823 RVA: 0x0011650E File Offset: 0x0011470E
	// (set) Token: 0x060039E8 RID: 14824 RVA: 0x00116516 File Offset: 0x00114716
	public bool PreTickRunning { get; set; }

	// Token: 0x060039E9 RID: 14825 RVA: 0x0011651F File Offset: 0x0011471F
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPreTickCallback(this);
	}

	// Token: 0x060039EA RID: 14826 RVA: 0x00116527 File Offset: 0x00114727
	public void OnDisable()
	{
		TickSystem<object>.RemovePreTickCallback(this);
	}

	// Token: 0x060039EB RID: 14827 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PreTick()
	{
	}
}

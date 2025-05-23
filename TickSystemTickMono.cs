using System;
using UnityEngine;

// Token: 0x02000956 RID: 2390
internal abstract class TickSystemTickMono : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170005BE RID: 1470
	// (get) Token: 0x060039ED RID: 14829 RVA: 0x0011652F File Offset: 0x0011472F
	// (set) Token: 0x060039EE RID: 14830 RVA: 0x00116537 File Offset: 0x00114737
	public bool TickRunning { get; set; }

	// Token: 0x060039EF RID: 14831 RVA: 0x0002E4DD File Offset: 0x0002C6DD
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060039F0 RID: 14832 RVA: 0x0002E4E5 File Offset: 0x0002C6E5
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060039F1 RID: 14833 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void Tick()
	{
	}
}

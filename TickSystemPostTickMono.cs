using System;
using UnityEngine;

// Token: 0x02000957 RID: 2391
internal abstract class TickSystemPostTickMono : MonoBehaviour, ITickSystemPost
{
	// Token: 0x170005BF RID: 1471
	// (get) Token: 0x060039F3 RID: 14835 RVA: 0x00116540 File Offset: 0x00114740
	// (set) Token: 0x060039F4 RID: 14836 RVA: 0x00116548 File Offset: 0x00114748
	public bool PostTickRunning { get; set; }

	// Token: 0x060039F5 RID: 14837 RVA: 0x00116551 File Offset: 0x00114751
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x060039F6 RID: 14838 RVA: 0x000D1CE3 File Offset: 0x000CFEE3
	public virtual void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x060039F7 RID: 14839 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PostTick()
	{
	}
}

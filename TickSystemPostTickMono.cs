using System;
using UnityEngine;

// Token: 0x02000957 RID: 2391
internal abstract class TickSystemPostTickMono : MonoBehaviour, ITickSystemPost
{
	// Token: 0x170005BF RID: 1471
	// (get) Token: 0x060039F4 RID: 14836 RVA: 0x00116618 File Offset: 0x00114818
	// (set) Token: 0x060039F5 RID: 14837 RVA: 0x00116620 File Offset: 0x00114820
	public bool PostTickRunning { get; set; }

	// Token: 0x060039F6 RID: 14838 RVA: 0x00116629 File Offset: 0x00114829
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x060039F7 RID: 14839 RVA: 0x000D1D87 File Offset: 0x000CFF87
	public virtual void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x060039F8 RID: 14840 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void PostTick()
	{
	}
}

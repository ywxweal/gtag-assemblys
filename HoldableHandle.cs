using System;
using UnityEngine;

// Token: 0x02000405 RID: 1029
public class HoldableHandle : InteractionPoint
{
	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x060018E8 RID: 6376 RVA: 0x00078E6B File Offset: 0x0007706B
	public new HoldableObject Holdable
	{
		get
		{
			return this.holdable;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x060018E9 RID: 6377 RVA: 0x00078E73 File Offset: 0x00077073
	public CapsuleCollider Capsule
	{
		get
		{
			return this.handleCapsuleTrigger;
		}
	}

	// Token: 0x04001BC8 RID: 7112
	[SerializeField]
	private HoldableObject holdable;

	// Token: 0x04001BC9 RID: 7113
	[SerializeField]
	private CapsuleCollider handleCapsuleTrigger;
}

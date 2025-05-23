using System;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public interface IRangedVariable<T> : IVariable<T>, IVariable
{
	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x06002934 RID: 10548
	// (set) Token: 0x06002935 RID: 10549
	T Min { get; set; }

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x06002936 RID: 10550
	// (set) Token: 0x06002937 RID: 10551
	T Max { get; set; }

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x06002938 RID: 10552
	T Range { get; }

	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x06002939 RID: 10553
	AnimationCurve Curve { get; }
}

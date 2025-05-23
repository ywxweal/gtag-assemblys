using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D69 RID: 3433
	public interface IGuidedRefTargetMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x060055BC RID: 21948
		// (set) Token: 0x060055BD RID: 21949
		GuidedRefBasicTargetInfo GRefTargetInfo { get; set; }

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x060055BE RID: 21950
		Object GuidedRefTargetObject { get; }
	}
}

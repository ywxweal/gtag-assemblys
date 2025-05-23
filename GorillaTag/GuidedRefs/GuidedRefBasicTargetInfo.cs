using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D5C RID: 3420
	[Serializable]
	public struct GuidedRefBasicTargetInfo
	{
		// Token: 0x040058E2 RID: 22754
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040058E3 RID: 22755
		[Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO[] hubIds;

		// Token: 0x040058E4 RID: 22756
		[DebugOption]
		[SerializeField]
		public bool hackIgnoreDuplicateRegistration;
	}
}

using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D6C RID: 3436
	public struct GuidedRefTryResolveInfo
	{
		// Token: 0x04005903 RID: 22787
		public int fieldId;

		// Token: 0x04005904 RID: 22788
		public int index;

		// Token: 0x04005905 RID: 22789
		[FormerlySerializedAs("target")]
		public IGuidedRefTargetMono targetMono;
	}
}

using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D63 RID: 3427
	[Serializable]
	public struct GuidedRefReceiverFieldInfo
	{
		// Token: 0x0600559A RID: 21914 RVA: 0x001A1554 File Offset: 0x0019F754
		public GuidedRefReceiverFieldInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targetId = null;
			this.hubId = null;
			this.fieldId = 0;
		}

		// Token: 0x040058F2 RID: 22770
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x040058F3 RID: 22771
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040058F4 RID: 22772
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x040058F5 RID: 22773
		[NonSerialized]
		public int fieldId;
	}
}

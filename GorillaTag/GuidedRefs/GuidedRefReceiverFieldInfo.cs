using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D63 RID: 3427
	[Serializable]
	public struct GuidedRefReceiverFieldInfo
	{
		// Token: 0x0600559B RID: 21915 RVA: 0x001A162C File Offset: 0x0019F82C
		public GuidedRefReceiverFieldInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targetId = null;
			this.hubId = null;
			this.fieldId = 0;
		}

		// Token: 0x040058F3 RID: 22771
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x040058F4 RID: 22772
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040058F5 RID: 22773
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x040058F6 RID: 22774
		[NonSerialized]
		public int fieldId;
	}
}

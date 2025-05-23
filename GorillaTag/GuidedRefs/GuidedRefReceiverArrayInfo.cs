using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D6A RID: 3434
	[Serializable]
	public struct GuidedRefReceiverArrayInfo
	{
		// Token: 0x060055C0 RID: 21952 RVA: 0x001A16E2 File Offset: 0x0019F8E2
		public GuidedRefReceiverArrayInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targets = Array.Empty<GuidedRefTargetIdSO>();
			this.hubId = null;
			this.fieldId = 0;
			this.resolveCount = 0;
		}

		// Token: 0x040058FB RID: 22779
		[Tooltip("Controls whether the array should be overridden by the guided refs.")]
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x040058FC RID: 22780
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x040058FD RID: 22781
		[SerializeField]
		public GuidedRefTargetIdSO[] targets;

		// Token: 0x040058FE RID: 22782
		[NonSerialized]
		public int fieldId;

		// Token: 0x040058FF RID: 22783
		[NonSerialized]
		public int resolveCount;
	}
}

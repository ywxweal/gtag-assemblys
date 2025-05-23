using System;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D68 RID: 3432
	public interface IGuidedRefReceiverMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x060055B7 RID: 21943
		bool GuidedRefTryResolveReference(GuidedRefTryResolveInfo target);

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x060055B8 RID: 21944
		// (set) Token: 0x060055B9 RID: 21945
		int GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x060055BA RID: 21946
		void OnAllGuidedRefsResolved();

		// Token: 0x060055BB RID: 21947
		void OnGuidedRefTargetDestroyed(int fieldId);
	}
}

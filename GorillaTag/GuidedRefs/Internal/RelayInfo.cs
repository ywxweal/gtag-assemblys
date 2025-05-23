using System;
using System.Collections.Generic;

namespace GorillaTag.GuidedRefs.Internal
{
	// Token: 0x02000D6D RID: 3437
	public class RelayInfo
	{
		// Token: 0x04005906 RID: 22790
		[NonSerialized]
		public IGuidedRefTargetMono targetMono;

		// Token: 0x04005907 RID: 22791
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> registeredFields;

		// Token: 0x04005908 RID: 22792
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> resolvedFields;
	}
}

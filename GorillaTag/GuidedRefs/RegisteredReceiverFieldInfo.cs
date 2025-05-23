using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D6B RID: 3435
	public struct RegisteredReceiverFieldInfo
	{
		// Token: 0x04005900 RID: 22784
		[FormerlySerializedAs("receiver")]
		public IGuidedRefReceiverMono receiverMono;

		// Token: 0x04005901 RID: 22785
		public int fieldId;

		// Token: 0x04005902 RID: 22786
		public int index;
	}
}

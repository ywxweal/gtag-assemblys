using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D6B RID: 3435
	public struct RegisteredReceiverFieldInfo
	{
		// Token: 0x040058FF RID: 22783
		[FormerlySerializedAs("receiver")]
		public IGuidedRefReceiverMono receiverMono;

		// Token: 0x04005900 RID: 22784
		public int fieldId;

		// Token: 0x04005901 RID: 22785
		public int index;
	}
}

using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D7D RID: 3453
	[Serializable]
	public struct CosmeticPlacementInfo
	{
		// Token: 0x0400595C RID: 22876
		[Tooltip("The bone to attach the cosmetic to.")]
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x0400595D RID: 22877
		public XformOffset offset;
	}
}

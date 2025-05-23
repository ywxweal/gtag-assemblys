using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D79 RID: 3449
	[Serializable]
	public struct CosmeticHoldableSlotAttachInfo
	{
		// Token: 0x0400593C RID: 22844
		[Tooltip("The anchor that this holdable cosmetic can attach to.")]
		public GTSturdyEnum<GTHardCodedBones.EHandAndStowSlots> stowSlot;

		// Token: 0x0400593D RID: 22845
		public XformOffset offset;
	}
}

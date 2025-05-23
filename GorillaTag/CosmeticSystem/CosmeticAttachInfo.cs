using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D78 RID: 3448
	[Serializable]
	public struct CosmeticAttachInfo
	{
		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x060055E1 RID: 21985 RVA: 0x001A20DC File Offset: 0x001A02DC
		public static CosmeticAttachInfo Identity
		{
			get
			{
				return new CosmeticAttachInfo
				{
					selectSide = ECosmeticSelectSide.Both,
					parentBone = GTHardCodedBones.EBone.None,
					offset = XformOffset.Identity
				};
			}
		}

		// Token: 0x060055E2 RID: 21986 RVA: 0x001A2118 File Offset: 0x001A0318
		public CosmeticAttachInfo(ECosmeticSelectSide selectSide, GTHardCodedBones.EBone parentBone, XformOffset offset)
		{
			this.selectSide = selectSide;
			this.parentBone = parentBone;
			this.offset = offset;
		}

		// Token: 0x04005939 RID: 22841
		[Tooltip("(Not used for holdables) Determines if the cosmetic part be shown depending on the hand that is used to press the in-game wardrobe \"EQUIP\" button.\n- Both: Show no matter what hand is used.\n- Left: Only show if the left hand selected.\n- Right: Only show if the right hand selected.\n")]
		public StringEnum<ECosmeticSelectSide> selectSide;

		// Token: 0x0400593A RID: 22842
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x0400593B RID: 22843
		public XformOffset offset;
	}
}

using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D7B RID: 3451
	[Serializable]
	public struct CosmeticPart
	{
		// Token: 0x04005958 RID: 22872
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x04005959 RID: 22873
		[Tooltip("Determines how the cosmetic part will be attached to the player.")]
		public CosmeticAttachInfo[] attachAnchors;

		// Token: 0x0400595A RID: 22874
		[NonSerialized]
		public ECosmeticPartType partType;
	}
}

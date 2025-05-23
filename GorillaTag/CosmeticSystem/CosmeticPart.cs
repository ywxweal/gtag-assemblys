using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D7B RID: 3451
	[Serializable]
	public struct CosmeticPart
	{
		// Token: 0x04005957 RID: 22871
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x04005958 RID: 22872
		[Tooltip("Determines how the cosmetic part will be attached to the player.")]
		public CosmeticAttachInfo[] attachAnchors;

		// Token: 0x04005959 RID: 22873
		[NonSerialized]
		public ECosmeticPartType partType;
	}
}

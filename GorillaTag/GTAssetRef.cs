using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GorillaTag
{
	// Token: 0x02000D06 RID: 3334
	[Serializable]
	public class GTAssetRef<TObject> : AssetReferenceT<TObject> where TObject : Object
	{
		// Token: 0x060053AB RID: 21419 RVA: 0x0019632C File Offset: 0x0019452C
		public GTAssetRef(string guid)
			: base(guid)
		{
		}
	}
}

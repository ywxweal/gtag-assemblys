using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GorillaTag
{
	// Token: 0x02000D06 RID: 3334
	[Serializable]
	public class GTAssetRef<TObject> : AssetReferenceT<TObject> where TObject : Object
	{
		// Token: 0x060053AA RID: 21418 RVA: 0x00196254 File Offset: 0x00194454
		public GTAssetRef(string guid)
			: base(guid)
		{
		}
	}
}

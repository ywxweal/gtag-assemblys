using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02000EC9 RID: 3785
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityLinkedListSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06005DD7 RID: 24023 RVA: 0x001CD4FC File Offset: 0x001CB6FC
		// (set) Token: 0x06005DD8 RID: 24024 RVA: 0x001CD504 File Offset: 0x001CB704
		[WeaverGenerated]
		public override Quaternion[] DataProperty
		{
			[WeaverGenerated]
			get
			{
				return this.Data;
			}
			[WeaverGenerated]
			set
			{
				this.Data = value;
			}
		}

		// Token: 0x06005DD9 RID: 24025 RVA: 0x001CD50D File Offset: 0x001CB70D
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x040063DC RID: 25564
		[WeaverGenerated]
		public Quaternion[] Data = Array.Empty<Quaternion>();
	}
}

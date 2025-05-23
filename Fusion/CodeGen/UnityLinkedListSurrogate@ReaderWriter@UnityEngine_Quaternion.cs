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
		// (get) Token: 0x06005DD6 RID: 24022 RVA: 0x001CD424 File Offset: 0x001CB624
		// (set) Token: 0x06005DD7 RID: 24023 RVA: 0x001CD42C File Offset: 0x001CB62C
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

		// Token: 0x06005DD8 RID: 24024 RVA: 0x001CD435 File Offset: 0x001CB635
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x040063DB RID: 25563
		[WeaverGenerated]
		public Quaternion[] Data = Array.Empty<Quaternion>();
	}
}

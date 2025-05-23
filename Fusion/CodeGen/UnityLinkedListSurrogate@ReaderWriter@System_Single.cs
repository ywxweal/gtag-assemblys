using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EDB RID: 3803
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@System_Single : UnityLinkedListSurrogate<float, ReaderWriter@System_Single>
	{
		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06005DFD RID: 24061 RVA: 0x001CD65C File Offset: 0x001CB85C
		// (set) Token: 0x06005DFE RID: 24062 RVA: 0x001CD664 File Offset: 0x001CB864
		[WeaverGenerated]
		public override float[] DataProperty
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

		// Token: 0x06005DFF RID: 24063 RVA: 0x001CD66D File Offset: 0x001CB86D
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x040064C9 RID: 25801
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}

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
		// (get) Token: 0x06005DFE RID: 24062 RVA: 0x001CD734 File Offset: 0x001CB934
		// (set) Token: 0x06005DFF RID: 24063 RVA: 0x001CD73C File Offset: 0x001CB93C
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

		// Token: 0x06005E00 RID: 24064 RVA: 0x001CD745 File Offset: 0x001CB945
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x040064CA RID: 25802
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}

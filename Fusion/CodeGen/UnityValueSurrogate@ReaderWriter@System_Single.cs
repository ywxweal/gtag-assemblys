using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EB2 RID: 3762
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@System_Single : UnityValueSurrogate<float, ReaderWriter@System_Single>
	{
		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x06005DA4 RID: 23972 RVA: 0x001CD234 File Offset: 0x001CB434
		// (set) Token: 0x06005DA5 RID: 23973 RVA: 0x001CD23C File Offset: 0x001CB43C
		[WeaverGenerated]
		public override float DataProperty
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

		// Token: 0x06005DA6 RID: 23974 RVA: 0x001CD245 File Offset: 0x001CB445
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x0400619D RID: 24989
		[WeaverGenerated]
		public float Data;
	}
}

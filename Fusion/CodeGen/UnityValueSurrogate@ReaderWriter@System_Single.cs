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
		// (get) Token: 0x06005DA3 RID: 23971 RVA: 0x001CD15C File Offset: 0x001CB35C
		// (set) Token: 0x06005DA4 RID: 23972 RVA: 0x001CD164 File Offset: 0x001CB364
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

		// Token: 0x06005DA5 RID: 23973 RVA: 0x001CD16D File Offset: 0x001CB36D
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x0400619C RID: 24988
		[WeaverGenerated]
		public float Data;
	}
}

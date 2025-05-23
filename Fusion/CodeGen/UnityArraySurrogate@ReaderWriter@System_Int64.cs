using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EC0 RID: 3776
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Int64 : UnityArraySurrogate<long, ReaderWriter@System_Int64>
	{
		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06005DC5 RID: 24005 RVA: 0x001CD400 File Offset: 0x001CB600
		// (set) Token: 0x06005DC6 RID: 24006 RVA: 0x001CD408 File Offset: 0x001CB608
		[WeaverGenerated]
		public override long[] DataProperty
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

		// Token: 0x06005DC7 RID: 24007 RVA: 0x001CD411 File Offset: 0x001CB611
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Int64()
		{
		}

		// Token: 0x04006285 RID: 25221
		[WeaverGenerated]
		public long[] Data = Array.Empty<long>();
	}
}

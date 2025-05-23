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
		// (get) Token: 0x06005DC4 RID: 24004 RVA: 0x001CD328 File Offset: 0x001CB528
		// (set) Token: 0x06005DC5 RID: 24005 RVA: 0x001CD330 File Offset: 0x001CB530
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

		// Token: 0x06005DC6 RID: 24006 RVA: 0x001CD339 File Offset: 0x001CB539
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Int64()
		{
		}

		// Token: 0x04006284 RID: 25220
		[WeaverGenerated]
		public long[] Data = Array.Empty<long>();
	}
}

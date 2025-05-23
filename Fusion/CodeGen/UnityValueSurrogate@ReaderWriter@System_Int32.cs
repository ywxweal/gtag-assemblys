using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EBB RID: 3771
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@System_Int32 : UnityValueSurrogate<int, ReaderWriter@System_Int32>
	{
		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06005DB8 RID: 23992 RVA: 0x001CD284 File Offset: 0x001CB484
		// (set) Token: 0x06005DB9 RID: 23993 RVA: 0x001CD28C File Offset: 0x001CB48C
		[WeaverGenerated]
		public override int DataProperty
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

		// Token: 0x06005DBA RID: 23994 RVA: 0x001CD295 File Offset: 0x001CB495
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x040061B1 RID: 25009
		[WeaverGenerated]
		public int Data;
	}
}

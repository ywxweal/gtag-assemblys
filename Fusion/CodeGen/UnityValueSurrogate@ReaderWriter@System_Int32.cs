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
		// (get) Token: 0x06005DB9 RID: 23993 RVA: 0x001CD35C File Offset: 0x001CB55C
		// (set) Token: 0x06005DBA RID: 23994 RVA: 0x001CD364 File Offset: 0x001CB564
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

		// Token: 0x06005DBB RID: 23995 RVA: 0x001CD36D File Offset: 0x001CB56D
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x040061B2 RID: 25010
		[WeaverGenerated]
		public int Data;
	}
}

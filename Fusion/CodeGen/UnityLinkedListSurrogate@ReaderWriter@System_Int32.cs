using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000ED5 RID: 3797
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@System_Int32 : UnityLinkedListSurrogate<int, ReaderWriter@System_Int32>
	{
		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06005DEE RID: 24046 RVA: 0x001CD580 File Offset: 0x001CB780
		// (set) Token: 0x06005DEF RID: 24047 RVA: 0x001CD588 File Offset: 0x001CB788
		[WeaverGenerated]
		public override int[] DataProperty
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

		// Token: 0x06005DF0 RID: 24048 RVA: 0x001CD591 File Offset: 0x001CB791
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x040064B2 RID: 25778
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}

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
		// (get) Token: 0x06005DEF RID: 24047 RVA: 0x001CD658 File Offset: 0x001CB858
		// (set) Token: 0x06005DF0 RID: 24048 RVA: 0x001CD660 File Offset: 0x001CB860
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

		// Token: 0x06005DF1 RID: 24049 RVA: 0x001CD669 File Offset: 0x001CB869
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x040064B3 RID: 25779
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}

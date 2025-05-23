using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000ED4 RID: 3796
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@System_Byte : UnityLinkedListSurrogate<byte, ReaderWriter@System_Byte>
	{
		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06005DEC RID: 24044 RVA: 0x001CD634 File Offset: 0x001CB834
		// (set) Token: 0x06005DED RID: 24045 RVA: 0x001CD63C File Offset: 0x001CB83C
		[WeaverGenerated]
		public override byte[] DataProperty
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

		// Token: 0x06005DEE RID: 24046 RVA: 0x001CD645 File Offset: 0x001CB845
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Byte()
		{
		}

		// Token: 0x040064B2 RID: 25778
		[WeaverGenerated]
		public byte[] Data = Array.Empty<byte>();
	}
}

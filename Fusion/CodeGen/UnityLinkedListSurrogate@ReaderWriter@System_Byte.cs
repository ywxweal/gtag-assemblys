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
		// (get) Token: 0x06005DEB RID: 24043 RVA: 0x001CD55C File Offset: 0x001CB75C
		// (set) Token: 0x06005DEC RID: 24044 RVA: 0x001CD564 File Offset: 0x001CB764
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

		// Token: 0x06005DED RID: 24045 RVA: 0x001CD56D File Offset: 0x001CB76D
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@System_Byte()
		{
		}

		// Token: 0x040064B1 RID: 25777
		[WeaverGenerated]
		public byte[] Data = Array.Empty<byte>();
	}
}

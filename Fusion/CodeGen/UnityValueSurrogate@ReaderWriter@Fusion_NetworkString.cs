using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EAC RID: 3756
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06005D92 RID: 23954 RVA: 0x001CD130 File Offset: 0x001CB330
		// (set) Token: 0x06005D93 RID: 23955 RVA: 0x001CD138 File Offset: 0x001CB338
		[WeaverGenerated]
		public override NetworkString<_32> DataProperty
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

		// Token: 0x06005D94 RID: 23956 RVA: 0x001CD141 File Offset: 0x001CB341
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04006195 RID: 24981
		[WeaverGenerated]
		public NetworkString<_32> Data;
	}
}

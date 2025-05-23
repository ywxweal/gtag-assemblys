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
		// (get) Token: 0x06005D91 RID: 23953 RVA: 0x001CD058 File Offset: 0x001CB258
		// (set) Token: 0x06005D92 RID: 23954 RVA: 0x001CD060 File Offset: 0x001CB260
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

		// Token: 0x06005D93 RID: 23955 RVA: 0x001CD069 File Offset: 0x001CB269
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04006194 RID: 24980
		[WeaverGenerated]
		public NetworkString<_32> Data;
	}
}

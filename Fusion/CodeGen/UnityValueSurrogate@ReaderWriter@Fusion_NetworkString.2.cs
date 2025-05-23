using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000ECD RID: 3789
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_128>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06005DDF RID: 24031 RVA: 0x001CD4C4 File Offset: 0x001CB6C4
		// (set) Token: 0x06005DE0 RID: 24032 RVA: 0x001CD4CC File Offset: 0x001CB6CC
		[WeaverGenerated]
		public override NetworkString<_128> DataProperty
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

		// Token: 0x06005DE1 RID: 24033 RVA: 0x001CD4D5 File Offset: 0x001CB6D5
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x0400645F RID: 25695
		[WeaverGenerated]
		public NetworkString<_128> Data;
	}
}

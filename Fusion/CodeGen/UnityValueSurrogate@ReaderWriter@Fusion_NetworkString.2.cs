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
		// (get) Token: 0x06005DE0 RID: 24032 RVA: 0x001CD59C File Offset: 0x001CB79C
		// (set) Token: 0x06005DE1 RID: 24033 RVA: 0x001CD5A4 File Offset: 0x001CB7A4
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

		// Token: 0x06005DE2 RID: 24034 RVA: 0x001CD5AD File Offset: 0x001CB7AD
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04006460 RID: 25696
		[WeaverGenerated]
		public NetworkString<_128> Data;
	}
}

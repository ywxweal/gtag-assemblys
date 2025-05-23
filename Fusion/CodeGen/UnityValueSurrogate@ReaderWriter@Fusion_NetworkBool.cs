using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EA8 RID: 3752
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkBool : UnityValueSurrogate<NetworkBool, ReaderWriter@Fusion_NetworkBool>
	{
		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06005D88 RID: 23944 RVA: 0x001CCFC4 File Offset: 0x001CB1C4
		// (set) Token: 0x06005D89 RID: 23945 RVA: 0x001CCFCC File Offset: 0x001CB1CC
		[WeaverGenerated]
		public override NetworkBool DataProperty
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

		// Token: 0x06005D8A RID: 23946 RVA: 0x001CCFD5 File Offset: 0x001CB1D5
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkBool()
		{
		}

		// Token: 0x04006170 RID: 24944
		[WeaverGenerated]
		public NetworkBool Data;
	}
}

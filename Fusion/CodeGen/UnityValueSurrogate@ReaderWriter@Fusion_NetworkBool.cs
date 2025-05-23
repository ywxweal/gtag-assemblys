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
		// (get) Token: 0x06005D89 RID: 23945 RVA: 0x001CD09C File Offset: 0x001CB29C
		// (set) Token: 0x06005D8A RID: 23946 RVA: 0x001CD0A4 File Offset: 0x001CB2A4
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

		// Token: 0x06005D8B RID: 23947 RVA: 0x001CD0AD File Offset: 0x001CB2AD
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkBool()
		{
		}

		// Token: 0x04006171 RID: 24945
		[WeaverGenerated]
		public NetworkBool Data;
	}
}

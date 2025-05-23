using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EB5 RID: 3765
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@Fusion_NetworkBool : UnityArraySurrogate<NetworkBool, ReaderWriter@Fusion_NetworkBool>
	{
		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06005DA7 RID: 23975 RVA: 0x001CD24D File Offset: 0x001CB44D
		// (set) Token: 0x06005DA8 RID: 23976 RVA: 0x001CD255 File Offset: 0x001CB455
		[WeaverGenerated]
		public override NetworkBool[] DataProperty
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

		// Token: 0x06005DA9 RID: 23977 RVA: 0x001CD25E File Offset: 0x001CB45E
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@Fusion_NetworkBool()
		{
		}

		// Token: 0x040061A9 RID: 25001
		[WeaverGenerated]
		public NetworkBool[] Data = Array.Empty<NetworkBool>();
	}
}

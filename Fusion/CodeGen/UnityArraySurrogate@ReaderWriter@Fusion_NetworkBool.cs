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
		// (get) Token: 0x06005DA6 RID: 23974 RVA: 0x001CD175 File Offset: 0x001CB375
		// (set) Token: 0x06005DA7 RID: 23975 RVA: 0x001CD17D File Offset: 0x001CB37D
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

		// Token: 0x06005DA8 RID: 23976 RVA: 0x001CD186 File Offset: 0x001CB386
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@Fusion_NetworkBool()
		{
		}

		// Token: 0x040061A8 RID: 25000
		[WeaverGenerated]
		public NetworkBool[] Data = Array.Empty<NetworkBool>();
	}
}

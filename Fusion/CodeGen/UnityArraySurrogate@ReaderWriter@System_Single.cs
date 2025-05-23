using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000ED8 RID: 3800
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Single : UnityArraySurrogate<float, ReaderWriter@System_Single>
	{
		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06005DFB RID: 24059 RVA: 0x001CD710 File Offset: 0x001CB910
		// (set) Token: 0x06005DFC RID: 24060 RVA: 0x001CD718 File Offset: 0x001CB918
		[WeaverGenerated]
		public override float[] DataProperty
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

		// Token: 0x06005DFD RID: 24061 RVA: 0x001CD721 File Offset: 0x001CB921
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x040064B6 RID: 25782
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}

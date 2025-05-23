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
		// (get) Token: 0x06005DFA RID: 24058 RVA: 0x001CD638 File Offset: 0x001CB838
		// (set) Token: 0x06005DFB RID: 24059 RVA: 0x001CD640 File Offset: 0x001CB840
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

		// Token: 0x06005DFC RID: 24060 RVA: 0x001CD649 File Offset: 0x001CB849
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Single()
		{
		}

		// Token: 0x040064B5 RID: 25781
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}

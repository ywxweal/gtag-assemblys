using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000EC1 RID: 3777
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Int32 : UnityArraySurrogate<int, ReaderWriter@System_Int32>
	{
		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06005DC7 RID: 24007 RVA: 0x001CD34C File Offset: 0x001CB54C
		// (set) Token: 0x06005DC8 RID: 24008 RVA: 0x001CD354 File Offset: 0x001CB554
		[WeaverGenerated]
		public override int[] DataProperty
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

		// Token: 0x06005DC9 RID: 24009 RVA: 0x001CD35D File Offset: 0x001CB55D
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Int32()
		{
		}

		// Token: 0x04006285 RID: 25221
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}

using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02000ED7 RID: 3799
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ReaderWriter@System_Boolean : UnityArraySurrogate<bool, ReaderWriter@System_Boolean>
	{
		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06005DF7 RID: 24055 RVA: 0x001CD614 File Offset: 0x001CB814
		// (set) Token: 0x06005DF8 RID: 24056 RVA: 0x001CD61C File Offset: 0x001CB81C
		[WeaverGenerated]
		public override bool[] DataProperty
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

		// Token: 0x06005DF9 RID: 24057 RVA: 0x001CD625 File Offset: 0x001CB825
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Boolean()
		{
		}

		// Token: 0x040064B4 RID: 25780
		[WeaverGenerated]
		public bool[] Data = Array.Empty<bool>();
	}
}

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
		// (get) Token: 0x06005DF8 RID: 24056 RVA: 0x001CD6EC File Offset: 0x001CB8EC
		// (set) Token: 0x06005DF9 RID: 24057 RVA: 0x001CD6F4 File Offset: 0x001CB8F4
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

		// Token: 0x06005DFA RID: 24058 RVA: 0x001CD6FD File Offset: 0x001CB8FD
		[WeaverGenerated]
		public UnityArraySurrogate@ReaderWriter@System_Boolean()
		{
		}

		// Token: 0x040064B5 RID: 25781
		[WeaverGenerated]
		public bool[] Data = Array.Empty<bool>();
	}
}

using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02000EB9 RID: 3769
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityValueSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06005DAF RID: 23983 RVA: 0x001CD210 File Offset: 0x001CB410
		// (set) Token: 0x06005DB0 RID: 23984 RVA: 0x001CD218 File Offset: 0x001CB418
		[WeaverGenerated]
		public override Quaternion DataProperty
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

		// Token: 0x06005DB1 RID: 23985 RVA: 0x001CD221 File Offset: 0x001CB421
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x040061AF RID: 25007
		[WeaverGenerated]
		public Quaternion Data;
	}
}

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
		// (get) Token: 0x06005DB0 RID: 23984 RVA: 0x001CD2E8 File Offset: 0x001CB4E8
		// (set) Token: 0x06005DB1 RID: 23985 RVA: 0x001CD2F0 File Offset: 0x001CB4F0
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

		// Token: 0x06005DB2 RID: 23986 RVA: 0x001CD2F9 File Offset: 0x001CB4F9
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x040061B0 RID: 25008
		[WeaverGenerated]
		public Quaternion Data;
	}
}

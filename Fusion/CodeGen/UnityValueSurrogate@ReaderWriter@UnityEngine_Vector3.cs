using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02000EB0 RID: 3760
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3 : UnityValueSurrogate<Vector3, ReaderWriter@UnityEngine_Vector3>
	{
		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x06005D9B RID: 23963 RVA: 0x001CD1C0 File Offset: 0x001CB3C0
		// (set) Token: 0x06005D9C RID: 23964 RVA: 0x001CD1C8 File Offset: 0x001CB3C8
		[WeaverGenerated]
		public override Vector3 DataProperty
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

		// Token: 0x06005D9D RID: 23965 RVA: 0x001CD1D1 File Offset: 0x001CB3D1
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3()
		{
		}

		// Token: 0x0400619B RID: 24987
		[WeaverGenerated]
		public Vector3 Data;
	}
}

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
		// (get) Token: 0x06005D9A RID: 23962 RVA: 0x001CD0E8 File Offset: 0x001CB2E8
		// (set) Token: 0x06005D9B RID: 23963 RVA: 0x001CD0F0 File Offset: 0x001CB2F0
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

		// Token: 0x06005D9C RID: 23964 RVA: 0x001CD0F9 File Offset: 0x001CB2F9
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Vector3()
		{
		}

		// Token: 0x0400619A RID: 24986
		[WeaverGenerated]
		public Vector3 Data;
	}
}

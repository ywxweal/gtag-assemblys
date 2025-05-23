using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02000EC6 RID: 3782
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Vector3 : UnityLinkedListSurrogate<Vector3, ReaderWriter@UnityEngine_Vector3>
	{
		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06005DD3 RID: 24019 RVA: 0x001CD400 File Offset: 0x001CB600
		// (set) Token: 0x06005DD4 RID: 24020 RVA: 0x001CD408 File Offset: 0x001CB608
		[WeaverGenerated]
		public override Vector3[] DataProperty
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

		// Token: 0x06005DD5 RID: 24021 RVA: 0x001CD411 File Offset: 0x001CB611
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Vector3()
		{
		}

		// Token: 0x04006322 RID: 25378
		[WeaverGenerated]
		public Vector3[] Data = Array.Empty<Vector3>();
	}
}

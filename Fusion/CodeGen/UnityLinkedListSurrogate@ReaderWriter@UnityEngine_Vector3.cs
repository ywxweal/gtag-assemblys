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
		// (get) Token: 0x06005DD4 RID: 24020 RVA: 0x001CD4D8 File Offset: 0x001CB6D8
		// (set) Token: 0x06005DD5 RID: 24021 RVA: 0x001CD4E0 File Offset: 0x001CB6E0
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

		// Token: 0x06005DD6 RID: 24022 RVA: 0x001CD4E9 File Offset: 0x001CB6E9
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Vector3()
		{
		}

		// Token: 0x04006323 RID: 25379
		[WeaverGenerated]
		public Vector3[] Data = Array.Empty<Vector3>();
	}
}

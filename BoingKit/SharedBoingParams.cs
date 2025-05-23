using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E83 RID: 3715
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x06005CE5 RID: 23781 RVA: 0x001CAAF0 File Offset: 0x001C8CF0
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x04006113 RID: 24851
		public BoingWork.Params Params;
	}
}

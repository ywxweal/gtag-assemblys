using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E83 RID: 3715
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x06005CE6 RID: 23782 RVA: 0x001CABC8 File Offset: 0x001C8DC8
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x04006114 RID: 24852
		public BoingWork.Params Params;
	}
}

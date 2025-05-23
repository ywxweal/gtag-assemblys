using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D1B RID: 3355
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour
	{
		// Token: 0x060053F1 RID: 21489 RVA: 0x00196CC9 File Offset: 0x00194EC9
		protected void Awake()
		{
			this.index = StaticLodManager.Register(this);
		}

		// Token: 0x060053F2 RID: 21490 RVA: 0x00196CD7 File Offset: 0x00194ED7
		protected void OnEnable()
		{
			StaticLodManager.SetEnabled(this.index, true);
		}

		// Token: 0x060053F3 RID: 21491 RVA: 0x00196CE5 File Offset: 0x00194EE5
		protected void OnDisable()
		{
			StaticLodManager.SetEnabled(this.index, false);
		}

		// Token: 0x060053F4 RID: 21492 RVA: 0x00196CF3 File Offset: 0x00194EF3
		private void OnDestroy()
		{
			StaticLodManager.Unregister(this.index);
		}

		// Token: 0x040056E1 RID: 22241
		private int index;

		// Token: 0x040056E2 RID: 22242
		public float collisionEnableDistance = 3f;

		// Token: 0x040056E3 RID: 22243
		public float uiFadeDistanceMin = 1f;

		// Token: 0x040056E4 RID: 22244
		public float uiFadeDistanceMax = 10f;
	}
}

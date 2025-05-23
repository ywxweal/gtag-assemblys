using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D1B RID: 3355
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour
	{
		// Token: 0x060053F0 RID: 21488 RVA: 0x00196BF1 File Offset: 0x00194DF1
		protected void Awake()
		{
			this.index = StaticLodManager.Register(this);
		}

		// Token: 0x060053F1 RID: 21489 RVA: 0x00196BFF File Offset: 0x00194DFF
		protected void OnEnable()
		{
			StaticLodManager.SetEnabled(this.index, true);
		}

		// Token: 0x060053F2 RID: 21490 RVA: 0x00196C0D File Offset: 0x00194E0D
		protected void OnDisable()
		{
			StaticLodManager.SetEnabled(this.index, false);
		}

		// Token: 0x060053F3 RID: 21491 RVA: 0x00196C1B File Offset: 0x00194E1B
		private void OnDestroy()
		{
			StaticLodManager.Unregister(this.index);
		}

		// Token: 0x040056E0 RID: 22240
		private int index;

		// Token: 0x040056E1 RID: 22241
		public float collisionEnableDistance = 3f;

		// Token: 0x040056E2 RID: 22242
		public float uiFadeDistanceMin = 1f;

		// Token: 0x040056E3 RID: 22243
		public float uiFadeDistanceMax = 10f;
	}
}

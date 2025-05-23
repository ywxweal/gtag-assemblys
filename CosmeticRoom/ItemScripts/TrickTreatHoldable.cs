using System;
using UnityEngine;

namespace CosmeticRoom.ItemScripts
{
	// Token: 0x02000BA2 RID: 2978
	public class TrickTreatHoldable : TransferrableObject
	{
		// Token: 0x060049E0 RID: 18912 RVA: 0x0016092B File Offset: 0x0015EB2B
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.candyCollider)
			{
				this.candyCollider.enabled = this.IsMyItem() && this.IsHeld();
			}
		}

		// Token: 0x04004CA8 RID: 19624
		public MeshCollider candyCollider;
	}
}

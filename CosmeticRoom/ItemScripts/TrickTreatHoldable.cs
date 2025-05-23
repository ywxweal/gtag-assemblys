using System;
using UnityEngine;

namespace CosmeticRoom.ItemScripts
{
	// Token: 0x02000BA2 RID: 2978
	public class TrickTreatHoldable : TransferrableObject
	{
		// Token: 0x060049E1 RID: 18913 RVA: 0x00160A03 File Offset: 0x0015EC03
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.candyCollider)
			{
				this.candyCollider.enabled = this.IsMyItem() && this.IsHeld();
			}
		}

		// Token: 0x04004CA9 RID: 19625
		public MeshCollider candyCollider;
	}
}

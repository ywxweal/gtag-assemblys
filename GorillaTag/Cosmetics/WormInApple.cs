using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000E00 RID: 3584
	public class WormInApple : MonoBehaviour
	{
		// Token: 0x060058B1 RID: 22705 RVA: 0x001B47A2 File Offset: 0x001B29A2
		public void OnHandTap()
		{
			if (this.blendShapeCosmetic && this.blendShapeCosmetic.GetBlendValue() > 0.5f)
			{
				UnityEvent onHandTapped = this.OnHandTapped;
				if (onHandTapped == null)
				{
					return;
				}
				onHandTapped.Invoke();
			}
		}

		// Token: 0x04005E20 RID: 24096
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04005E21 RID: 24097
		public UnityEvent OnHandTapped;
	}
}

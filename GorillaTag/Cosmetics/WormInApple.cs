using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000E00 RID: 3584
	public class WormInApple : MonoBehaviour
	{
		// Token: 0x060058B0 RID: 22704 RVA: 0x001B46CA File Offset: 0x001B28CA
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

		// Token: 0x04005E1F RID: 24095
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04005E20 RID: 24096
		public UnityEvent OnHandTapped;
	}
}

using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CBE RID: 3262
	public class GameObjectOnDisableDispatcher : MonoBehaviour
	{
		// Token: 0x14000095 RID: 149
		// (add) Token: 0x06005099 RID: 20633 RVA: 0x00180E24 File Offset: 0x0017F024
		// (remove) Token: 0x0600509A RID: 20634 RVA: 0x00180E5C File Offset: 0x0017F05C
		public event GameObjectOnDisableDispatcher.OnDisabledEvent OnDisabled;

		// Token: 0x0600509B RID: 20635 RVA: 0x00180E91 File Offset: 0x0017F091
		private void OnDisable()
		{
			if (this.OnDisabled != null)
			{
				this.OnDisabled(this);
			}
		}

		// Token: 0x02000CBF RID: 3263
		// (Invoke) Token: 0x0600509E RID: 20638
		public delegate void OnDisabledEvent(GameObjectOnDisableDispatcher me);
	}
}

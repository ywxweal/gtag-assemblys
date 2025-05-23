using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CBE RID: 3262
	public class GameObjectOnDisableDispatcher : MonoBehaviour
	{
		// Token: 0x14000095 RID: 149
		// (add) Token: 0x06005098 RID: 20632 RVA: 0x00180D4C File Offset: 0x0017EF4C
		// (remove) Token: 0x06005099 RID: 20633 RVA: 0x00180D84 File Offset: 0x0017EF84
		public event GameObjectOnDisableDispatcher.OnDisabledEvent OnDisabled;

		// Token: 0x0600509A RID: 20634 RVA: 0x00180DB9 File Offset: 0x0017EFB9
		private void OnDisable()
		{
			if (this.OnDisabled != null)
			{
				this.OnDisabled(this);
			}
		}

		// Token: 0x02000CBF RID: 3263
		// (Invoke) Token: 0x0600509D RID: 20637
		public delegate void OnDisabledEvent(GameObjectOnDisableDispatcher me);
	}
}

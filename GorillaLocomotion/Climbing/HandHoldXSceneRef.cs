using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000CEC RID: 3308
	public class HandHoldXSceneRef : MonoBehaviour
	{
		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06005221 RID: 21025 RVA: 0x0018FB08 File Offset: 0x0018DD08
		public HandHold target
		{
			get
			{
				HandHold handHold;
				if (this.reference.TryResolve<HandHold>(out handHold))
				{
					return handHold;
				}
				return null;
			}
		}

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x06005222 RID: 21026 RVA: 0x0018FB28 File Offset: 0x0018DD28
		public GameObject targetObject
		{
			get
			{
				GameObject gameObject;
				if (this.reference.TryResolve(out gameObject))
				{
					return gameObject;
				}
				return null;
			}
		}

		// Token: 0x0400564E RID: 22094
		[SerializeField]
		public XSceneRef reference;
	}
}

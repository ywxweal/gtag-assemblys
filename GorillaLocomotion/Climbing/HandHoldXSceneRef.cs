using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000CEC RID: 3308
	public class HandHoldXSceneRef : MonoBehaviour
	{
		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06005220 RID: 21024 RVA: 0x0018FA30 File Offset: 0x0018DC30
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
		// (get) Token: 0x06005221 RID: 21025 RVA: 0x0018FA50 File Offset: 0x0018DC50
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

		// Token: 0x0400564D RID: 22093
		[SerializeField]
		public XSceneRef reference;
	}
}

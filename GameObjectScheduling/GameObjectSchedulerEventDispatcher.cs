using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectScheduling
{
	// Token: 0x02000E2B RID: 3627
	public class GameObjectSchedulerEventDispatcher : MonoBehaviour
	{
		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06005AC0 RID: 23232 RVA: 0x001B9E7D File Offset: 0x001B807D
		public UnityEvent OnScheduledActivation
		{
			get
			{
				return this.onScheduledActivation;
			}
		}

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06005AC1 RID: 23233 RVA: 0x001B9E85 File Offset: 0x001B8085
		public UnityEvent OnScheduledDeactivation
		{
			get
			{
				return this.onScheduledDeactivation;
			}
		}

		// Token: 0x04005EC7 RID: 24263
		[SerializeField]
		private UnityEvent onScheduledActivation;

		// Token: 0x04005EC8 RID: 24264
		[SerializeField]
		private UnityEvent onScheduledDeactivation;
	}
}

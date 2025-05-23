using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectScheduling
{
	// Token: 0x02000E2B RID: 3627
	public class GameObjectSchedulerEventDispatcher : MonoBehaviour
	{
		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06005ABF RID: 23231 RVA: 0x001B9DA5 File Offset: 0x001B7FA5
		public UnityEvent OnScheduledActivation
		{
			get
			{
				return this.onScheduledActivation;
			}
		}

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06005AC0 RID: 23232 RVA: 0x001B9DAD File Offset: 0x001B7FAD
		public UnityEvent OnScheduledDeactivation
		{
			get
			{
				return this.onScheduledDeactivation;
			}
		}

		// Token: 0x04005EC6 RID: 24262
		[SerializeField]
		private UnityEvent onScheduledActivation;

		// Token: 0x04005EC7 RID: 24263
		[SerializeField]
		private UnityEvent onScheduledDeactivation;
	}
}

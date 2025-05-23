using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009F6 RID: 2550
public class TimeEvent : MonoBehaviour
{
	// Token: 0x06003D00 RID: 15616 RVA: 0x00121EEB File Offset: 0x001200EB
	protected void StartEvent()
	{
		this._ongoing = true;
		UnityEvent unityEvent = this.onEventStart;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06003D01 RID: 15617 RVA: 0x00121F04 File Offset: 0x00120104
	protected void StopEvent()
	{
		this._ongoing = false;
		UnityEvent unityEvent = this.onEventStop;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x040040BF RID: 16575
	public UnityEvent onEventStart;

	// Token: 0x040040C0 RID: 16576
	public UnityEvent onEventStop;

	// Token: 0x040040C1 RID: 16577
	[SerializeField]
	protected bool _ongoing;
}

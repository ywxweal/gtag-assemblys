using System;
using UnityEngine;

// Token: 0x020009D2 RID: 2514
public class TriggerEventNotifier : MonoBehaviour
{
	// Token: 0x14000070 RID: 112
	// (add) Token: 0x06003C2D RID: 15405 RVA: 0x0011FE58 File Offset: 0x0011E058
	// (remove) Token: 0x06003C2E RID: 15406 RVA: 0x0011FE90 File Offset: 0x0011E090
	public event TriggerEventNotifier.TriggerEvent TriggerEnterEvent;

	// Token: 0x14000071 RID: 113
	// (add) Token: 0x06003C2F RID: 15407 RVA: 0x0011FEC8 File Offset: 0x0011E0C8
	// (remove) Token: 0x06003C30 RID: 15408 RVA: 0x0011FF00 File Offset: 0x0011E100
	public event TriggerEventNotifier.TriggerEvent TriggerExitEvent;

	// Token: 0x06003C31 RID: 15409 RVA: 0x0011FF35 File Offset: 0x0011E135
	private void OnTriggerEnter(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerEnterEvent = this.TriggerEnterEvent;
		if (triggerEnterEvent == null)
		{
			return;
		}
		triggerEnterEvent(this, other);
	}

	// Token: 0x06003C32 RID: 15410 RVA: 0x0011FF49 File Offset: 0x0011E149
	private void OnTriggerExit(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerExitEvent = this.TriggerExitEvent;
		if (triggerExitEvent == null)
		{
			return;
		}
		triggerExitEvent(this, other);
	}

	// Token: 0x04004051 RID: 16465
	[HideInInspector]
	public int maskIndex;

	// Token: 0x020009D3 RID: 2515
	// (Invoke) Token: 0x06003C35 RID: 15413
	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);
}

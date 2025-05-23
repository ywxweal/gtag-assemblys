using System;
using UnityEngine;

// Token: 0x020009D2 RID: 2514
public class TriggerEventNotifier : MonoBehaviour
{
	// Token: 0x14000070 RID: 112
	// (add) Token: 0x06003C2C RID: 15404 RVA: 0x0011FD80 File Offset: 0x0011DF80
	// (remove) Token: 0x06003C2D RID: 15405 RVA: 0x0011FDB8 File Offset: 0x0011DFB8
	public event TriggerEventNotifier.TriggerEvent TriggerEnterEvent;

	// Token: 0x14000071 RID: 113
	// (add) Token: 0x06003C2E RID: 15406 RVA: 0x0011FDF0 File Offset: 0x0011DFF0
	// (remove) Token: 0x06003C2F RID: 15407 RVA: 0x0011FE28 File Offset: 0x0011E028
	public event TriggerEventNotifier.TriggerEvent TriggerExitEvent;

	// Token: 0x06003C30 RID: 15408 RVA: 0x0011FE5D File Offset: 0x0011E05D
	private void OnTriggerEnter(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerEnterEvent = this.TriggerEnterEvent;
		if (triggerEnterEvent == null)
		{
			return;
		}
		triggerEnterEvent(this, other);
	}

	// Token: 0x06003C31 RID: 15409 RVA: 0x0011FE71 File Offset: 0x0011E071
	private void OnTriggerExit(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerExitEvent = this.TriggerExitEvent;
		if (triggerExitEvent == null)
		{
			return;
		}
		triggerExitEvent(this, other);
	}

	// Token: 0x04004050 RID: 16464
	[HideInInspector]
	public int maskIndex;

	// Token: 0x020009D3 RID: 2515
	// (Invoke) Token: 0x06003C34 RID: 15412
	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);
}

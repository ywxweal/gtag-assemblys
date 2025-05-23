using System;
using UnityEngine;

// Token: 0x02000981 RID: 2433
public class CollisionEventNotifier : MonoBehaviour
{
	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06003A7B RID: 14971 RVA: 0x00118194 File Offset: 0x00116394
	// (remove) Token: 0x06003A7C RID: 14972 RVA: 0x001181CC File Offset: 0x001163CC
	public event CollisionEventNotifier.CollisionEvent CollisionEnterEvent;

	// Token: 0x1400006D RID: 109
	// (add) Token: 0x06003A7D RID: 14973 RVA: 0x00118204 File Offset: 0x00116404
	// (remove) Token: 0x06003A7E RID: 14974 RVA: 0x0011823C File Offset: 0x0011643C
	public event CollisionEventNotifier.CollisionEvent CollisionExitEvent;

	// Token: 0x06003A7F RID: 14975 RVA: 0x00118271 File Offset: 0x00116471
	private void OnCollisionEnter(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionEnterEvent = this.CollisionEnterEvent;
		if (collisionEnterEvent == null)
		{
			return;
		}
		collisionEnterEvent(this, collision);
	}

	// Token: 0x06003A80 RID: 14976 RVA: 0x00118285 File Offset: 0x00116485
	private void OnCollisionExit(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionExitEvent = this.CollisionExitEvent;
		if (collisionExitEvent == null)
		{
			return;
		}
		collisionExitEvent(this, collision);
	}

	// Token: 0x02000982 RID: 2434
	// (Invoke) Token: 0x06003A83 RID: 14979
	public delegate void CollisionEvent(CollisionEventNotifier notifier, Collision collision);
}

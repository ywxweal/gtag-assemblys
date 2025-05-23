using System;
using UnityEngine;

// Token: 0x02000981 RID: 2433
public class CollisionEventNotifier : MonoBehaviour
{
	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06003A7C RID: 14972 RVA: 0x0011826C File Offset: 0x0011646C
	// (remove) Token: 0x06003A7D RID: 14973 RVA: 0x001182A4 File Offset: 0x001164A4
	public event CollisionEventNotifier.CollisionEvent CollisionEnterEvent;

	// Token: 0x1400006D RID: 109
	// (add) Token: 0x06003A7E RID: 14974 RVA: 0x001182DC File Offset: 0x001164DC
	// (remove) Token: 0x06003A7F RID: 14975 RVA: 0x00118314 File Offset: 0x00116514
	public event CollisionEventNotifier.CollisionEvent CollisionExitEvent;

	// Token: 0x06003A80 RID: 14976 RVA: 0x00118349 File Offset: 0x00116549
	private void OnCollisionEnter(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionEnterEvent = this.CollisionEnterEvent;
		if (collisionEnterEvent == null)
		{
			return;
		}
		collisionEnterEvent(this, collision);
	}

	// Token: 0x06003A81 RID: 14977 RVA: 0x0011835D File Offset: 0x0011655D
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
	// (Invoke) Token: 0x06003A84 RID: 14980
	public delegate void CollisionEvent(CollisionEventNotifier notifier, Collision collision);
}

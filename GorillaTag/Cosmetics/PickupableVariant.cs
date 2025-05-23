using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DC9 RID: 3529
	public class PickupableVariant : MonoBehaviour
	{
		// Token: 0x06005777 RID: 22391 RVA: 0x000023F4 File Offset: 0x000005F4
		protected internal virtual void Release(HoldableObject holdable, Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
		}

		// Token: 0x06005778 RID: 22392 RVA: 0x000023F4 File Offset: 0x000005F4
		protected internal virtual void Pickup()
		{
		}

		// Token: 0x06005779 RID: 22393 RVA: 0x000023F4 File Offset: 0x000005F4
		protected internal virtual void DelayedPickup()
		{
		}
	}
}

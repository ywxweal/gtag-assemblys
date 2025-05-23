using System;
using GorillaTag.GuidedRefs;
using UnityEngine;

// Token: 0x020003AE RID: 942
public class SlingshotProjectileHitNotifier : BaseGuidedRefTargetMono
{
	// Token: 0x14000041 RID: 65
	// (add) Token: 0x0600160B RID: 5643 RVA: 0x0006B77C File Offset: 0x0006997C
	// (remove) Token: 0x0600160C RID: 5644 RVA: 0x0006B7B4 File Offset: 0x000699B4
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x0600160D RID: 5645 RVA: 0x0006B7EC File Offset: 0x000699EC
	// (remove) Token: 0x0600160E RID: 5646 RVA: 0x0006B824 File Offset: 0x00069A24
	public event SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent OnPaperPlaneHit;

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x0600160F RID: 5647 RVA: 0x0006B85C File Offset: 0x00069A5C
	// (remove) Token: 0x06001610 RID: 5648 RVA: 0x0006B894 File Offset: 0x00069A94
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	// Token: 0x14000044 RID: 68
	// (add) Token: 0x06001611 RID: 5649 RVA: 0x0006B8CC File Offset: 0x00069ACC
	// (remove) Token: 0x06001612 RID: 5650 RVA: 0x0006B904 File Offset: 0x00069B04
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerEnter;

	// Token: 0x14000045 RID: 69
	// (add) Token: 0x06001613 RID: 5651 RVA: 0x0006B93C File Offset: 0x00069B3C
	// (remove) Token: 0x06001614 RID: 5652 RVA: 0x0006B974 File Offset: 0x00069B74
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerExit;

	// Token: 0x06001615 RID: 5653 RVA: 0x0006B9A9 File Offset: 0x00069BA9
	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x0006B9BD File Offset: 0x00069BBD
	public void InvokeHit(PaperPlaneProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent onPaperPlaneHit = this.OnPaperPlaneHit;
		if (onPaperPlaneHit == null)
		{
			return;
		}
		onPaperPlaneHit(projectile, collider);
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x0006B9D1 File Offset: 0x00069BD1
	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x0006B9E5 File Offset: 0x00069BE5
	public void InvokeTriggerEnter(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerEnter = this.OnProjectileTriggerEnter;
		if (onProjectileTriggerEnter == null)
		{
			return;
		}
		onProjectileTriggerEnter(projectile, collider);
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x0006B9F9 File Offset: 0x00069BF9
	public void InvokeTriggerExit(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerExit = this.OnProjectileTriggerExit;
		if (onProjectileTriggerExit == null)
		{
			return;
		}
		onProjectileTriggerExit(projectile, collider);
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x0006BA0D File Offset: 0x00069C0D
	private new void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
		this.OnProjectileTriggerEnter = null;
		this.OnProjectileTriggerExit = null;
	}

	// Token: 0x020003AF RID: 943
	// (Invoke) Token: 0x0600161D RID: 5661
	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);

	// Token: 0x020003B0 RID: 944
	// (Invoke) Token: 0x06001621 RID: 5665
	public delegate void PaperPlaneProjectileHitEvent(PaperPlaneProjectile projectile, Collider collider);

	// Token: 0x020003B1 RID: 945
	// (Invoke) Token: 0x06001625 RID: 5669
	public delegate void ProjectileTriggerEvent(SlingshotProjectile projectile, Collider collider);
}

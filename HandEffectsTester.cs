using System;
using GorillaExtensions;
using TagEffects;
using UnityEngine;

// Token: 0x02000251 RID: 593
[RequireComponent(typeof(Collider))]
public class HandEffectsTester : MonoBehaviour, IHandEffectsTrigger
{
	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000D6E RID: 3438 RVA: 0x00045F81 File Offset: 0x00044181
	public bool Static
	{
		get
		{
			return this.isStatic;
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000D6F RID: 3439 RVA: 0x00045F89 File Offset: 0x00044189
	Transform IHandEffectsTrigger.Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000D70 RID: 3440 RVA: 0x00045F91 File Offset: 0x00044191
	VRRig IHandEffectsTrigger.Rig
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000D71 RID: 3441 RVA: 0x00045F94 File Offset: 0x00044194
	IHandEffectsTrigger.Mode IHandEffectsTrigger.EffectMode
	{
		get
		{
			return this.mode;
		}
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000D72 RID: 3442 RVA: 0x00045F9C File Offset: 0x0004419C
	bool IHandEffectsTrigger.FingersDown
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.FistBump || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000D73 RID: 3443 RVA: 0x00045FB3 File Offset: 0x000441B3
	bool IHandEffectsTrigger.FingersUp
	{
		get
		{
			return this.mode == IHandEffectsTrigger.Mode.HighFive || this.mode == IHandEffectsTrigger.Mode.HighFive_And_FistBump;
		}
	}

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000D74 RID: 3444 RVA: 0x00045FC9 File Offset: 0x000441C9
	public bool RightHand { get; }

	// Token: 0x06000D75 RID: 3445 RVA: 0x00045FD1 File Offset: 0x000441D1
	private void Awake()
	{
		this.triggerZone = base.GetComponent<Collider>();
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x00045FDF File Offset: 0x000441DF
	private void OnEnable()
	{
		if (!HandEffectsTriggerRegistry.HasInstance)
		{
			HandEffectsTriggerRegistry.FindInstance();
		}
		HandEffectsTriggerRegistry.Instance.Register(this);
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00045FF8 File Offset: 0x000441F8
	private void OnDisable()
	{
		HandEffectsTriggerRegistry.Instance.Unregister(this);
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000D78 RID: 3448 RVA: 0x00046005 File Offset: 0x00044205
	Vector3 IHandEffectsTrigger.Velocity
	{
		get
		{
			if (this.mode == IHandEffectsTrigger.Mode.HighFive)
			{
				return Vector3.zero;
			}
			IHandEffectsTrigger.Mode mode = this.mode;
			return Vector3.zero;
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000D79 RID: 3449 RVA: 0x00046023 File Offset: 0x00044223
	TagEffectPack IHandEffectsTrigger.CosmeticEffectPack
	{
		get
		{
			return this.cosmeticEffectPack;
		}
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnTriggerEntered(IHandEffectsTrigger other)
	{
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x0004602C File Offset: 0x0004422C
	public bool InTriggerZone(IHandEffectsTrigger t)
	{
		if (!(base.transform.position - t.Transform.position).IsShorterThan(this.triggerZone.bounds.size))
		{
			return false;
		}
		RaycastHit raycastHit;
		switch (this.mode)
		{
		case IHandEffectsTrigger.Mode.HighFive:
			return t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.FistBump:
			return t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius);
		case IHandEffectsTrigger.Mode.HighFive_And_FistBump:
			return (t.FingersUp && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.up), out raycastHit, this.triggerRadius)) || (t.FingersDown && this.triggerZone.Raycast(new Ray(t.Transform.position, t.Transform.right), out raycastHit, this.triggerRadius));
		}
		return this.triggerZone.Raycast(new Ray(t.Transform.position, this.triggerZone.bounds.center - t.Transform.position), out raycastHit, this.triggerRadius);
	}

	// Token: 0x0400110C RID: 4364
	[SerializeField]
	private TagEffectPack cosmeticEffectPack;

	// Token: 0x0400110D RID: 4365
	private Collider triggerZone;

	// Token: 0x0400110E RID: 4366
	public IHandEffectsTrigger.Mode mode;

	// Token: 0x0400110F RID: 4367
	[SerializeField]
	private float triggerRadius = 0.07f;

	// Token: 0x04001110 RID: 4368
	[SerializeField]
	private bool isStatic = true;
}

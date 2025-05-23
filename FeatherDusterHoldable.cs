using System;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class FeatherDusterHoldable : MonoBehaviour
{
	// Token: 0x06000627 RID: 1575 RVA: 0x00023693 File Offset: 0x00021893
	protected void Awake()
	{
		this.timeSinceLastSound = this.soundCooldown;
		this.emissionModule = this.particleFx.emission;
		this.initialRateOverTime = this.emissionModule.rateOverTimeMultiplier;
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x000236C3 File Offset: 0x000218C3
	protected void OnEnable()
	{
		this.lastWorldPos = base.transform.position;
		this.emissionModule.rateOverTimeMultiplier = 0f;
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x000236E8 File Offset: 0x000218E8
	protected void Update()
	{
		this.timeSinceLastSound += Time.deltaTime;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num = (position - this.lastWorldPos).magnitude / Time.deltaTime;
		this.emissionModule.rateOverTimeMultiplier = 0f;
		if (num >= this.collideMinSpeed && Physics.OverlapSphereNonAlloc(position, this.overlapSphereRadius * transform.localScale.x, this.colliderResult, this.collisionLayer) > 0)
		{
			this.emissionModule.rateOverTimeMultiplier = this.initialRateOverTime;
			if (this.timeSinceLastSound >= this.soundCooldown)
			{
				this.soundBankPlayer.Play();
				this.timeSinceLastSound = 0f;
			}
		}
		this.lastWorldPos = position;
	}

	// Token: 0x04000743 RID: 1859
	public LayerMask collisionLayer;

	// Token: 0x04000744 RID: 1860
	public float overlapSphereRadius = 0.08f;

	// Token: 0x04000745 RID: 1861
	[Tooltip("Collision is not tested until this speed requirement is met.")]
	private float collideMinSpeed = 1f;

	// Token: 0x04000746 RID: 1862
	public ParticleSystem particleFx;

	// Token: 0x04000747 RID: 1863
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000748 RID: 1864
	private float soundCooldown = 0.8f;

	// Token: 0x04000749 RID: 1865
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x0400074A RID: 1866
	private float initialRateOverTime;

	// Token: 0x0400074B RID: 1867
	private float timeSinceLastSound;

	// Token: 0x0400074C RID: 1868
	private Vector3 lastWorldPos;

	// Token: 0x0400074D RID: 1869
	private Collider[] colliderResult = new Collider[1];
}

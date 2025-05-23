using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class LeafBlowerEffects : MonoBehaviour, ISpawnable
{
	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x060009CB RID: 2507 RVA: 0x00034281 File Offset: 0x00032481
	// (set) Token: 0x060009CC RID: 2508 RVA: 0x00034289 File Offset: 0x00032489
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x060009CD RID: 2509 RVA: 0x00034292 File Offset: 0x00032492
	// (set) Token: 0x060009CE RID: 2510 RVA: 0x0003429A File Offset: 0x0003249A
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060009CF RID: 2511 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x000342A4 File Offset: 0x000324A4
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.headToleranceAngleCos = Mathf.Cos(0.017453292f * this.headToleranceAngle);
		this.squareHitAngleCos = Mathf.Cos(0.017453292f * this.squareHitAngle);
		this.fan = rig.cosmeticReferences.Get(this.fanRef).GetComponent<CosmeticFan>();
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x000342FB File Offset: 0x000324FB
	public void StartFan()
	{
		this.fan.Run();
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00034308 File Offset: 0x00032508
	public void StopFan()
	{
		this.fan.Stop();
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00034315 File Offset: 0x00032515
	public void UpdateEffects()
	{
		this.ProjectParticles();
		this.BlowFaces();
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00034324 File Offset: 0x00032524
	public void ProjectParticles()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.gunBarrel.transform.position, this.gunBarrel.transform.forward, out raycastHit, this.projectionRange, this.raycastLayers))
		{
			SpawnOnEnter component = raycastHit.collider.GetComponent<SpawnOnEnter>();
			if (component != null)
			{
				component.OnTriggerEnter(raycastHit.collider);
			}
			if (Vector3.Dot(raycastHit.normal, this.gunBarrel.transform.forward) < -this.squareHitAngleCos)
			{
				this.squareHitParticleSystem.transform.position = raycastHit.point;
				this.squareHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Play(true);
					return;
				}
			}
			else
			{
				this.angledHitParticleSystem.transform.position = raycastHit.point;
				this.angledHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Play(true);
					return;
				}
			}
		}
		else
		{
			this.StopEffects();
		}
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x000344D6 File Offset: 0x000326D6
	public void StopEffects()
	{
		this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x000344F4 File Offset: 0x000326F4
	public void BlowFaces()
	{
		Vector3 position = this.gunBarrel.transform.position;
		Vector3 forward = this.gunBarrel.transform.forward;
		if (NetworkSystem.Instance.InRoom)
		{
			using (List<VRRig>.Enumerator enumerator = GorillaParent.instance.vrrigs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VRRig vrrig = enumerator.Current;
					this.TryBlowFace(vrrig, position, forward);
				}
				return;
			}
		}
		this.TryBlowFace(VRRig.LocalRig, position, forward);
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x0003458C File Offset: 0x0003278C
	private void TryBlowFace(VRRig rig, Vector3 origin, Vector3 directionNormalized)
	{
		Transform rigTarget = rig.head.rigTarget;
		Vector3 vector = rigTarget.position - origin;
		float num = Vector3.Dot(vector, directionNormalized);
		if (num < 0f || num > this.projectionRange)
		{
			return;
		}
		if ((vector - num * directionNormalized).IsLongerThan(this.projectionWidth))
		{
			return;
		}
		if (Vector3.Dot(-rigTarget.forward, vector.normalized) < this.headToleranceAngleCos)
		{
			return;
		}
		rig.GetComponent<GorillaMouthFlap>().EnableLeafBlower();
	}

	// Token: 0x04000BEF RID: 3055
	[SerializeField]
	private GameObject gunBarrel;

	// Token: 0x04000BF0 RID: 3056
	[SerializeField]
	private float projectionRange;

	// Token: 0x04000BF1 RID: 3057
	[SerializeField]
	private float projectionWidth;

	// Token: 0x04000BF2 RID: 3058
	[SerializeField]
	private float headToleranceAngle;

	// Token: 0x04000BF3 RID: 3059
	[SerializeField]
	private LayerMask raycastLayers;

	// Token: 0x04000BF4 RID: 3060
	[SerializeField]
	private ParticleSystem angledHitParticleSystem;

	// Token: 0x04000BF5 RID: 3061
	[SerializeField]
	private ParticleSystem squareHitParticleSystem;

	// Token: 0x04000BF6 RID: 3062
	[SerializeField]
	private float squareHitAngle;

	// Token: 0x04000BF7 RID: 3063
	[SerializeField]
	private CosmeticRefID fanRef;

	// Token: 0x04000BF8 RID: 3064
	private float headToleranceAngleCos;

	// Token: 0x04000BF9 RID: 3065
	private float squareHitAngleCos;

	// Token: 0x04000BFA RID: 3066
	private CosmeticFan fan;
}

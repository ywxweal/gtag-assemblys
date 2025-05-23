using System;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B63 RID: 2915
	public class BuilderProjectile : MonoBehaviour
	{
		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06004818 RID: 18456 RVA: 0x00157D4D File Offset: 0x00155F4D
		// (set) Token: 0x06004819 RID: 18457 RVA: 0x00157D55 File Offset: 0x00155F55
		public Vector3 launchPosition { get; private set; }

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x0600481A RID: 18458 RVA: 0x00157D60 File Offset: 0x00155F60
		// (remove) Token: 0x0600481B RID: 18459 RVA: 0x00157D98 File Offset: 0x00155F98
		public event BuilderProjectile.ProjectileImpactEvent OnImpact;

		// Token: 0x0600481C RID: 18460 RVA: 0x00157DD0 File Offset: 0x00155FD0
		public void Launch(Vector3 position, Vector3 velocity, BuilderProjectileLauncher sourceObject, int projectileCount, float scale, int timeStamp)
		{
			this.particleLaunched = true;
			this.timeCreated = Time.time;
			this.projectileSource = sourceObject;
			float num = (NetworkSystem.Instance.ServerTimestamp - timeStamp) / 1000f;
			if (num >= this.lifeTime)
			{
				this.Deactivate();
				return;
			}
			this.timeCreated -= num;
			Vector3 vector = Vector3.ProjectOnPlane(velocity, Vector3.up);
			float num2 = 0.017453292f * Vector3.Angle(vector, velocity);
			float num3 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * 9.8f;
			Vector3 vector2 = num * Mathf.Cos(num2) * vector;
			float num4 = velocity.z * num * Mathf.Sin(num2) - 0.5f * num3 * num * num;
			this.launchPosition = position + vector2 + num4 * Vector3.down;
			Transform transform = base.transform;
			transform.position = position;
			transform.localScale = Vector3.one * scale;
			base.GetComponent<Collider>().contactOffset = 0.01f * scale;
			RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
			if (component != null)
			{
				component.objectRadiusForWaterCollision = 0.02f * scale;
			}
			this.projectileRigidbody.useGravity = false;
			Vector3 vector3 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * Physics.gravity;
			this.forceComponent.force = vector3;
			this.projectileRigidbody.velocity = velocity + num * vector3;
			this.projectileId = projectileCount;
			this.projectileRigidbody.position = position;
			this.projectileSource.RegisterProjectile(this);
		}

		// Token: 0x0600481D RID: 18461 RVA: 0x00157F91 File Offset: 0x00156191
		protected void Awake()
		{
			this.projectileRigidbody = base.GetComponent<Rigidbody>();
			this.forceComponent = base.GetComponent<ConstantForce>();
			this.initialScale = base.transform.localScale.x;
		}

		// Token: 0x0600481E RID: 18462 RVA: 0x00157FC4 File Offset: 0x001561C4
		public void Deactivate()
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			this.projectileRigidbody.useGravity = true;
			this.forceComponent.force = Vector3.zero;
			this.OnImpact = null;
			this.aoeKnockbackConfig = null;
			this.impactSoundVolumeOverride = null;
			this.impactSoundPitchOverride = null;
			this.impactEffectScaleMultiplier = 1f;
			this.gravityMultiplier = 1f;
			ObjectPools.instance.Destroy(base.gameObject);
		}

		// Token: 0x0600481F RID: 18463 RVA: 0x0015805C File Offset: 0x0015625C
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			Vector3 vector = position + normal * this.impactEffectOffset;
			GameObject gameObject = ObjectPools.instance.Instantiate(prefab, vector, true);
			Vector3 localScale = base.transform.localScale;
			gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(localScale.x * this.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
			}
		}

		// Token: 0x06004820 RID: 18464 RVA: 0x00158104 File Offset: 0x00156304
		public void ApplyHitKnockback(Vector3 hitNormal)
		{
			if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
			{
				Vector3 vector = Vector3.ProjectOnPlane(hitNormal, Vector3.up);
				vector.Normalize();
				Vector3 vector2 = 0.75f * vector + 0.25f * Vector3.up;
				vector2.Normalize();
				GTPlayer instance = GTPlayer.Instance;
				instance.ApplyKnockback(vector2, this.aoeKnockbackConfig.Value.knockbackVelocity, instance.scale < 0.9f);
			}
		}

		// Token: 0x06004821 RID: 18465 RVA: 0x00158194 File Offset: 0x00156394
		private void OnEnable()
		{
			this.timeCreated = 0f;
			this.particleLaunched = false;
		}

		// Token: 0x06004822 RID: 18466 RVA: 0x001581A8 File Offset: 0x001563A8
		protected void OnDisable()
		{
			this.particleLaunched = false;
			if (this.projectileSource != null)
			{
				this.projectileSource.UnRegisterProjectile(this);
			}
			this.projectileSource = null;
		}

		// Token: 0x06004823 RID: 18467 RVA: 0x001581D4 File Offset: 0x001563D4
		public void UpdateProjectile()
		{
			if (this.particleLaunched)
			{
				if (Time.time > this.timeCreated + this.lifeTime)
				{
					this.Deactivate();
				}
				if (this.faceDirectionOfTravel)
				{
					Transform transform = base.transform;
					Vector3 position = transform.position;
					Vector3 vector = position - this.previousPosition;
					transform.rotation = ((vector.sqrMagnitude > 0f) ? Quaternion.LookRotation(vector) : transform.rotation);
					this.previousPosition = position;
				}
			}
		}

		// Token: 0x06004824 RID: 18468 RVA: 0x00158250 File Offset: 0x00156450
		private void OnCollisionEnter(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06004825 RID: 18469 RVA: 0x00158308 File Offset: 0x00156508
		protected void OnCollisionStay(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06004826 RID: 18470 RVA: 0x001583C0 File Offset: 0x001565C0
		protected void OnTriggerEnter(Collider other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = ((componentInParent != null) ? componentInParent.creator : null);
			if (netPlayer == null)
			{
				return;
			}
			if (netPlayer.IsLocal)
			{
				return;
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
			this.Deactivate();
		}

		// Token: 0x04004A8F RID: 19087
		public BuilderProjectileLauncher projectileSource;

		// Token: 0x04004A90 RID: 19088
		[Tooltip("Rotates to point along the Y axis after spawn.")]
		public GameObject surfaceImpactEffectPrefab;

		// Token: 0x04004A91 RID: 19089
		[Tooltip("Distance from the surface that the particle should spawn.")]
		private float impactEffectOffset;

		// Token: 0x04004A92 RID: 19090
		public float lifeTime = 20f;

		// Token: 0x04004A93 RID: 19091
		public bool faceDirectionOfTravel = true;

		// Token: 0x04004A94 RID: 19092
		private bool particleLaunched;

		// Token: 0x04004A95 RID: 19093
		private float timeCreated;

		// Token: 0x04004A97 RID: 19095
		private Rigidbody projectileRigidbody;

		// Token: 0x04004A98 RID: 19096
		public int projectileId;

		// Token: 0x04004A99 RID: 19097
		private float initialScale;

		// Token: 0x04004A9A RID: 19098
		private Vector3 previousPosition;

		// Token: 0x04004A9B RID: 19099
		[HideInInspector]
		public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

		// Token: 0x04004A9C RID: 19100
		[HideInInspector]
		public float? impactSoundVolumeOverride;

		// Token: 0x04004A9D RID: 19101
		[HideInInspector]
		public float? impactSoundPitchOverride;

		// Token: 0x04004A9E RID: 19102
		[HideInInspector]
		public float impactEffectScaleMultiplier = 1f;

		// Token: 0x04004A9F RID: 19103
		[HideInInspector]
		public float gravityMultiplier = 1f;

		// Token: 0x04004AA0 RID: 19104
		private ConstantForce forceComponent;

		// Token: 0x02000B64 RID: 2916
		// (Invoke) Token: 0x06004829 RID: 18473
		public delegate void ProjectileImpactEvent(BuilderProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
	}
}

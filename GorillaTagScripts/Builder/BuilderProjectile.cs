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
		// (get) Token: 0x06004817 RID: 18455 RVA: 0x00157C75 File Offset: 0x00155E75
		// (set) Token: 0x06004818 RID: 18456 RVA: 0x00157C7D File Offset: 0x00155E7D
		public Vector3 launchPosition { get; private set; }

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x06004819 RID: 18457 RVA: 0x00157C88 File Offset: 0x00155E88
		// (remove) Token: 0x0600481A RID: 18458 RVA: 0x00157CC0 File Offset: 0x00155EC0
		public event BuilderProjectile.ProjectileImpactEvent OnImpact;

		// Token: 0x0600481B RID: 18459 RVA: 0x00157CF8 File Offset: 0x00155EF8
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

		// Token: 0x0600481C RID: 18460 RVA: 0x00157EB9 File Offset: 0x001560B9
		protected void Awake()
		{
			this.projectileRigidbody = base.GetComponent<Rigidbody>();
			this.forceComponent = base.GetComponent<ConstantForce>();
			this.initialScale = base.transform.localScale.x;
		}

		// Token: 0x0600481D RID: 18461 RVA: 0x00157EEC File Offset: 0x001560EC
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

		// Token: 0x0600481E RID: 18462 RVA: 0x00157F84 File Offset: 0x00156184
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

		// Token: 0x0600481F RID: 18463 RVA: 0x0015802C File Offset: 0x0015622C
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

		// Token: 0x06004820 RID: 18464 RVA: 0x001580BC File Offset: 0x001562BC
		private void OnEnable()
		{
			this.timeCreated = 0f;
			this.particleLaunched = false;
		}

		// Token: 0x06004821 RID: 18465 RVA: 0x001580D0 File Offset: 0x001562D0
		protected void OnDisable()
		{
			this.particleLaunched = false;
			if (this.projectileSource != null)
			{
				this.projectileSource.UnRegisterProjectile(this);
			}
			this.projectileSource = null;
		}

		// Token: 0x06004822 RID: 18466 RVA: 0x001580FC File Offset: 0x001562FC
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

		// Token: 0x06004823 RID: 18467 RVA: 0x00158178 File Offset: 0x00156378
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

		// Token: 0x06004824 RID: 18468 RVA: 0x00158230 File Offset: 0x00156430
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

		// Token: 0x06004825 RID: 18469 RVA: 0x001582E8 File Offset: 0x001564E8
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

		// Token: 0x04004A8E RID: 19086
		public BuilderProjectileLauncher projectileSource;

		// Token: 0x04004A8F RID: 19087
		[Tooltip("Rotates to point along the Y axis after spawn.")]
		public GameObject surfaceImpactEffectPrefab;

		// Token: 0x04004A90 RID: 19088
		[Tooltip("Distance from the surface that the particle should spawn.")]
		private float impactEffectOffset;

		// Token: 0x04004A91 RID: 19089
		public float lifeTime = 20f;

		// Token: 0x04004A92 RID: 19090
		public bool faceDirectionOfTravel = true;

		// Token: 0x04004A93 RID: 19091
		private bool particleLaunched;

		// Token: 0x04004A94 RID: 19092
		private float timeCreated;

		// Token: 0x04004A96 RID: 19094
		private Rigidbody projectileRigidbody;

		// Token: 0x04004A97 RID: 19095
		public int projectileId;

		// Token: 0x04004A98 RID: 19096
		private float initialScale;

		// Token: 0x04004A99 RID: 19097
		private Vector3 previousPosition;

		// Token: 0x04004A9A RID: 19098
		[HideInInspector]
		public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

		// Token: 0x04004A9B RID: 19099
		[HideInInspector]
		public float? impactSoundVolumeOverride;

		// Token: 0x04004A9C RID: 19100
		[HideInInspector]
		public float? impactSoundPitchOverride;

		// Token: 0x04004A9D RID: 19101
		[HideInInspector]
		public float impactEffectScaleMultiplier = 1f;

		// Token: 0x04004A9E RID: 19102
		[HideInInspector]
		public float gravityMultiplier = 1f;

		// Token: 0x04004A9F RID: 19103
		private ConstantForce forceComponent;

		// Token: 0x02000B64 RID: 2916
		// (Invoke) Token: 0x06004828 RID: 18472
		public delegate void ProjectileImpactEvent(BuilderProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
	}
}

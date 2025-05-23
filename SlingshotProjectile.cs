using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003AA RID: 938
public class SlingshotProjectile : MonoBehaviour
{
	// Token: 0x1700026A RID: 618
	// (get) Token: 0x060015F1 RID: 5617 RVA: 0x0006ACE8 File Offset: 0x00068EE8
	// (set) Token: 0x060015F2 RID: 5618 RVA: 0x0006ACF0 File Offset: 0x00068EF0
	public Vector3 launchPosition { get; private set; }

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x060015F3 RID: 5619 RVA: 0x0006ACFC File Offset: 0x00068EFC
	// (remove) Token: 0x060015F4 RID: 5620 RVA: 0x0006AD34 File Offset: 0x00068F34
	public event SlingshotProjectile.ProjectileImpactEvent OnImpact;

	// Token: 0x060015F5 RID: 5621 RVA: 0x0006AD6C File Offset: 0x00068F6C
	public void Launch(Vector3 position, Vector3 velocity, NetPlayer player, bool blueTeam, bool orangeTeam, int projectileCount, float scale, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (this.launchSoundBankPlayer != null)
		{
			this.launchSoundBankPlayer.Play();
		}
		this.particleLaunched = true;
		this.timeCreated = Time.time;
		this.launchPosition = position;
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
		this.forceComponent.force = Physics.gravity * this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f);
		this.projectileRigidbody.velocity = velocity;
		this.projectileOwner = player;
		this.myProjectileCount = projectileCount;
		this.projectileRigidbody.position = position;
		this.ApplyTeamModelAndColor(blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x0006AE80 File Offset: 0x00069080
	protected void Awake()
	{
		if (this.playerImpactEffectPrefab == null)
		{
			this.playerImpactEffectPrefab = this.surfaceImpactEffectPrefab;
		}
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.forceComponent = base.GetComponent<ConstantForce>();
		this.initialScale = base.transform.localScale.x;
		this.matPropBlock = new MaterialPropertyBlock();
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x0006AEEC File Offset: 0x000690EC
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
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x0006AF78 File Offset: 0x00069178
	private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
	{
		Vector3 vector = position + normal * this.impactEffectOffset;
		GameObject gameObject = ObjectPools.instance.Instantiate(prefab, vector, true);
		Vector3 localScale = base.transform.localScale;
		gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
		gameObject.transform.up = normal;
		GorillaColorizableBase component = gameObject.GetComponent<GorillaColorizableBase>();
		if (component != null)
		{
			component.SetColor(this.teamColor);
		}
		SurfaceImpactFX component2 = gameObject.GetComponent<SurfaceImpactFX>();
		if (component2 != null)
		{
			component2.SetScale(localScale.x * this.impactEffectScaleMultiplier);
		}
		SoundBankPlayer component3 = gameObject.GetComponent<SoundBankPlayer>();
		if (component3 != null && !component3.playOnEnable)
		{
			component3.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
		}
		if (this.spawnWorldEffects != null)
		{
			this.spawnWorldEffects.RequestSpawn(position, normal);
		}
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x0006B05C File Offset: 0x0006925C
	public void CheckForAOEKnockback(Vector3 impactPosition, float impactSpeed)
	{
		if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
		{
			Vector3 vector = GTPlayer.Instance.HeadCenterPosition - impactPosition;
			if (vector.sqrMagnitude < this.aoeKnockbackConfig.Value.aeoOuterRadius * this.aoeKnockbackConfig.Value.aeoOuterRadius)
			{
				float magnitude = vector.magnitude;
				Vector3 vector2 = ((magnitude > 0.001f) ? (vector / magnitude) : Vector3.up);
				float num = Mathf.InverseLerp(this.aoeKnockbackConfig.Value.aeoOuterRadius, this.aoeKnockbackConfig.Value.aeoInnerRadius, magnitude);
				float num2 = Mathf.InverseLerp(0f, this.aoeKnockbackConfig.Value.impactVelocityThreshold, impactSpeed);
				GTPlayer.Instance.ApplyKnockback(vector2, this.aoeKnockbackConfig.Value.knockbackVelocity * num * num2, false);
				this.impactEffectScaleMultiplier = Mathf.Lerp(1f, this.impactEffectScaleMultiplier, num2);
				if (this.impactSoundVolumeOverride != null)
				{
					this.impactSoundVolumeOverride = new float?(Mathf.Lerp(this.impactSoundVolumeOverride.Value * 0.5f, this.impactSoundVolumeOverride.Value, num2));
				}
				float num3 = Mathf.Lerp(this.aoeKnockbackConfig.Value.aeoInnerRadius, this.aoeKnockbackConfig.Value.aeoOuterRadius, 0.25f);
				if (this.aoeKnockbackConfig.Value.playerProximityEffect != PlayerEffect.NONE && vector.sqrMagnitude < num3 * num3)
				{
					RoomSystem.SendPlayerEffect(PlayerEffect.SNOWBALL_IMPACT, NetworkSystem.Instance.LocalPlayer);
				}
			}
		}
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x0006B200 File Offset: 0x00069400
	public void ApplyTeamModelAndColor(bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (shouldOverrideColor)
		{
			this.teamColor = overrideColor;
		}
		else
		{
			this.teamColor = (blueTeam ? this.blueColor : (orangeTeam ? this.orangeColor : this.defaultColor));
		}
		this.blueBall.enabled = blueTeam;
		this.orangeBall.enabled = orangeTeam;
		this.defaultBall.enabled = !blueTeam && !orangeTeam;
		this.teamRenderer = (blueTeam ? this.blueBall : (orangeTeam ? this.orangeBall : this.defaultBall));
		this.ApplyColor(this.teamRenderer, (this.colorizeBalls || shouldOverrideColor) ? this.teamColor : Color.white);
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x0006B2AE File Offset: 0x000694AE
	protected void OnEnable()
	{
		this.timeCreated = 0f;
		this.particleLaunched = false;
		SlingshotProjectileManager.RegisterSP(this);
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x0006B2C8 File Offset: 0x000694C8
	protected void OnDisable()
	{
		this.particleLaunched = false;
		SlingshotProjectileManager.UnregisterSP(this);
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x0006B2D8 File Offset: 0x000694D8
	public void InvokeUpdate()
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

	// Token: 0x060015FE RID: 5630 RVA: 0x0006B353 File Offset: 0x00069553
	public void DestroyAfterRelease()
	{
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
		this.Deactivate();
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x0006B378 File Offset: 0x00069578
	protected void OnCollisionEnter(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.collider.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeHit(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x0006B408 File Offset: 0x00069608
	protected void OnCollisionStay(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeCollisionStay(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x0006B494 File Offset: 0x00069694
	protected void OnTriggerExit(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerExit(this, other);
		}
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x0006B4C4 File Offset: 0x000696C4
	protected void OnTriggerEnter(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerEnter(this, other);
		}
		if (this.projectileOwner == NetworkSystem.Instance.LocalPlayer)
		{
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			GorillaPaintbrawlManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = ((componentInParent != null) ? componentInParent.creator : null);
			if (netPlayer == null)
			{
				return;
			}
			if (NetworkSystem.Instance.LocalPlayer == netPlayer)
			{
				return;
			}
			SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, base.transform.position, netPlayer);
			}
			if (component && !component.LocalCanHit(NetworkSystem.Instance.LocalPlayer, netPlayer))
			{
				return;
			}
			if (component && GameMode.ActiveNetworkHandler)
			{
				GameMode.ActiveNetworkHandler.SendRPC("RPC_ReportSlingshotHit", false, new object[]
				{
					(netPlayer as PunNetPlayer).PlayerRef,
					base.transform.position,
					this.myProjectileCount
				});
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
			RoomSystem.SendImpactEffect(base.transform.position, this.teamColor.r, this.teamColor.g, this.teamColor.b, this.teamColor.a, this.myProjectileCount);
			this.Deactivate();
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		VRRig vrrig;
		if (attachedRigidbody.IsNotNull() && attachedRigidbody.gameObject.TryGetComponent<VRRig>(out vrrig))
		{
			UnityEvent<VRRig> onHitPlayer = this.OnHitPlayer;
			if (onHitPlayer == null)
			{
				return;
			}
			onHitPlayer.Invoke(vrrig);
		}
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x0006B689 File Offset: 0x00069889
	private void ApplyColor(Renderer rend, Color color)
	{
		if (!rend)
		{
			return;
		}
		this.matPropBlock.SetColor(SlingshotProjectile.baseColorShaderProp, color);
		this.matPropBlock.SetColor(SlingshotProjectile.colorShaderProp, color);
		rend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x04001874 RID: 6260
	public NetPlayer projectileOwner;

	// Token: 0x04001875 RID: 6261
	[Tooltip("Rotates to point along the Y axis after spawn.")]
	public GameObject surfaceImpactEffectPrefab;

	// Token: 0x04001876 RID: 6262
	[Tooltip("if left empty, the default player impact that is set in Room System Setting will be played")]
	public GameObject playerImpactEffectPrefab;

	// Token: 0x04001877 RID: 6263
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x04001878 RID: 6264
	[SerializeField]
	private SoundBankPlayer launchSoundBankPlayer;

	// Token: 0x04001879 RID: 6265
	public float lifeTime = 20f;

	// Token: 0x0400187A RID: 6266
	public float gravityMultiplier = 1f;

	// Token: 0x0400187B RID: 6267
	public Color defaultColor = Color.white;

	// Token: 0x0400187C RID: 6268
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x0400187D RID: 6269
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x0400187E RID: 6270
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer defaultBall;

	// Token: 0x0400187F RID: 6271
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer orangeBall;

	// Token: 0x04001880 RID: 6272
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer blueBall;

	// Token: 0x04001881 RID: 6273
	public bool colorizeBalls;

	// Token: 0x04001882 RID: 6274
	public bool faceDirectionOfTravel = true;

	// Token: 0x04001883 RID: 6275
	private bool particleLaunched;

	// Token: 0x04001884 RID: 6276
	private float timeCreated;

	// Token: 0x04001886 RID: 6278
	private Rigidbody projectileRigidbody;

	// Token: 0x04001887 RID: 6279
	private Color teamColor = Color.white;

	// Token: 0x04001888 RID: 6280
	private Renderer teamRenderer;

	// Token: 0x04001889 RID: 6281
	public int myProjectileCount;

	// Token: 0x0400188A RID: 6282
	private float initialScale;

	// Token: 0x0400188B RID: 6283
	private Vector3 previousPosition;

	// Token: 0x0400188C RID: 6284
	[HideInInspector]
	public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

	// Token: 0x0400188D RID: 6285
	[HideInInspector]
	public float? impactSoundVolumeOverride;

	// Token: 0x0400188E RID: 6286
	[HideInInspector]
	public float? impactSoundPitchOverride;

	// Token: 0x0400188F RID: 6287
	[HideInInspector]
	public float impactEffectScaleMultiplier = 1f;

	// Token: 0x04001890 RID: 6288
	private ConstantForce forceComponent;

	// Token: 0x04001892 RID: 6290
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04001893 RID: 6291
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x04001894 RID: 6292
	private static readonly int colorShaderProp = Shader.PropertyToID("_Color");

	// Token: 0x04001895 RID: 6293
	private static readonly int baseColorShaderProp = Shader.PropertyToID("_BaseColor");

	// Token: 0x04001896 RID: 6294
	public UnityEvent<VRRig> OnHitPlayer;

	// Token: 0x020003AB RID: 939
	[Serializable]
	public struct AOEKnockbackConfig
	{
		// Token: 0x04001897 RID: 6295
		public bool applyAOEKnockback;

		// Token: 0x04001898 RID: 6296
		[Tooltip("Full knockback velocity is imparted within the inner radius")]
		public float aeoInnerRadius;

		// Token: 0x04001899 RID: 6297
		[Tooltip("Partial knockback velocity is imparted between the inner and outer radius")]
		public float aeoOuterRadius;

		// Token: 0x0400189A RID: 6298
		public float knockbackVelocity;

		// Token: 0x0400189B RID: 6299
		[Tooltip("The required impact velocity to achieve full knockback velocity")]
		public float impactVelocityThreshold;

		// Token: 0x0400189C RID: 6300
		[SerializeField]
		public PlayerEffect playerProximityEffect;
	}

	// Token: 0x020003AC RID: 940
	// (Invoke) Token: 0x06001607 RID: 5639
	public delegate void ProjectileImpactEvent(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
}

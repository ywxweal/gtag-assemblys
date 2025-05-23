using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005C0 RID: 1472
public class GRToolClub : MonoBehaviour
{
	// Token: 0x1700036A RID: 874
	// (get) Token: 0x060023D3 RID: 9171 RVA: 0x000B4104 File Offset: 0x000B2304
	private bool IsExtended
	{
		get
		{
			return this.state == GRToolClub.State.Extended && this.extendedAmount > 0.7f;
		}
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x000B411E File Offset: 0x000B231E
	private void Awake()
	{
		this.retractableSection.localPosition = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x000B413F File Offset: 0x000B233F
	public void OnEnable()
	{
		this.SetExtendedAmount(0f);
		this.SetState(GRToolClub.State.Idle);
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000B4153 File Offset: 0x000B2353
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000B416C File Offset: 0x000B236C
	private bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000B4180 File Offset: 0x000B2380
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeld())
		{
			if (this.IsHeldLocal())
			{
				this.OnUpdateAuthority(deltaTime);
			}
			else
			{
				this.OnUpdateRemote(deltaTime);
			}
		}
		else
		{
			this.SetState(GRToolClub.State.Idle);
		}
		this.OnUpdateShared(deltaTime);
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x000B41C4 File Offset: 0x000B23C4
	private void OnUpdateAuthority(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolClub.State.Idle);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetState(GRToolClub.State.Extended);
			return;
		}
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x000B4224 File Offset: 0x000B2424
	private void OnUpdateRemote(float dt)
	{
		GRToolClub.State state = (GRToolClub.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x000B4250 File Offset: 0x000B2450
	private void OnUpdateShared(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (this.extendedAmount < 1f)
			{
				float num = Mathf.MoveTowards(this.extendedAmount, 1f, 1f / this.extensionTime * Time.deltaTime);
				this.SetExtendedAmount(num);
			}
		}
		else if (this.extendedAmount > 0f)
		{
			float num2 = Mathf.MoveTowards(this.extendedAmount, 0f, 1f / this.extensionTime * Time.deltaTime);
			this.SetExtendedAmount(num2);
			return;
		}
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x000B42DC File Offset: 0x000B24DC
	private void SetExtendedAmount(float newExtendedAmount)
	{
		this.extendedAmount = newExtendedAmount;
		float num = Mathf.Lerp(this.retractableSectionMin, this.retractableSectionMax, this.extendedAmount);
		this.retractableSection.localPosition = new Vector3(0f, num, 0f);
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000B4324 File Offset: 0x000B2524
	private void SetState(GRToolClub.State newState)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
		}
		this.state = newState;
		state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state == GRToolClub.State.Extended)
			{
				this.idleCollider.enabled = false;
				this.extendedCollider.enabled = true;
				this.poweredMeshRenderer.material = this.poweredMaterial;
				this.humAudioSource.Play();
				this.dullLight.SetActive(true);
				for (int i = 0; i < this.humParticleEffects.Count; i++)
				{
					this.humParticleEffects[i].gameObject.SetActive(true);
				}
			}
		}
		else
		{
			this.extendedCollider.enabled = false;
			this.idleCollider.enabled = true;
			this.poweredMeshRenderer.material = this.idleMaterial;
			this.humAudioSource.Stop();
			this.dullLight.SetActive(false);
			for (int j = 0; j < this.humParticleEffects.Count; j++)
			{
				this.humParticleEffects[j].gameObject.SetActive(false);
			}
		}
		if (this.IsHeldLocal())
		{
			GameEntityManager.instance.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000B4458 File Offset: 0x000B2658
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000B44BC File Offset: 0x000B26BC
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.IsExtended)
		{
			return;
		}
		float num = this.gameEntity.GetVelocity().sqrMagnitude;
		if (this.gameEntity.lastHeldByActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer != null)
		{
			float handSpeed = GamePlayerLocal.instance.GetHandSpeed(gamePlayer.FindHandIndex(this.gameEntity.id));
			num = handSpeed * handSpeed;
		}
		if (num < this.minHitSpeed)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.hitCooldownEnd)
		{
			return;
		}
		Collider collider = collision.collider;
		GREnemyChaser parentEnemy = this.GetParentEnemy<GREnemyChaser>(collider);
		if (parentEnemy != null)
		{
			if (this.tool.HasEnoughEnergy())
			{
				this.tool.UseEnergy();
				Vector3 vector = parentEnemy.transform.position - base.transform.position;
				vector.Normalize();
				vector *= 6f;
				if (gamePlayer != null)
				{
					Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(gamePlayer.FindHandIndex(this.gameEntity.id));
					handVelocity.y += 2f;
					vector = handVelocity.normalized * Mathf.Sqrt(num) * 2f;
				}
				Vector3 vector2 = parentEnemy.transform.position;
				RaycastHit raycastHit;
				if (Physics.SphereCast(vector2 + Vector3.up * 0.8f, 0.5f, Vector3.down, out raycastHit, 0.8f, GhostReactor.instance.envLayerMask))
				{
					vector2 = raycastHit.point;
				}
				parentEnemy.TryHitEnemy(this.tool, vector2, vector);
				this.OnHit();
				this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
			}
			else
			{
				this.OnHitFailedOutOfEnergy();
				this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
			}
		}
		GREnemyRanged parentEnemy2 = this.GetParentEnemy<GREnemyRanged>(collider);
		if (parentEnemy2 != null)
		{
			if (this.tool.HasEnoughEnergy())
			{
				this.tool.UseEnergy();
				Vector3 vector3 = parentEnemy2.transform.position - base.transform.position;
				vector3.y = 1f;
				vector3.Normalize();
				Vector3 vector4 = parentEnemy2.transform.position;
				RaycastHit raycastHit2;
				if (Physics.SphereCast(vector4 + Vector3.up * 0.6f, 0.4f, Vector3.down, out raycastHit2, 0.6f, GhostReactor.instance.envLayerMask))
				{
					vector4 = raycastHit2.point;
				}
				parentEnemy2.TryHitEnemy(this.tool, vector4, vector3 * 6f);
				this.OnHit();
				this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
			}
			else
			{
				this.OnHitFailedOutOfEnergy();
				this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
			}
		}
		if (collision.collider.attachedRigidbody != null)
		{
			GRBreakable component = collision.collider.attachedRigidbody.GetComponent<GRBreakable>();
			if (component != null && !component.BrokenLocal)
			{
				if (this.tool.HasEnoughEnergy())
				{
					this.OnHit();
					component.TryHit(this.tool.gameEntity);
					return;
				}
				this.OnHitFailedOutOfEnergy();
				this.hitCooldownEnd = timeAsDouble + 0.10000000149011612;
			}
		}
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000B4820 File Offset: 0x000B2A20
	private T GetParentEnemy<T>(Collider collider) where T : MonoBehaviour
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000B4868 File Offset: 0x000B2A68
	private void OnHit()
	{
		this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, 0.2f);
		this.audioSource.volume = this.hitWithEnergyVolume;
		this.audioSource.PlayOneShot(this.hitWithEnergyAudio);
		this.hitParticleEffect.Play();
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x000B48B7 File Offset: 0x000B2AB7
	private void OnHitFailedOutOfEnergy()
	{
		this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, 0.2f);
		this.audioSource.volume = this.hitEmptyVolume;
		this.audioSource.PlayOneShot(this.hitEmptyAudio);
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000B48F0 File Offset: 0x000B2AF0
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x040028A1 RID: 10401
	public GameEntity gameEntity;

	// Token: 0x040028A2 RID: 10402
	public GRTool tool;

	// Token: 0x040028A3 RID: 10403
	public Rigidbody rigidBody;

	// Token: 0x040028A4 RID: 10404
	public AudioSource audioSource;

	// Token: 0x040028A5 RID: 10405
	public AudioSource humAudioSource;

	// Token: 0x040028A6 RID: 10406
	public List<ParticleSystem> humParticleEffects = new List<ParticleSystem>();

	// Token: 0x040028A7 RID: 10407
	public ParticleSystem hitParticleEffect;

	// Token: 0x040028A8 RID: 10408
	public AudioClip hitWithEnergyAudio;

	// Token: 0x040028A9 RID: 10409
	public float hitWithEnergyVolume = 0.5f;

	// Token: 0x040028AA RID: 10410
	public AudioClip hitEmptyAudio;

	// Token: 0x040028AB RID: 10411
	public float hitEmptyVolume = 0.5f;

	// Token: 0x040028AC RID: 10412
	public float minHitSpeed = 2.25f;

	// Token: 0x040028AD RID: 10413
	public GameObject dullLight;

	// Token: 0x040028AE RID: 10414
	public Material idleMaterial;

	// Token: 0x040028AF RID: 10415
	public Material poweredMaterial;

	// Token: 0x040028B0 RID: 10416
	public MeshRenderer poweredMeshRenderer;

	// Token: 0x040028B1 RID: 10417
	public Transform retractableSection;

	// Token: 0x040028B2 RID: 10418
	public Collider idleCollider;

	// Token: 0x040028B3 RID: 10419
	public Collider extendedCollider;

	// Token: 0x040028B4 RID: 10420
	public float retractableSectionMin = -0.31f;

	// Token: 0x040028B5 RID: 10421
	public float retractableSectionMax;

	// Token: 0x040028B6 RID: 10422
	public float extensionTime = 0.15f;

	// Token: 0x040028B7 RID: 10423
	private float extendedAmount;

	// Token: 0x040028B8 RID: 10424
	private double hitCooldownEnd;

	// Token: 0x040028B9 RID: 10425
	private GRToolClub.State state;

	// Token: 0x020005C1 RID: 1473
	private enum State
	{
		// Token: 0x040028BB RID: 10427
		Idle,
		// Token: 0x040028BC RID: 10428
		Extended
	}
}

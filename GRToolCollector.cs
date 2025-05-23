using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x020005C2 RID: 1474
public class GRToolCollector : MonoBehaviour
{
	// Token: 0x060023E5 RID: 9189 RVA: 0x000B49A1 File Offset: 0x000B2BA1
	private void Awake()
	{
		this.state = GRToolCollector.State.Idle;
		this.stateTimeRemaining = -1f;
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000B49B5 File Offset: 0x000B2BB5
	private void OnEnable()
	{
		this.SetState(GRToolCollector.State.Idle);
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000B49BE File Offset: 0x000B2BBE
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000B49D7 File Offset: 0x000B2BD7
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000B49F8 File Offset: 0x000B2BF8
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000B4A2C File Offset: 0x000B2C2C
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
		{
			bool flag = this.IsButtonHeld();
			this.waitingForButtonRelease = this.waitingForButtonRelease && flag;
			if (flag && !this.waitingForButtonRelease)
			{
				this.SetStateAuthority(GRToolCollector.State.Vacuuming);
				this.activatedLocally = true;
				return;
			}
			break;
		}
		case GRToolCollector.State.Vacuuming:
		{
			bool flag2 = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Collect);
				return;
			}
			if (!flag2)
			{
				this.SetStateAuthority(GRToolCollector.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolCollector.State.Collect:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Cooldown);
				return;
			}
			break;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.activatedLocally = false;
				this.waitingForButtonRelease = true;
				this.SetStateAuthority(GRToolCollector.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000B4B1C File Offset: 0x000B2D1C
	private void OnUpdateRemote(float dt)
	{
		GRToolCollector.State state = (GRToolCollector.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000B4B46 File Offset: 0x000B2D46
	private void SetStateAuthority(GRToolCollector.State newState)
	{
		this.SetState(newState);
		GameEntityManager.instance.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000B4B68 File Offset: 0x000B2D68
	private void SetState(GRToolCollector.State newState)
	{
		this.state = newState;
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
			this.StopVacuum();
			this.stateTimeRemaining = -1f;
			return;
		case GRToolCollector.State.Vacuuming:
			this.StartVacuum();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolCollector.State.Collect:
			this.TryCollect();
			this.stateTimeRemaining = this.collectDuration;
			return;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000B4BE0 File Offset: 0x000B2DE0
	private void StartVacuum()
	{
		this.vacuumAudioSource.clip = this.vacuumSound;
		this.vacuumAudioSource.volume = this.vacuumSoundVolume;
		this.vacuumAudioSource.loop = true;
		this.vacuumAudioSource.Play();
		this.vacuumParticleEffect.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000B4C4F File Offset: 0x000B2E4F
	private void StopVacuum()
	{
		this.vacuumAudioSource.loop = false;
		this.vacuumAudioSource.Stop();
		this.vacuumParticleEffect.Stop();
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000B4C74 File Offset: 0x000B2E74
	private void TryCollect()
	{
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.2f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 1f, this.collectibleLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				GameObject gameObject = null;
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					gameObject = attachedRigidbody.gameObject;
				}
				else
				{
					GameEntity gameEntity = GameEntity.Get(raycastHit.collider);
					if (gameEntity != null)
					{
						gameObject = gameEntity.gameObject;
					}
				}
				if (gameObject != null)
				{
					GRCollectible component = gameObject.GetComponent<GRCollectible>();
					if (component != null)
					{
						GhostReactorManager.instance.RequestCollectItem(component.entity.id, this.gameEntity.id);
						return;
					}
					if (gameObject.GetComponent<GRCurrencyDepositor>() != null)
					{
						if (this.tool.energy > 0)
						{
							GhostReactorManager.instance.RequestDepositCurrency(this.gameEntity.id);
						}
						return;
					}
					GRTool component2 = gameObject.GetComponent<GRTool>();
					if (!(component2 == null) && !(component2 == this.tool))
					{
						GameEntity component3 = gameObject.GetComponent<GameEntity>();
						if (component2 != null && component3 != null)
						{
							GhostReactorManager.instance.RequestChargeTool(this.gameEntity.id, component3.id);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x000B4DFC File Offset: 0x000B2FFC
	public void PerformCollection(GRCollectible collectible)
	{
		this.tool.RefillEnergy(collectible.energyValue);
		this.collectAudioSource.volume = this.collectSoundVolume;
		this.collectAudioSource.PlayOneShot(this.collectSound);
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x000B4E34 File Offset: 0x000B3034
	public void PlayChargeEffect(GRTool targetTool)
	{
		if (targetTool == null)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		for (int i = 0; i < targetTool.energyMeters.Count; i++)
		{
			if (targetTool.energyMeters[i].chargePoint != null)
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].chargePoint.position);
			}
			else
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].transform.position);
			}
		}
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000B4F08 File Offset: 0x000B3108
	public void PlayChargeEffect(GRCurrencyDepositor targetDepositor)
	{
		if (targetDepositor == null)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetDepositor.depositingChargePoint.position);
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000B4F68 File Offset: 0x000B3168
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

	// Token: 0x060023F5 RID: 9205 RVA: 0x000B4FCC File Offset: 0x000B31CC
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

	// Token: 0x040028BD RID: 10429
	public GameEntity gameEntity;

	// Token: 0x040028BE RID: 10430
	public GRTool tool;

	// Token: 0x040028BF RID: 10431
	public int energyDepositPerUse = 100;

	// Token: 0x040028C0 RID: 10432
	public Transform shootFrom;

	// Token: 0x040028C1 RID: 10433
	public LayerMask collectibleLayerMask;

	// Token: 0x040028C2 RID: 10434
	public ParticleSystem vacuumParticleEffect;

	// Token: 0x040028C3 RID: 10435
	public AudioSource vacuumAudioSource;

	// Token: 0x040028C4 RID: 10436
	public AudioClip vacuumSound;

	// Token: 0x040028C5 RID: 10437
	public float vacuumSoundVolume = 0.2f;

	// Token: 0x040028C6 RID: 10438
	public AudioSource collectAudioSource;

	// Token: 0x040028C7 RID: 10439
	[FormerlySerializedAs("flashSound")]
	public AudioClip collectSound;

	// Token: 0x040028C8 RID: 10440
	[FormerlySerializedAs("flashSoundVolume")]
	public float collectSoundVolume = 1f;

	// Token: 0x040028C9 RID: 10441
	public AudioClip chargeBeamSound;

	// Token: 0x040028CA RID: 10442
	public float chargeBeamVolume = 0.2f;

	// Token: 0x040028CB RID: 10443
	public LightningDispatcher lightningDispatcher;

	// Token: 0x040028CC RID: 10444
	public float chargeDuration = 0.75f;

	// Token: 0x040028CD RID: 10445
	[FormerlySerializedAs("flashDuration")]
	public float collectDuration = 0.1f;

	// Token: 0x040028CE RID: 10446
	public float cooldownDuration;

	// Token: 0x040028CF RID: 10447
	[NonSerialized]
	public GhostReactorManager grManager;

	// Token: 0x040028D0 RID: 10448
	private GRToolCollector.State state;

	// Token: 0x040028D1 RID: 10449
	private float stateTimeRemaining;

	// Token: 0x040028D2 RID: 10450
	private bool activatedLocally;

	// Token: 0x040028D3 RID: 10451
	private bool waitingForButtonRelease;

	// Token: 0x040028D4 RID: 10452
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x020005C3 RID: 1475
	private enum State
	{
		// Token: 0x040028D6 RID: 10454
		Idle,
		// Token: 0x040028D7 RID: 10455
		Vacuuming,
		// Token: 0x040028D8 RID: 10456
		Collect,
		// Token: 0x040028D9 RID: 10457
		Cooldown
	}
}

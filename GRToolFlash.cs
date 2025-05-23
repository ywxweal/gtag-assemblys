using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005C4 RID: 1476
public class GRToolFlash : MonoBehaviour
{
	// Token: 0x060023F7 RID: 9207 RVA: 0x000B506A File Offset: 0x000B326A
	private void Awake()
	{
		this.state = GRToolFlash.State.Idle;
		this.stateTimeRemaining = -1f;
	}

	// Token: 0x060023F8 RID: 9208 RVA: 0x000B507E File Offset: 0x000B327E
	private void OnEnable()
	{
		this.StopFlash();
		this.SetState(GRToolFlash.State.Idle);
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000B508D File Offset: 0x000B328D
	private bool IsHeldLocal()
	{
		return this.item.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000B50A6 File Offset: 0x000B32A6
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000B50C0 File Offset: 0x000B32C0
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

	// Token: 0x060023FC RID: 9212 RVA: 0x000B50F4 File Offset: 0x000B32F4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolFlash.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Flash);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolFlash.State.Flash:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Cooldown);
				return;
			}
			break;
		case GRToolFlash.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000B51DC File Offset: 0x000B33DC
	private void OnUpdateRemote(float dt)
	{
		GRToolFlash.State state = (GRToolFlash.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetStateAuthority(state);
		}
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000B5206 File Offset: 0x000B3406
	private void SetStateAuthority(GRToolFlash.State newState)
	{
		this.SetState(newState);
		GameEntityManager.instance.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000B5228 File Offset: 0x000B3428
	private void SetState(GRToolFlash.State newState)
	{
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolFlash.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolFlash.State.Flash:
			this.StartFlash();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolFlash.State.Cooldown:
			this.StopFlash();
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000B52AC File Offset: 0x000B34AC
	private void StartCharge()
	{
		this.audioSource.volume = this.chargeSoundVolume;
		this.audioSource.clip = this.chargeSound;
		this.audioSource.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000B5304 File Offset: 0x000B3504
	private void StartFlash()
	{
		this.flash.SetActive(true);
		this.audioSource.volume = this.flashSoundVolume;
		this.audioSource.clip = this.flashSound;
		this.audioSource.Play();
		this.timeLastFlashed = Time.time;
		this.tool.UseEnergy();
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 1f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 5f, this.enemyLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					GREnemyChaser component = attachedRigidbody.GetComponent<GREnemyChaser>();
					if (component != null)
					{
						component.TryFlashEnemy(this.tool);
					}
					GREnemyRanged component2 = attachedRigidbody.GetComponent<GREnemyRanged>();
					if (component2 != null)
					{
						component2.TryFlashEnemy(this.tool);
					}
					GRBarrierSpectral component3 = attachedRigidbody.GetComponent<GRBarrierSpectral>();
					if (component3 != null)
					{
						component3.TryFlash(this.tool);
					}
				}
			}
		}
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000B543E File Offset: 0x000B363E
	private void StopFlash()
	{
		this.flash.SetActive(false);
	}

	// Token: 0x06002403 RID: 9219 RVA: 0x000B544C File Offset: 0x000B364C
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.item.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000B54B0 File Offset: 0x000B36B0
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.item.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000B550A File Offset: 0x000B370A
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFlashed + this.cooldownMinimum);
	}

	// Token: 0x040028DA RID: 10458
	public GameEntity gameEntity;

	// Token: 0x040028DB RID: 10459
	public GRTool tool;

	// Token: 0x040028DC RID: 10460
	public GameObject flash;

	// Token: 0x040028DD RID: 10461
	public Transform shootFrom;

	// Token: 0x040028DE RID: 10462
	public LayerMask enemyLayerMask;

	// Token: 0x040028DF RID: 10463
	public AudioSource audioSource;

	// Token: 0x040028E0 RID: 10464
	public AudioClip chargeSound;

	// Token: 0x040028E1 RID: 10465
	public float chargeSoundVolume = 0.2f;

	// Token: 0x040028E2 RID: 10466
	public AudioClip flashSound;

	// Token: 0x040028E3 RID: 10467
	public float flashSoundVolume = 1f;

	// Token: 0x040028E4 RID: 10468
	public float chargeDuration = 0.75f;

	// Token: 0x040028E5 RID: 10469
	public float flashDuration = 0.1f;

	// Token: 0x040028E6 RID: 10470
	public float cooldownDuration;

	// Token: 0x040028E7 RID: 10471
	private float timeLastFlashed;

	// Token: 0x040028E8 RID: 10472
	private float cooldownMinimum = 0.35f;

	// Token: 0x040028E9 RID: 10473
	private bool activatedLocally;

	// Token: 0x040028EA RID: 10474
	public GameEntity item;

	// Token: 0x040028EB RID: 10475
	private GRToolFlash.State state;

	// Token: 0x040028EC RID: 10476
	private float stateTimeRemaining;

	// Token: 0x040028ED RID: 10477
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x020005C5 RID: 1477
	private enum State
	{
		// Token: 0x040028EF RID: 10479
		Idle,
		// Token: 0x040028F0 RID: 10480
		Charging,
		// Token: 0x040028F1 RID: 10481
		Flash,
		// Token: 0x040028F2 RID: 10482
		Cooldown,
		// Token: 0x040028F3 RID: 10483
		Count
	}
}

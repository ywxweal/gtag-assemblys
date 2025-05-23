using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005CA RID: 1482
[RequireComponent(typeof(GameEntity))]
public class GRToolRevive : MonoBehaviour
{
	// Token: 0x06002426 RID: 9254 RVA: 0x000B60C7 File Offset: 0x000B42C7
	private void Awake()
	{
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000B60D0 File Offset: 0x000B42D0
	private void OnEnable()
	{
		this.StopRevive();
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnDestroy()
	{
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000B60E0 File Offset: 0x000B42E0
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x000B610C File Offset: 0x000B430C
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolRevive.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Reviving);
				return;
			}
			break;
		case GRToolRevive.State.Reviving:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolRevive.State.Cooldown);
				return;
			}
			break;
		case GRToolRevive.State.Cooldown:
			if (!this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x000B6184 File Offset: 0x000B4384
	private void OnUpdateRemote(float dt)
	{
		GRToolRevive.State state = (GRToolRevive.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000B61AE File Offset: 0x000B43AE
	private void SetStateAuthority(GRToolRevive.State newState)
	{
		this.SetState(newState);
		GameEntityManager.instance.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000B61D0 File Offset: 0x000B43D0
	private void SetState(GRToolRevive.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (this.state == GRToolRevive.State.Reviving)
		{
			this.StopRevive();
		}
		this.state = newState;
		GRToolRevive.State state = this.state;
		if (state != GRToolRevive.State.Idle)
		{
			if (state == GRToolRevive.State.Reviving)
			{
				this.StartRevive();
				this.stateTimeRemaining = this.reviveDuration;
				return;
			}
		}
		else
		{
			this.stateTimeRemaining = -1f;
		}
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000B622C File Offset: 0x000B442C
	private void StartRevive()
	{
		this.reviveFx.SetActive(true);
		this.audioSource.volume = this.reviveSoundVolume;
		this.audioSource.clip = this.reviveSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		if (GameEntityManager.instance.IsAuthority())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.5f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, this.reviveDistance, this.playerLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
					if (component != null && component.State != GRPlayer.GRPlayerState.Alive)
					{
						GhostReactorManager.instance.RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Alive);
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000B6328 File Offset: 0x000B4528
	private void StopRevive()
	{
		this.reviveFx.SetActive(false);
		this.audioSource.Stop();
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000B6341 File Offset: 0x000B4541
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x000B635C File Offset: 0x000B455C
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

	// Token: 0x04002939 RID: 10553
	public GameEntity gameEntity;

	// Token: 0x0400293A RID: 10554
	public GRTool tool;

	// Token: 0x0400293B RID: 10555
	[SerializeField]
	private Transform shootFrom;

	// Token: 0x0400293C RID: 10556
	[SerializeField]
	private LayerMask playerLayerMask;

	// Token: 0x0400293D RID: 10557
	[SerializeField]
	private float reviveDistance = 1.5f;

	// Token: 0x0400293E RID: 10558
	[SerializeField]
	private GameObject reviveFx;

	// Token: 0x0400293F RID: 10559
	[SerializeField]
	private float reviveSoundVolume;

	// Token: 0x04002940 RID: 10560
	[SerializeField]
	private AudioClip reviveSound;

	// Token: 0x04002941 RID: 10561
	[SerializeField]
	private float reviveDuration = 0.75f;

	// Token: 0x04002942 RID: 10562
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002943 RID: 10563
	private GRToolRevive.State state;

	// Token: 0x04002944 RID: 10564
	private float stateTimeRemaining;

	// Token: 0x04002945 RID: 10565
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x020005CB RID: 1483
	private enum State
	{
		// Token: 0x04002947 RID: 10567
		Idle,
		// Token: 0x04002948 RID: 10568
		Reviving,
		// Token: 0x04002949 RID: 10569
		Cooldown
	}
}

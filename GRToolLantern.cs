using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005C6 RID: 1478
[RequireComponent(typeof(GameEntity))]
public class GRToolLantern : MonoBehaviour
{
	// Token: 0x06002407 RID: 9223 RVA: 0x000B55AE File Offset: 0x000B37AE
	private void Awake()
	{
		this.state = GRToolLantern.State.Off;
		this.gameEntity.OnStateChanged += this.OnStateChanged;
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x000B55CE File Offset: 0x000B37CE
	private void OnEnable()
	{
		this.TurnOff();
		this.state = GRToolLantern.State.Off;
	}

	// Token: 0x06002409 RID: 9225 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnDestroy()
	{
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x000B55E0 File Offset: 0x000B37E0
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.tool.energy > 0)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x000B5618 File Offset: 0x000B3818
	private void OnUpdateAuthority(float dt)
	{
		GRToolLantern.State state = this.state;
		if (state != GRToolLantern.State.Off)
		{
			if (state != GRToolLantern.State.On)
			{
				return;
			}
			this.timeOnSpentEnergy -= dt;
			if ((!this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f) || this.tool.energy <= 0)
			{
				this.SetState(GRToolLantern.State.Off);
				GameEntityManager.instance.RequestState(this.gameEntity.id, 0L);
				return;
			}
			if (this.IsButtonHeld() && this.timeOnSpentEnergy <= 0f)
			{
				this.TryConsumeEnergy();
			}
		}
		else if (this.IsButtonHeld() && this.tool.energy > 0)
		{
			this.SetState(GRToolLantern.State.On);
			GameEntityManager.instance.RequestState(this.gameEntity.id, 1L);
			return;
		}
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x000B56E0 File Offset: 0x000B38E0
	private void TryConsumeEnergy()
	{
		int num = Mathf.Min(this.tool.energy, this.minEnergyPerUse);
		if (num > 0)
		{
			this.tool.SetEnergy(this.tool.energy - num);
			this.timeOnSpentEnergy = this.fullChargeDurationSeconds * (float)num / (float)this.tool.maxEnergy;
		}
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x000B573C File Offset: 0x000B393C
	private void OnUpdateRemote(float dt)
	{
		GRToolLantern.State state = (GRToolLantern.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000B5768 File Offset: 0x000B3968
	private void SetState(GRToolLantern.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		GRToolLantern.State state = this.state;
		if (state != GRToolLantern.State.Off)
		{
			if (state == GRToolLantern.State.On)
			{
				this.TurnOn();
				return;
			}
		}
		else
		{
			this.TurnOff();
		}
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000B57AC File Offset: 0x000B39AC
	private void TurnOn()
	{
		this.gameLight.gameObject.SetActive(true);
		this.audioSource.PlayOneShot(this.turnOnSound, this.turnOnSoundVolume);
		this.meshRenderer.material = this.onMaterial;
		this.timeLastTurnedOn = Time.time;
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000B57FD File Offset: 0x000B39FD
	private void TurnOff()
	{
		this.gameLight.gameObject.SetActive(false);
		this.meshRenderer.material = this.offMaterial;
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000B5821 File Offset: 0x000B3A21
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000B583C File Offset: 0x000B3A3C
	private bool IsButtonHeld()
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return false;
		}
		if (!GamePlayer.IsLeftHand(num))
		{
			return gamePlayer.rig.rightIndex.calcT > 0.25f;
		}
		return gamePlayer.rig.leftIndex.calcT > 0.25f;
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnStateChanged(long prevState, long nextState)
	{
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000B58B4 File Offset: 0x000B3AB4
	public bool CanChangeState(long newStateIndex)
	{
		if (newStateIndex < 0L || newStateIndex >= 2L)
		{
			return false;
		}
		GRToolLantern.State state = (GRToolLantern.State)newStateIndex;
		if (state != GRToolLantern.State.Off)
		{
			return state == GRToolLantern.State.On && this.tool.energy > 0;
		}
		return Time.time > this.timeLastTurnedOn + this.minOnDuration || this.tool.energy <= 0;
	}

	// Token: 0x040028F4 RID: 10484
	public GameEntity gameEntity;

	// Token: 0x040028F5 RID: 10485
	public GRTool tool;

	// Token: 0x040028F6 RID: 10486
	public GameLight gameLight;

	// Token: 0x040028F7 RID: 10487
	[SerializeField]
	private float fullChargeDurationSeconds = 120f;

	// Token: 0x040028F8 RID: 10488
	[SerializeField]
	private int minEnergyPerUse = 1;

	// Token: 0x040028F9 RID: 10489
	[SerializeField]
	private float turnOnSoundVolume;

	// Token: 0x040028FA RID: 10490
	[SerializeField]
	private AudioClip turnOnSound;

	// Token: 0x040028FB RID: 10491
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040028FC RID: 10492
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x040028FD RID: 10493
	[SerializeField]
	private Material offMaterial;

	// Token: 0x040028FE RID: 10494
	[SerializeField]
	private Material onMaterial;

	// Token: 0x040028FF RID: 10495
	private float timeOnSpentEnergy;

	// Token: 0x04002900 RID: 10496
	private float timeLastTurnedOn;

	// Token: 0x04002901 RID: 10497
	private float minOnDuration = 0.5f;

	// Token: 0x04002902 RID: 10498
	private GRToolLantern.State state;

	// Token: 0x020005C7 RID: 1479
	private enum State
	{
		// Token: 0x04002904 RID: 10500
		Off,
		// Token: 0x04002905 RID: 10501
		On,
		// Token: 0x04002906 RID: 10502
		Count
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005B8 RID: 1464
public class GRPlayer : MonoBehaviour
{
	// Token: 0x17000368 RID: 872
	// (get) Token: 0x060023A9 RID: 9129 RVA: 0x000B3736 File Offset: 0x000B1936
	public GRPlayer.GRPlayerState State
	{
		get
		{
			return this.state;
		}
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000B3740 File Offset: 0x000B1940
	private void Awake()
	{
		this.vrRig = base.GetComponent<VRRig>();
		this.currency = 0;
		this.isEmployee = false;
		this.hp = this.maxHp;
		this.requestCollectItemLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestChargeToolLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestDepositCurrencyLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestShiftStartLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestToolPurchaseStationLimiter = new CallLimiter(25, 1f, 0.5f);
		this.applyEnemyHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportLocalHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportBreakableBrokenLimiter = new CallLimiter(25, 1f, 0.5f);
		this.playerStateChangeLimiter = new CallLimiter(25, 1f, 0.5f);
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000B3842 File Offset: 0x000B1A42
	public void OnPlayerHit()
	{
		if (this.hp <= 0)
		{
			return;
		}
		this.hp--;
		int num = this.hp;
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000B3868 File Offset: 0x000B1A68
	public void ChangePlayerState(GRPlayer.GRPlayerState newState)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			newState = GRPlayer.GRPlayerState.Alive;
		}
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState != GRPlayer.GRPlayerState.Alive)
		{
			if (grplayerState == GRPlayer.GRPlayerState.Ghost)
			{
				if (this.playerTurnedGhostEffect != null)
				{
					this.playerTurnedGhostEffect.Play();
				}
				this.playerTurnedGhostSoundBank.Play();
				GhostReactorManager.instance.ReportPlayerDeath();
			}
		}
		else
		{
			if (this.playerRevivedEffect != null)
			{
				this.playerRevivedEffect.Play();
			}
			if (this.audioSource != null && this.playerRevivedSound != null)
			{
				this.audioSource.PlayOneShot(this.playerRevivedSound, this.playerRevivedVolume);
			}
		}
		this.RefreshPlayerVisuals();
		if (this.vrRig.isLocal)
		{
			this.vrRigs.Clear();
			VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
			for (int i = 0; i < this.vrRigs.Count; i++)
			{
				this.vrRigs[i].GetComponent<GRPlayer>().RefreshPlayerVisuals();
			}
		}
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000B397C File Offset: 0x000B1B7C
	public void RefreshPlayerVisuals()
	{
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState != GRPlayer.GRPlayerState.Alive)
		{
			if (grplayerState != GRPlayer.GRPlayerState.Ghost)
			{
				return;
			}
			this.gamePlayer.DisableGrabbing(true);
			if (this.badge != null)
			{
				this.badge.Hide();
			}
			if (this.vrRig.isLocal)
			{
				this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
				this.vrRig.ChangeMaterialLocal(13);
				this.vrRig.SetInvisibleToLocalPlayer(false);
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(true);
				GameLightingManager.instance.SetDesaturateAndTintEnabled(true, this.deathTintColor);
				GameLightingManager.instance.SetAmbientLightDynamic(this.deathAmbientLightColor);
				return;
			}
			if (VRRigCache.Instance.localRig.GetComponent<GRPlayer>().State == GRPlayer.GRPlayerState.Ghost)
			{
				this.vrRig.ChangeMaterialLocal(13);
				this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
				this.vrRig.SetInvisibleToLocalPlayer(false);
				return;
			}
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Invisible);
			this.vrRig.SetInvisibleToLocalPlayer(true);
		}
		else
		{
			this.gamePlayer.DisableGrabbing(false);
			if (this.badge != null)
			{
				this.badge.UnHide();
			}
			this.vrRig.ChangeMaterialLocal(0);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			if (this.vrRig.isLocal)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
				GameLightingManager.instance.SetDesaturateAndTintEnabled(false, Color.black);
				GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
				return;
			}
		}
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000B3B1C File Offset: 0x000B1D1C
	public static GRPlayer Get(int actorNumber)
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(actorNumber);
		if (gamePlayer == null)
		{
			return null;
		}
		return gamePlayer.GetComponent<GRPlayer>();
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000B3B41 File Offset: 0x000B1D41
	public static GRPlayer Get(VRRig vrRig)
	{
		if (!(vrRig != null))
		{
			return null;
		}
		return vrRig.GetComponent<GRPlayer>();
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000B3B54 File Offset: 0x000B1D54
	public void AttachBadge(GRBadge grBadge)
	{
		this.badge = grBadge;
		this.badge.transform.SetParent(this.badgeBodyAnchor);
		this.badge.GetComponent<Rigidbody>().isKinematic = true;
		this.badge.StartRetracting();
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000B3B8F File Offset: 0x000B1D8F
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		writer.Write((byte)this.state);
		writer.Write(this.currency);
		writer.Write(this.isEmployee ? 1 : 0);
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000B3BC0 File Offset: 0x000B1DC0
	public static void DeserializeNetworkStateAndBurn(BinaryReader reader, GRPlayer player)
	{
		GRPlayer.GRPlayerState grplayerState = (GRPlayer.GRPlayerState)reader.ReadByte();
		int num = reader.ReadInt32();
		bool flag = reader.ReadByte() > 0;
		if (player != null)
		{
			player.ChangePlayerState(grplayerState);
			player.currency = num;
			player.isEmployee = flag;
		}
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000B3C04 File Offset: 0x000B1E04
	public void PlayHitFx(Vector3 attackLocation)
	{
		if (this.playerDamageAudioSource != null)
		{
			this.playerDamageAudioSource.PlayOneShot(this.playerDamageSound, this.playerDamageVolume);
		}
		if (this.playerDamageEffect != null && this.bodyCenter != null)
		{
			Vector3 vector = attackLocation - this.bodyCenter.position;
			vector.y = 0f;
			vector = vector.normalized * this.playerDamageOffsetDist;
			this.playerDamageEffect.transform.position = this.bodyCenter.position + vector;
			this.playerDamageEffect.Play();
		}
		if (this.gamePlayer == GamePlayerLocal.instance.gamePlayer)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, 0.5f);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, 0.5f);
		}
	}

	// Token: 0x04002865 RID: 10341
	public GamePlayer gamePlayer;

	// Token: 0x04002866 RID: 10342
	private GRPlayer.GRPlayerState state;

	// Token: 0x04002867 RID: 10343
	public int currency;

	// Token: 0x04002868 RID: 10344
	public bool isEmployee;

	// Token: 0x04002869 RID: 10345
	public AudioSource audioSource;

	// Token: 0x0400286A RID: 10346
	public ParticleSystem playerTurnedGhostEffect;

	// Token: 0x0400286B RID: 10347
	public SoundBankPlayer playerTurnedGhostSoundBank;

	// Token: 0x0400286C RID: 10348
	public ParticleSystem playerRevivedEffect;

	// Token: 0x0400286D RID: 10349
	public AudioClip playerRevivedSound;

	// Token: 0x0400286E RID: 10350
	public float playerRevivedVolume = 1f;

	// Token: 0x0400286F RID: 10351
	public AudioSource playerDamageAudioSource;

	// Token: 0x04002870 RID: 10352
	public Transform bodyCenter;

	// Token: 0x04002871 RID: 10353
	public ParticleSystem playerDamageEffect;

	// Token: 0x04002872 RID: 10354
	public float playerDamageVolume = 1f;

	// Token: 0x04002873 RID: 10355
	public AudioClip playerDamageSound;

	// Token: 0x04002874 RID: 10356
	public float playerDamageOffsetDist = 0.25f;

	// Token: 0x04002875 RID: 10357
	public Transform badgeBodyAnchor;

	// Token: 0x04002876 RID: 10358
	[SerializeField]
	private Transform badgeBodyStringAttach;

	// Token: 0x04002877 RID: 10359
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathTintColor;

	// Token: 0x04002878 RID: 10360
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathAmbientLightColor;

	// Token: 0x04002879 RID: 10361
	private VRRig vrRig;

	// Token: 0x0400287A RID: 10362
	private List<VRRig> vrRigs = new List<VRRig>();

	// Token: 0x0400287B RID: 10363
	[SerializeField]
	private int maxHp;

	// Token: 0x0400287C RID: 10364
	private int hp;

	// Token: 0x0400287D RID: 10365
	[HideInInspector]
	public GRBadge badge;

	// Token: 0x0400287E RID: 10366
	public CallLimiter requestCollectItemLimiter;

	// Token: 0x0400287F RID: 10367
	public CallLimiter requestChargeToolLimiter;

	// Token: 0x04002880 RID: 10368
	public CallLimiter requestDepositCurrencyLimiter;

	// Token: 0x04002881 RID: 10369
	public CallLimiter requestShiftStartLimiter;

	// Token: 0x04002882 RID: 10370
	public CallLimiter requestToolPurchaseStationLimiter;

	// Token: 0x04002883 RID: 10371
	public CallLimiter applyEnemyHitLimiter;

	// Token: 0x04002884 RID: 10372
	public CallLimiter reportLocalHitLimiter;

	// Token: 0x04002885 RID: 10373
	public CallLimiter reportBreakableBrokenLimiter;

	// Token: 0x04002886 RID: 10374
	public CallLimiter playerStateChangeLimiter;

	// Token: 0x020005B9 RID: 1465
	public enum GRPlayerState
	{
		// Token: 0x04002888 RID: 10376
		Alive,
		// Token: 0x04002889 RID: 10377
		Ghost
	}
}

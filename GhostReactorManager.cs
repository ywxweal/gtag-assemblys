using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000586 RID: 1414
[NetworkBehaviourWeaved(0)]
public class GhostReactorManager : NetworkComponent
{
	// Token: 0x0600227F RID: 8831 RVA: 0x000ACCC1 File Offset: 0x000AAEC1
	protected override void Awake()
	{
		base.Awake();
		GhostReactorManager.instance = this;
	}

	// Token: 0x06002280 RID: 8832 RVA: 0x000ACCCF File Offset: 0x000AAECF
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		this.gameEntityManager.onZoneStart += this.OnEntityZoneStart;
		this.gameEntityManager.onZoneClear += this.OnEntityZoneClear;
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000ACD0B File Offset: 0x000AAF0B
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		this.gameEntityManager.onZoneStart -= this.OnEntityZoneStart;
		this.gameEntityManager.onZoneClear -= this.OnEntityZoneClear;
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000ACD47 File Offset: 0x000AAF47
	private bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x000ACD54 File Offset: 0x000AAF54
	private bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x000ACD62 File Offset: 0x000AAF62
	private bool IsAuthorityPlayer(Player player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000ACD70 File Offset: 0x000AAF70
	private Player GetAuthorityPlayer()
	{
		return this.gameEntityManager.GetAuthorityPlayer();
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000ACD7D File Offset: 0x000AAF7D
	public bool IsZoneActive()
	{
		return this.gameEntityManager.IsZoneActive();
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000ACD8A File Offset: 0x000AAF8A
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.gameEntityManager.IsPositionInZone(pos);
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x000ACD98 File Offset: 0x000AAF98
	public bool IsValidClientRPC(Player sender)
	{
		return this.gameEntityManager.IsValidClientRPC(sender);
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x000ACDA6 File Offset: 0x000AAFA6
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x000ACDB5 File Offset: 0x000AAFB5
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x000ACDC5 File Offset: 0x000AAFC5
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000ACDD4 File Offset: 0x000AAFD4
	public bool IsValidAuthorityRPC()
	{
		return this.gameEntityManager.IsValidAuthorityRPC();
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000ACDE1 File Offset: 0x000AAFE1
	public bool IsValidAuthorityRPC(int entityNetId)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(entityNetId);
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000ACDEF File Offset: 0x000AAFEF
	public bool IsValidAuthorityRPC(int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(entityNetId, pos);
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000ACDFE File Offset: 0x000AAFFE
	public bool IsValidAuthorityRPC(Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(pos);
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x000ACE0C File Offset: 0x000AB00C
	public void RequestCollectItem(GameEntityId collectibleEntityId, GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestCollectItemRPC", this.GetAuthorityPlayer(), new object[]
		{
			GameEntity.GetNetId(collectibleEntityId),
			GameEntity.GetNetId(collectorEntityId)
		});
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x000ACE48 File Offset: 0x000AB048
	public void RequestDepositCollectible(GameEntityId collectibleEntityId)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				GameEntity.GetNetId(collectibleEntityId),
				-1,
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000ACEB0 File Offset: 0x000AB0B0
	[PunRPC]
	public void RequestCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(collectibleEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestCollectItemLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!GameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsEntityNearEntity(collectibleEntityNetId, collectorEntityNetId, 16f))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				collectibleEntityNetId,
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x000ACF50 File Offset: 0x000AB150
	[PunRPC]
	public void ApplyCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyCollectItem))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(collectingPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId idFromNetId = GameEntity.GetIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(idFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			GameEntityId idFromNetId2 = GameEntity.GetIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(idFromNetId2);
			if (gameEntity2 != null)
			{
				GRToolCollector component2 = gameEntity2.GetComponent<GRToolCollector>();
				if (component2 != null && component2.tool != null)
				{
					component2.PerformCollection(component);
					this.ReportCoreCollection();
				}
			}
			else
			{
				grplayer.currency += component.energyValue;
				this.reactor.RefreshScoreboards();
				this.ReportCoreCollection();
			}
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(idFromNetId);
		}
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x000AD06B File Offset: 0x000AB26B
	public void RequestChargeTool(GameEntityId collectorEntityId, GameEntityId targetToolId)
	{
		this.photonView.RPC("RequestChargeToolRPC", this.GetAuthorityPlayer(), new object[]
		{
			GameEntity.GetNetId(collectorEntityId),
			GameEntity.GetNetId(targetToolId)
		});
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x000AD0A8 File Offset: 0x000AB2A8
	[PunRPC]
	public void RequestChargeToolRPC(int collectorEntityNetId, int targetToolNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC() || !GameEntityManager.IsValidNetId(collectorEntityNetId) || !GameEntityManager.IsValidNetId(targetToolNetId) || !this.gameEntityManager.IsEntityNearEntity(collectorEntityNetId, targetToolNetId, 16f) || !this.gameEntityManager.IsPlayerHandNearEntity(GamePlayer.GetGamePlayer(info.Sender.ActorNumber), collectorEntityNetId, false, true, 16f) || !this.gameEntityManager.IsPlayerHandNearEntity(GamePlayer.GetGamePlayer(info.Sender.ActorNumber), targetToolNetId, false, true, 16f))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestChargeToolLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyChargeToolRPC", RpcTarget.All, new object[] { collectorEntityNetId, targetToolNetId, info.Sender });
		}
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000AD190 File Offset: 0x000AB390
	[PunRPC]
	public void ApplyChargeToolRPC(int collectorEntityNetId, int targetToolNetId, Player collectingPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyChargeTool) || !GameEntityManager.IsValidNetId(collectorEntityNetId) || !GameEntityManager.IsValidNetId(targetToolNetId))
		{
			return;
		}
		if (true)
		{
			GameEntityId idFromNetId = GameEntity.GetIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(idFromNetId);
			GameEntityId idFromNetId2 = GameEntity.GetIdFromNetId(targetToolNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(idFromNetId2);
			if (gameEntity != null && gameEntity2 != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				GRTool component2 = gameEntity2.GetComponent<GRTool>();
				if (component != null && component.tool != null && component2 != null)
				{
					int num = Mathf.Max(component2.maxEnergy - component2.energy, 0);
					int num2 = Mathf.Min(Mathf.Min(component.tool.energy, 100), num);
					if (num2 > 0)
					{
						component.tool.SetEnergy(component.tool.energy - num2);
						component2.RefillEnergy(num2);
						component.PlayChargeEffect(component2);
					}
				}
			}
		}
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000AD2A9 File Offset: 0x000AB4A9
	public void RequestDepositCurrency(GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestDepositCurrencyRPC", this.GetAuthorityPlayer(), new object[] { GameEntity.GetNetId(collectorEntityId) });
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000AD2D8 File Offset: 0x000AB4D8
	[PunRPC]
	public void RequestDepositCurrencyRPC(int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(collectorEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestDepositCurrencyLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(collectorEntityNetId);
		this.gameEntityManager.GetGameEntity(idFromNetId);
		if (this.gameEntityManager.IsPlayerHandNearEntity(GamePlayer.GetGamePlayer(info.Sender.ActorNumber), collectorEntityNetId, false, true, 16f) & ((grplayer.transform.position - GhostReactor.instance.currencyDepositor.transform.position).magnitude < 16f))
		{
			this.photonView.RPC("ApplyDepositCurrencyRPC", RpcTarget.All, new object[]
			{
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x000AD3BC File Offset: 0x000AB5BC
	[PunRPC]
	public void ApplyDepositCurrencyRPC(int collectorEntityNetId, int targetPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectorEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyDepositCurrency))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(targetPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId idFromNetId = GameEntity.GetIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(idFromNetId);
			if (gameEntity != null && grplayer != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				if (component != null && component.tool != null)
				{
					int energy = component.tool.energy;
					if (energy > 0)
					{
						grplayer.currency += energy;
						component.tool.SetEnergy(0);
						this.reactor.RefreshScoreboards();
						component.PlayChargeEffect(this.reactor.currencyDepositor);
					}
				}
			}
		}
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x000AD499 File Offset: 0x000AB699
	public void RequestEnemyHitPlayer(GhostReactor.EnemyType type, GameEntityId hitByEntityId, GRPlayer player)
	{
		this.photonView.RPC("ApplyEnemyHitPlayerRPC", RpcTarget.All, new object[]
		{
			type,
			GameEntity.GetNetId(hitByEntityId)
		});
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x000AD4CC File Offset: 0x000AB6CC
	[PunRPC]
	private void ApplyEnemyHitPlayerRPC(GhostReactor.EnemyType type, int entityNetId, PhotonMessageInfo info)
	{
		if (!GameEntityManager.IsValidNetId(entityNetId))
		{
			return;
		}
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(entityNetId);
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.applyEnemyHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.OnEnemyHitPlayerInternal(type, idFromNetId, grplayer);
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000AD520 File Offset: 0x000AB720
	private void OnEnemyHitPlayerInternal(GhostReactor.EnemyType type, GameEntityId entityId, GRPlayer player)
	{
		if (type == GhostReactor.EnemyType.Chaser)
		{
			GREnemyChaser grenemyChaser = GREnemyChaser.Get(entityId);
			if (grenemyChaser != null)
			{
				grenemyChaser.HitPlayer(player);
				return;
			}
		}
		else if (type == GhostReactor.EnemyType.Ranged)
		{
			GREnemyRanged grenemyRanged = GREnemyRanged.Get(entityId);
			if (grenemyRanged != null)
			{
				grenemyRanged.HitPlayer(player);
			}
		}
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000AD563 File Offset: 0x000AB763
	public void ReportLocalPlayerHit()
	{
		base.GetView.RPC("ReportLocalPlayerHitRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000AD57C File Offset: 0x000AB77C
	[PunRPC]
	private void ReportLocalPlayerHitRPC(PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.reportLocalHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000AD5C0 File Offset: 0x000AB7C0
	public void RequestPlayerRevive(GRReviveStation reviveStation, GRPlayer player)
	{
		if ((NetworkSystem.Instance.InRoom && this.IsAuthority()) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("ApplyPlayerRevivedRPC", RpcTarget.All, new object[]
			{
				reviveStation.Index,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber
			});
		}
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000AD630 File Offset: 0x000AB830
	[PunRPC]
	private void ApplyPlayerRevivedRPC(int reviveStationIndex, int playerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyPlayerRevived))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (reviveStationIndex < 0 || reviveStationIndex >= this.reactor.reviveStations.Count)
		{
			return;
		}
		GRReviveStation grreviveStation = this.reactor.reviveStations[reviveStationIndex];
		if (grreviveStation == null)
		{
			return;
		}
		grreviveStation.RevivePlayer(grplayer);
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000AD6A8 File Offset: 0x000AB8A8
	public void RequestPlayerStateChange(GRPlayer player, GRPlayer.GRPlayerState newState)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("PlayerStateChangeRPC", RpcTarget.All, new object[]
			{
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
				(int)newState
			});
			return;
		}
		player.ChangePlayerState(newState);
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000AD708 File Offset: 0x000AB908
	[PunRPC]
	private void PlayerStateChangeRPC(int playerActorNumber, int newState, PhotonMessageInfo info)
	{
		bool flag = this.IsValidClientRPC(info.Sender);
		bool flag2 = newState == 1 && info.Sender.ActorNumber == playerActorNumber;
		bool flag3 = newState == 0 && flag;
		if (!flag2 && !flag3)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || grplayer2.IsNull() || !grplayer2.playerStateChangeLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.ChangePlayerState((GRPlayer.GRPlayerState)newState);
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x000AD78C File Offset: 0x000AB98C
	public void RequestFireProjectile(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if ((NetworkSystem.Instance.InRoom && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("RequestFireProjectileRPC", RpcTarget.All, new object[]
			{
				GameEntity.GetNetId(entityId),
				firingPosition,
				targetPosition,
				networkTime
			});
		}
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000AD804 File Offset: 0x000ABA04
	[PunRPC]
	private void RequestFireProjectileRPC(int entityNetId, Vector3 firingPosition, Vector3 targetPosition, double networkTime, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId, targetPosition) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RequestFireProjectile) || !this.gameEntityManager.IsEntityNearPosition(entityNetId, firingPosition, 16f))
		{
			return;
		}
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(entityNetId);
		this.OnRequestFireProjectileInternal(idFromNetId, firingPosition, targetPosition, networkTime);
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000AD858 File Offset: 0x000ABA58
	private void OnRequestFireProjectileInternal(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		GREnemyRanged grenemyRanged = GREnemyRanged.Get(entityId);
		if (grenemyRanged != null)
		{
			grenemyRanged.RequestRangedAttack(firingPosition, targetPosition, networkTime);
		}
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000AD87F File Offset: 0x000ABA7F
	public void RequestShiftStart()
	{
		this.photonView.RPC("RequestShiftStartRPC", this.GetAuthorityPlayer(), Array.Empty<object>());
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x000AD89C File Offset: 0x000ABA9C
	[PunRPC]
	public void RequestShiftStartRPC(PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			double time = PhotonNetwork.Time;
			int num = Random.Range(levelGenerator.MinSections, levelGenerator.MaxSections + 1);
			int num2 = Random.Range(0, int.MaxValue);
			this.photonView.RPC("ApplyShiftStartRPC", RpcTarget.All, new object[] { time, num, num2 });
		}
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000AD968 File Offset: 0x000ABB68
	[PunRPC]
	public void ApplyShiftStartRPC(double shiftStartTime, int sectionsToSpawn, int randomSeed, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		double num = PhotonNetwork.Time - shiftStartTime;
		if (sectionsToSpawn < levelGenerator.MinSections || sectionsToSpawn > levelGenerator.MaxSections || num < 0.0 || num > 10.0)
		{
			return;
		}
		levelGenerator.GenerateRandomLevelConfiguration(sectionsToSpawn, randomSeed);
		levelGenerator.SpawnSectionsBasedOnLevelGenerationConfig();
		if (this.gameEntityManager.IsAuthority())
		{
			levelGenerator.SpawnEntitiesInEachSection();
		}
		shiftManager.EnemyDeaths = 0;
		shiftManager.CoresCollected = 0;
		shiftManager.PlayerDeaths = 0;
		shiftManager.ResetJudgment();
		shiftManager.RefreshShiftStatsDisplay();
		shiftManager.OnShiftStarted(shiftStartTime);
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x000ADA38 File Offset: 0x000ABC38
	public void RequestShiftEnd()
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		GhostReactorManager.tempEntitiesToDestroy.Clear();
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			GameEntity gameEntity = gameEntities[i];
			if (gameEntity != null && !this.ShouldEntitySurviveShift(gameEntity))
			{
				GhostReactorManager.tempEntitiesToDestroy.Add(gameEntity.id);
			}
		}
		this.gameEntityManager.RequestDestroyItems(GhostReactorManager.tempEntitiesToDestroy);
		this.photonView.RPC("ApplyShiftEndRPC", RpcTarget.Others, Array.Empty<object>());
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.EnemyDeaths / 5f));
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000ADB20 File Offset: 0x000ABD20
	[PunRPC]
	public void ApplyShiftEndRPC(PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftEnd))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.EnemyDeaths / 5f));
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x000ADBA0 File Offset: 0x000ABDA0
	private bool ShouldEntitySurviveShift(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return true;
		}
		if (this.reactor == null)
		{
			return false;
		}
		if (gameEntity.GetComponent<GREnemyChaser>() != null || gameEntity.GetComponent<GREnemyRanged>() != null)
		{
			return false;
		}
		Collider safeZoneLimit = this.reactor.safeZoneLimit;
		Vector3 position = gameEntity.gameObject.transform.position;
		return safeZoneLimit.bounds.Contains(position) || gameEntity.GetComponent<GRBadge>() != null;
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x000ADC28 File Offset: 0x000ABE28
	public void ReportEnemyDeath()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.EnemyDeaths++;
		shiftManager.RefreshShiftStatsDisplay();
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x000ADC57 File Offset: 0x000ABE57
	public void ReportCoreCollection()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.CoresCollected++;
		shiftManager.RefreshShiftStatsDisplay();
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000ADC86 File Offset: 0x000ABE86
	public void ReportPlayerDeath()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.PlayerDeaths++;
		shiftManager.RefreshShiftStatsDisplay();
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000ADCB5 File Offset: 0x000ABEB5
	public void ToolPurchaseStationRequest(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action)
	{
		this.photonView.RPC("ToolPurchaseStationRequestRPC", this.GetAuthorityPlayer(), new object[] { stationIndex, action });
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000ADCE8 File Offset: 0x000ABEE8
	[PunRPC]
	public void ToolPurchaseStationRequestRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidAuthorityRPC() || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestToolPurchaseStationLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (action)
		{
		case GhostReactorManager.ToolPurchaseStationAction.ShiftLeft:
			grtoolPurchaseStation.ShiftLeftAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.ShiftRight:
			grtoolPurchaseStation.ShiftRightAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.TryPurchase:
		{
			bool flag = false;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetNetPlayerByID(info.Sender.ActorNumber), out rigContainer))
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				int num;
				if (component != null && grtoolPurchaseStation.TryPurchaseAuthority(component, out num))
				{
					this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
					{
						stationIndex,
						GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded,
						info.Sender.ActorNumber,
						num
					});
					this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded, info.Sender.ActorNumber, num);
					flag = true;
				}
			}
			if (!flag)
			{
				this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
				{
					stationIndex,
					GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed,
					info.Sender.ActorNumber,
					0
				});
				this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed, info.Sender.ActorNumber, 0);
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000ADF28 File Offset: 0x000AC128
	[PunRPC]
	public void ToolPurchaseStationResponseRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidClientRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolPurchaseResponse))
		{
			return;
		}
		this.ToolPurchaseResponseLocal(stationIndex, responseType, dataA, dataB);
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000ADF88 File Offset: 0x000AC188
	private void ToolPurchaseResponseLocal(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (responseType)
		{
		case GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate:
			grtoolPurchaseStation.OnSelectionUpdate(dataA);
			return;
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded:
		{
			grtoolPurchaseStation.OnPurchaseSucceeded();
			GRPlayer grplayer = GRPlayer.Get(dataA);
			if (grplayer != null)
			{
				grplayer.currency = Mathf.Max(grplayer.currency - dataB, 0);
				this.reactor.RefreshScoreboards();
				return;
			}
			break;
		}
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed:
			grtoolPurchaseStation.OnPurchaseFailed();
			break;
		default:
			return;
		}
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000AE028 File Offset: 0x000AC228
	public void ReportBreakableBroken(GameEntity breakableEntity, GameEntity tool = null)
	{
		this.photonView.RPC("ReportBreakableBrokenRPC", this.GetAuthorityPlayer(), new object[]
		{
			GameEntity.GetNetId(breakableEntity.id),
			(tool != null) ? GameEntity.GetNetId(tool.id) : (-1)
		});
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000AE084 File Offset: 0x000AC284
	[PunRPC]
	public void ReportBreakableBrokenRPC(int breakableEntityNetId, int toolNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(breakableEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.reportBreakableBrokenLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(breakableEntityNetId);
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(idFromNetId);
		GameEntityId idFromNetId2 = GameEntity.GetIdFromNetId(toolNetId);
		GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(idFromNetId2);
		if (gameEntity == null)
		{
			return;
		}
		GRBreakable component = gameEntity.GetComponent<GRBreakable>();
		GRTool grtool = ((gameEntity2 != null) ? gameEntity2.GetComponent<GRTool>() : null);
		bool flag = (int)gameEntity.GetState() == 1;
		if (component != null && !flag && (grtool == null || (grtool.energy > grtool.useCost && this.gameEntityManager.IsEntityNearEntity(grtool.gameEntity.id.GetNetId(), component.gameEntity.id.GetNetId(), 16f))))
		{
			this.photonView.RPC("ApplyBreakableBrokenRPC", RpcTarget.All, new object[] { breakableEntityNetId, toolNetId });
			GameEntityManager.instance.RequestState(idFromNetId, 1L);
			GameEntity gameEntity3;
			if (component.holdsRandomItem && component.itemSpawnProbability.TryForRandomItem(out gameEntity3))
			{
				this.gameEntityManager.RequestCreateItem(gameEntity3.gameObject.name.GetStaticHash(), component.itemSpawnLocation.position, component.itemSpawnLocation.rotation, 0L);
			}
		}
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x000AE214 File Offset: 0x000AC414
	[PunRPC]
	public void ApplyBreakableBrokenRPC(int breakableEntityNetId, int toolNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, breakableEntityNetId) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyBreakableBroken))
		{
			return;
		}
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(breakableEntityNetId);
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(idFromNetId);
		if (gameEntity == null)
		{
			return;
		}
		GRBreakable component = gameEntity.GetComponent<GRBreakable>();
		if (component != null)
		{
			component.BreakLocal();
		}
		GameEntityId idFromNetId2 = GameEntity.GetIdFromNetId(toolNetId);
		GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(idFromNetId2);
		if (gameEntity2 == null)
		{
			return;
		}
		GRTool component2 = gameEntity2.GetComponent<GRTool>();
		if (component2 != null)
		{
			component2.UseEnergy();
		}
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000AE2B0 File Offset: 0x000AC4B0
	public void EntityEnteredDropZone(GameEntity entity)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRUIStationEmployeeBadges employeeBadges = this.reactor.employeeBadges;
		long num = BitPackUtils.PackWorldPosForNetwork(entity.transform.position);
		int num2 = BitPackUtils.PackQuaternionForNetwork(entity.transform.rotation);
		if (entity.gameObject.GetComponent<GRBadge>() != null)
		{
			GRUIEmployeeBadgeDispenser gruiemployeeBadgeDispenser = employeeBadges.badgeDispensers[entity.gameObject.GetComponent<GRBadge>().dispenserIndex];
			if (gruiemployeeBadgeDispenser != null)
			{
				num = BitPackUtils.PackWorldPosForNetwork(gruiemployeeBadgeDispenser.GetSpawnPosition());
				num2 = BitPackUtils.PackQuaternionForNetwork(gruiemployeeBadgeDispenser.GetSpawnRotation());
			}
		}
		this.photonView.RPC("EntityEnteredDropZoneRPC", RpcTarget.All, new object[]
		{
			GameEntity.GetNetId(entity.id),
			num,
			num2
		});
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000AE390 File Offset: 0x000AC590
	[PunRPC]
	public void EntityEnteredDropZoneRPC(int entityNetId, long position, int rotation, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.EntityEnteredDropZone))
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "EntityEnteredDropZoneRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(position);
		float num = 10000f;
		if (!(in vector).IsValid(in num))
		{
			return;
		}
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
		if (!(in quaternion).IsValid())
		{
			return;
		}
		if (!this.IsPositionInZone(vector))
		{
			return;
		}
		if ((vector - GhostReactor.instance.dropZone.transform.position).magnitude > 5f)
		{
			return;
		}
		this.LocalEntityEnteredDropZone(GameEntity.GetIdFromNetId(entityNetId), vector, quaternion);
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000AE438 File Offset: 0x000AC638
	private void LocalEntityEnteredDropZone(GameEntityId entityId, Vector3 position, Quaternion rotation)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRDropZone dropZone = this.reactor.dropZone;
		Vector3 vector = dropZone.GetRepelDirectionWorld() * GhostReactor.DROP_ZONE_REPEL;
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityId);
		if (gameEntity.heldByActorNumber >= 0)
		{
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
			int num = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(num);
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		if (!(gameEntity.gameObject.GetComponent<GRBadge>() != null))
		{
			Rigidbody component = gameEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.position = position;
				component.rotation = rotation;
				component.velocity = vector;
				component.angularVelocity = Vector3.zero;
			}
		}
		dropZone.PlayEffect();
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000AE557 File Offset: 0x000AC757
	protected void OnNewPlayerEnteredGhostReactor()
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.VRRigRefresh();
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000AE573 File Offset: 0x000AC773
	private void OnEntityZoneStart(GTZone zoneId)
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.VRRigRefresh();
		if (this.reactor.employeeTerminal != null)
		{
			this.reactor.employeeTerminal.Setup();
		}
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x000AE5B2 File Offset: 0x000AC7B2
	public void OnEntityZoneClear(GTZone zoneId)
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.levelGenerator.ClearLevelSections();
		this.reactor.shiftManager.OnShiftEnded();
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040026AD RID: 9901
	public const int GHOSTREACTOR_ZONE_ID = 5;

	// Token: 0x040026AE RID: 9902
	public const GTZone GT_ZONE_GHOSTREACTOR = GTZone.ghostReactor;

	// Token: 0x040026AF RID: 9903
	public GameEntityManager gameEntityManager;

	// Token: 0x040026B0 RID: 9904
	public GameAgentManager gameAgentManager;

	// Token: 0x040026B1 RID: 9905
	public static GhostReactorManager instance;

	// Token: 0x040026B2 RID: 9906
	public PhotonView photonView;

	// Token: 0x040026B3 RID: 9907
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x040026B4 RID: 9908
	public CallLimitersList<CallLimiter, GhostReactorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GhostReactorManager.RPC>();

	// Token: 0x040026B5 RID: 9909
	private static List<GameEntityId> tempEntitiesToDestroy = new List<GameEntityId>();

	// Token: 0x02000587 RID: 1415
	public enum RPC
	{
		// Token: 0x040026B7 RID: 9911
		ApplyCollectItem,
		// Token: 0x040026B8 RID: 9912
		ApplyChargeTool,
		// Token: 0x040026B9 RID: 9913
		ApplyDepositCurrency,
		// Token: 0x040026BA RID: 9914
		ApplyPlayerRevived,
		// Token: 0x040026BB RID: 9915
		RequestFireProjectile,
		// Token: 0x040026BC RID: 9916
		ApplyShiftStart,
		// Token: 0x040026BD RID: 9917
		ApplyShiftEnd,
		// Token: 0x040026BE RID: 9918
		ToolPurchaseResponse,
		// Token: 0x040026BF RID: 9919
		ApplyBreakableBroken,
		// Token: 0x040026C0 RID: 9920
		EntityEnteredDropZone
	}

	// Token: 0x02000588 RID: 1416
	public enum ToolPurchaseStationAction
	{
		// Token: 0x040026C2 RID: 9922
		ShiftLeft,
		// Token: 0x040026C3 RID: 9923
		ShiftRight,
		// Token: 0x040026C4 RID: 9924
		TryPurchase
	}

	// Token: 0x02000589 RID: 1417
	public enum ToolPurchaseStationResponse
	{
		// Token: 0x040026C6 RID: 9926
		SelectionUpdate,
		// Token: 0x040026C7 RID: 9927
		PurchaseSucceeded,
		// Token: 0x040026C8 RID: 9928
		PurchaseFailed
	}
}

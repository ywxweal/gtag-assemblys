using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200055E RID: 1374
[NetworkBehaviourWeaved(0)]
public class GameAgentManager : NetworkComponent
{
	// Token: 0x06002148 RID: 8520 RVA: 0x000A6940 File Offset: 0x000A4B40
	protected override void Awake()
	{
		GameAgentManager.instance = this;
		this.agents = new List<GameAgent>(128);
		this.netIdsForDestination = new List<int>();
		this.destinationsForDestination = new List<Vector3>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<byte>();
		this.netIdsForBehavior = new List<int>();
		this.behaviorsForBehavior = new List<byte>();
		this.nextAgentIndexUpdate = 0;
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000A69AE File Offset: 0x000A4BAE
	public void AddGameAgent(GameAgent gameAgent)
	{
		this.agents.Add(gameAgent);
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x000A69BC File Offset: 0x000A4BBC
	public void RemoveGameAgent(GameAgent gameAgent)
	{
		this.agents.Remove(gameAgent);
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x000A69CB File Offset: 0x000A4BCB
	public GameAgent GetGameAgent(GameEntityId id)
	{
		return this.entityManager.GetGameEntity(id).GetComponent<GameAgent>();
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x000A69E0 File Offset: 0x000A4BE0
	public void Update()
	{
		for (int i = 0; i < this.agents.Count; i++)
		{
			if (this.agents[i] != null)
			{
				this.agents[i].OnThink();
			}
		}
		if (this.IsAuthority())
		{
			if (this.netIdsForDestination.Count > 0 && Time.time > this.lastDestinationSentTime + this.destinationCooldown)
			{
				this.lastDestinationSentTime = Time.time;
				base.SendRPC("ApplyDestinationRPC", RpcTarget.All, new object[]
				{
					this.netIdsForDestination.ToArray(),
					this.destinationsForDestination.ToArray()
				});
				this.netIdsForDestination.Clear();
				this.destinationsForDestination.Clear();
			}
			if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSentTime + this.stateCooldown)
			{
				this.lastStateSentTime = Time.time;
				base.SendRPC("ApplyStateRPC", RpcTarget.All, new object[]
				{
					this.netIdsForState.ToArray(),
					this.statesForState.ToArray()
				});
				this.netIdsForState.Clear();
				this.statesForState.Clear();
			}
			if (this.netIdsForBehavior.Count > 0 && Time.time > this.lastBehaviorSentTime + this.behaviorCooldown)
			{
				this.lastBehaviorSentTime = Time.time;
				base.SendRPC("ApplyBehaviorRPC", RpcTarget.All, new object[]
				{
					this.netIdsForBehavior.ToArray(),
					this.behaviorsForBehavior.ToArray()
				});
				this.netIdsForBehavior.Clear();
				this.behaviorsForBehavior.Clear();
			}
		}
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x000A6B86 File Offset: 0x000A4D86
	public bool IsAuthority()
	{
		return this.entityManager.IsAuthority();
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000A6B93 File Offset: 0x000A4D93
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000A6BA1 File Offset: 0x000A4DA1
	public bool IsAuthorityPlayer(Player player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x000A6BAF File Offset: 0x000A4DAF
	public Player GetAuthorityPlayer()
	{
		return this.entityManager.GetAuthorityPlayer();
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x000A6BBC File Offset: 0x000A4DBC
	public bool IsZoneActive()
	{
		return this.entityManager.IsZoneActive();
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000A6BC9 File Offset: 0x000A4DC9
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.entityManager.IsPositionInZone(pos);
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000A6BD7 File Offset: 0x000A4DD7
	public bool IsValidClientRPC(Player sender)
	{
		return this.entityManager.IsValidClientRPC(sender);
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000A6BE5 File Offset: 0x000A4DE5
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000A6BF4 File Offset: 0x000A4DF4
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x000A6C04 File Offset: 0x000A4E04
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000A6C13 File Offset: 0x000A4E13
	public bool IsValidAuthorityRPC()
	{
		return this.entityManager.IsValidAuthorityRPC();
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000A6C20 File Offset: 0x000A4E20
	public bool IsValidAuthorityRPC(int entityNetId)
	{
		return this.entityManager.IsValidAuthorityRPC(entityNetId);
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x000A6C2E File Offset: 0x000A4E2E
	public bool IsValidAuthorityRPC(int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(entityNetId, pos);
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x000A6C3D File Offset: 0x000A4E3D
	public bool IsValidAuthorityRPC(Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(pos);
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000A6C4C File Offset: 0x000A4E4C
	public void RequestDestination(GameAgent agent, Vector3 dest)
	{
		if (!this.IsAuthority())
		{
			Debug.LogError("RequestDestination should only be called from the master client");
			return;
		}
		int netId = GameEntity.GetNetId(agent.entity.id);
		if (this.netIdsForDestination.Contains(netId))
		{
			this.destinationsForDestination[this.netIdsForDestination.IndexOf(netId)] = dest;
			return;
		}
		this.netIdsForDestination.Add(netId);
		this.destinationsForDestination.Add(dest);
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000A6CBC File Offset: 0x000A4EBC
	[PunRPC]
	public void ApplyDestinationRPC(int[] netEntityId, Vector3[] dest, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyDestination))
		{
			return;
		}
		if (netEntityId.Length != dest.Length)
		{
			return;
		}
		int i = 0;
		while (i < netEntityId.Length)
		{
			if (this.IsValidClientRPC(info.Sender, netEntityId[i], dest[i]))
			{
				int num = i;
				float num2 = 10000f;
				if ((in dest[num]).IsValid(in num2))
				{
					i++;
					continue;
				}
			}
			return;
		}
		for (int j = 0; j < netEntityId.Length; j++)
		{
			GameEntity gameEntity = this.entityManager.GetGameEntity(GameEntity.GetIdFromNetId(netEntityId[j]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.ApplyDestination(dest[j]);
		}
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000A6D74 File Offset: 0x000A4F74
	public void RequestState(GameAgent agent, byte state)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netId = GameEntity.GetNetId(agent.entity.id);
		if (this.netIdsForState.Contains(netId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netId)] = state;
			return;
		}
		this.netIdsForState.Add(netId);
		this.statesForState.Add(state);
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000A6DDC File Offset: 0x000A4FDC
	[PunRPC]
	public void ApplyStateRPC(int[] netEntityId, byte[] state, PhotonMessageInfo info)
	{
		if (netEntityId.Length != state.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(GameEntity.GetIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.OnBodyStateChanged(state[i]);
		}
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000A6E58 File Offset: 0x000A5058
	public void RequestBehavior(GameAgent agent, byte behavior)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netId = GameEntity.GetNetId(agent.entity.id);
		if (this.netIdsForBehavior.Contains(netId))
		{
			this.behaviorsForBehavior[this.netIdsForBehavior.IndexOf(netId)] = behavior;
			return;
		}
		this.netIdsForBehavior.Add(netId);
		this.behaviorsForBehavior.Add(behavior);
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000A6EC0 File Offset: 0x000A50C0
	[PunRPC]
	public void ApplyBehaviorRPC(int[] netEntityId, byte[] behavior, PhotonMessageInfo info)
	{
		if (netEntityId.Length != behavior.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyBehaviour))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(GameEntity.GetIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component != null)
			{
				component.OnBehaviorStateChanged(behavior[i]);
			}
		}
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x000A6F3C File Offset: 0x000A513C
	public void RequestImpact(GameEntity target, GRTool tool, Vector3 startPos, Vector3 impulse, byte impulseData)
	{
		GREnemyChaser component = target.GetComponent<GREnemyChaser>();
		if (component != null)
		{
			component.OnImpact(tool, startPos, impulse, impulseData);
		}
		GREnemyRanged component2 = target.GetComponent<GREnemyRanged>();
		if (component2 != null)
		{
			component2.OnImpact(tool, startPos, impulse, impulseData);
		}
		GRBarrierSpectral component3 = target.GetComponent<GRBarrierSpectral>();
		if (component3 != null)
		{
			component3.OnImpact(tool, startPos, impulse, impulseData);
		}
		int num = ((tool.gameEntity != null) ? GameEntity.GetNetId(tool.gameEntity.id) : (-1));
		base.SendRPC("RequestImpactRPC", this.GetAuthorityPlayer(), new object[]
		{
			GameEntity.GetNetId(target.id),
			num,
			startPos,
			impulse,
			impulseData
		});
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000A7014 File Offset: 0x000A5214
	[PunRPC]
	public void RequestImpactRPC(int enemyNetId, int toolNetId, Vector3 startPos, Vector3 impulse, byte impulseData, PhotonMessageInfo info)
	{
		float num = 10000f;
		if ((in startPos).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in impulse).IsValid(in num2) && this.IsValidAuthorityRPC(enemyNetId, startPos))
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
				if (gamePlayer == null || !gamePlayer.netImpulseLimiter.CheckCallTime(Time.time))
				{
					return;
				}
				base.SendRPC("ApplyImpactRPC", RpcTarget.All, new object[] { enemyNetId, toolNetId, startPos, impulse, impulseData, info.Sender });
				return;
			}
		}
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x000A70C4 File Offset: 0x000A52C4
	[PunRPC]
	public void ApplyImpactRPC(int enemyNetId, int toolNetId, Vector3 startPos, Vector3 impulse, byte impulseData, Player player, PhotonMessageInfo info)
	{
		float num = 10000f;
		if ((in impulse).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in startPos).IsValid(in num2) && this.IsValidClientRPC(info.Sender, enemyNetId, startPos) && !this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyImpact) && player != null)
			{
				if (player.IsLocal)
				{
					return;
				}
				GameEntity gameEntity = this.entityManager.GetGameEntity(GameEntity.GetIdFromNetId(enemyNetId));
				if (gameEntity == null)
				{
					return;
				}
				impulse = Vector3.ClampMagnitude(impulse, 100f);
				GameEntity gameEntity2 = this.entityManager.GetGameEntity(GameEntity.GetIdFromNetId(toolNetId));
				GRTool grtool = null;
				if (gameEntity2 != null)
				{
					grtool = gameEntity2.GetComponent<GRTool>();
					grtool.UseEnergy();
				}
				GREnemyChaser component = gameEntity.GetComponent<GREnemyChaser>();
				if (component != null)
				{
					component.OnImpact(grtool, startPos, impulse, impulseData);
				}
				GREnemyRanged component2 = gameEntity.GetComponent<GREnemyRanged>();
				if (component2 != null)
				{
					component2.OnImpact(grtool, startPos, impulse, impulseData);
				}
				GRBarrierSpectral component3 = gameEntity.GetComponent<GRBarrierSpectral>();
				if (component3 != null)
				{
					component3.OnImpact(grtool, startPos, impulse, impulseData);
				}
				return;
			}
		}
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000A71D4 File Offset: 0x000A53D4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		int num = Mathf.Min(4, this.agents.Count);
		stream.SendNext(num);
		for (int i = 0; i < num; i++)
		{
			if (this.nextAgentIndexUpdate >= this.agents.Count)
			{
				this.nextAgentIndexUpdate = 0;
			}
			stream.SendNext(GameEntity.GetNetId(this.agents[this.nextAgentIndexUpdate].entity.id));
			long num2 = BitPackUtils.PackWorldPosForNetwork(this.agents[this.nextAgentIndexUpdate].transform.position);
			stream.SendNext(num2);
			int num3 = BitPackUtils.PackQuaternionForNetwork(this.agents[this.nextAgentIndexUpdate].transform.rotation);
			stream.SendNext(num3);
			this.nextAgentIndexUpdate++;
		}
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000A72C0 File Offset: 0x000A54C0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		for (int i = 0; i < num; i++)
		{
			int num2 = (int)stream.ReceiveNext();
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)stream.ReceiveNext());
			Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork((int)stream.ReceiveNext());
			if (this.IsPositionInZone(vector) && GameEntityManager.IsValidNetId(num2))
			{
				GameAgent gameAgent = GameAgent.Get(GameEntity.GetIdFromNetId(num2));
				if (gameAgent != null)
				{
					gameAgent.ApplyNetworkUpdate(vector, quaternion);
				}
			}
		}
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002583 RID: 9603
	[OnEnterPlay_SetNull]
	public static volatile GameAgentManager instance;

	// Token: 0x04002584 RID: 9604
	public GameEntityManager entityManager;

	// Token: 0x04002585 RID: 9605
	public PhotonView photonView;

	// Token: 0x04002586 RID: 9606
	private List<GameAgent> agents;

	// Token: 0x04002587 RID: 9607
	private float lastDestinationSentTime;

	// Token: 0x04002588 RID: 9608
	private float destinationCooldown;

	// Token: 0x04002589 RID: 9609
	private List<int> netIdsForDestination;

	// Token: 0x0400258A RID: 9610
	private List<Vector3> destinationsForDestination;

	// Token: 0x0400258B RID: 9611
	private List<int> netIdsForState;

	// Token: 0x0400258C RID: 9612
	private List<byte> statesForState;

	// Token: 0x0400258D RID: 9613
	private float lastStateSentTime;

	// Token: 0x0400258E RID: 9614
	private float stateCooldown;

	// Token: 0x0400258F RID: 9615
	private List<int> netIdsForBehavior;

	// Token: 0x04002590 RID: 9616
	private List<byte> behaviorsForBehavior;

	// Token: 0x04002591 RID: 9617
	private float lastBehaviorSentTime;

	// Token: 0x04002592 RID: 9618
	private float behaviorCooldown = 0.25f;

	// Token: 0x04002593 RID: 9619
	private const int MAX_UPDATES_PER_FRAME = 4;

	// Token: 0x04002594 RID: 9620
	private int nextAgentIndexUpdate;

	// Token: 0x04002595 RID: 9621
	public CallLimitersList<CallLimiter, GameAgentManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameAgentManager.RPC>();

	// Token: 0x0200055F RID: 1375
	public enum RPC
	{
		// Token: 0x04002597 RID: 9623
		ApplyDestination,
		// Token: 0x04002598 RID: 9624
		ApplyState,
		// Token: 0x04002599 RID: 9625
		ApplyBehaviour,
		// Token: 0x0400259A RID: 9626
		ApplyImpact
	}
}

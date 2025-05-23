using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020002A1 RID: 673
[Serializable]
public abstract class NetPlayer : ObjectPoolEvents
{
	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06000FA5 RID: 4005
	public abstract bool IsValid { get; }

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06000FA6 RID: 4006
	public abstract int ActorNumber { get; }

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06000FA7 RID: 4007
	public abstract string UserId { get; }

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06000FA8 RID: 4008
	public abstract bool IsMasterClient { get; }

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06000FA9 RID: 4009
	public abstract bool IsLocal { get; }

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06000FAA RID: 4010
	public abstract bool IsNull { get; }

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06000FAB RID: 4011
	public abstract string NickName { get; }

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06000FAC RID: 4012 RVA: 0x0004EE2E File Offset: 0x0004D02E
	// (set) Token: 0x06000FAD RID: 4013 RVA: 0x0004EE36 File Offset: 0x0004D036
	public virtual string SanitizedNickName { get; set; } = string.Empty;

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06000FAE RID: 4014
	public abstract string DefaultName { get; }

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06000FAF RID: 4015
	public abstract bool InRoom { get; }

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x0004EE3F File Offset: 0x0004D03F
	// (set) Token: 0x06000FB1 RID: 4017 RVA: 0x0004EE47 File Offset: 0x0004D047
	public virtual float JoinedTime { get; private set; }

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06000FB2 RID: 4018 RVA: 0x0004EE50 File Offset: 0x0004D050
	// (set) Token: 0x06000FB3 RID: 4019 RVA: 0x0004EE58 File Offset: 0x0004D058
	public virtual float LeftTime { get; private set; }

	// Token: 0x06000FB4 RID: 4020
	public abstract bool Equals(NetPlayer myPlayer, NetPlayer other);

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0004EE61 File Offset: 0x0004D061
	public virtual void OnReturned()
	{
		this.LeftTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus != null)
		{
			singleCallRPCStatus.Clear();
		}
		this.SanitizedNickName = string.Empty;
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0004EE8A File Offset: 0x0004D08A
	public virtual void OnTaken()
	{
		this.JoinedTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus == null)
		{
			return;
		}
		singleCallRPCStatus.Clear();
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0004EEA7 File Offset: 0x0004D0A7
	public virtual bool CheckSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		return this.SingleCallRPCStatus.Contains((int)RPCType);
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0004EEB5 File Offset: 0x0004D0B5
	public virtual void ReceivedSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		this.SingleCallRPCStatus.Add((int)RPCType);
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0004EEC4 File Offset: 0x0004D0C4
	public Player GetPlayerRef()
	{
		return (this as PunNetPlayer).PlayerRef;
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x0004EED1 File Offset: 0x0004D0D1
	public string ToStringFull()
	{
		return string.Format("#{0: 0:00} '{1}', Not sure what to do with inactive yet, Or custom props?", this.ActorNumber, this.NickName);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0004EEEE File Offset: 0x0004D0EE
	public static implicit operator NetPlayer(Player player)
	{
		Utils.Log("Using an implicit cast from Player to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0004EF11 File Offset: 0x0004D111
	public static implicit operator NetPlayer(PlayerRef player)
	{
		Utils.Log("Using an implicit cast from PlayerRef to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0004EF34 File Offset: 0x0004D134
	public static NetPlayer Get(Player player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0004EF4D File Offset: 0x0004D14D
	public static NetPlayer Get(PlayerRef player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x0400129C RID: 4764
	private HashSet<int> SingleCallRPCStatus = new HashSet<int>(5);

	// Token: 0x020002A2 RID: 674
	public enum SingleCallRPC
	{
		// Token: 0x0400129E RID: 4766
		CMS_RequestRoomInitialization,
		// Token: 0x0400129F RID: 4767
		CMS_RequestTriggerHistory,
		// Token: 0x040012A0 RID: 4768
		CMS_SyncTriggerHistory,
		// Token: 0x040012A1 RID: 4769
		CMS_SyncTriggerCounts,
		// Token: 0x040012A2 RID: 4770
		RequestQuestScore,
		// Token: 0x040012A3 RID: 4771
		Count
	}
}

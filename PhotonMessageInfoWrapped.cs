using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020002A0 RID: 672
public struct PhotonMessageInfoWrapped
{
	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06000F9E RID: 3998 RVA: 0x0004ED7D File Offset: 0x0004CF7D
	public NetPlayer Sender
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.senderID);
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06000F9F RID: 3999 RVA: 0x0004ED8F File Offset: 0x0004CF8F
	public double SentServerTime
	{
		get
		{
			return this.sentTick / 1000.0;
		}
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0004EDA3 File Offset: 0x0004CFA3
	public PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		this.senderID = ((sender != null) ? sender.ActorNumber : (-1));
		this.sentTick = info.SentServerTimestamp;
		this.punInfo = info;
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0004EDD1 File Offset: 0x0004CFD1
	public PhotonMessageInfoWrapped(RpcInfo info)
	{
		this.senderID = info.Source.PlayerId;
		this.sentTick = info.Tick.Raw;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0004EE02 File Offset: 0x0004D002
	public PhotonMessageInfoWrapped(int playerID, int tick)
	{
		this.senderID = playerID;
		this.sentTick = tick;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0004EE1E File Offset: 0x0004D01E
	public static implicit operator PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x0004EE26 File Offset: 0x0004D026
	public static implicit operator PhotonMessageInfoWrapped(RpcInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x04001296 RID: 4758
	public int senderID;

	// Token: 0x04001297 RID: 4759
	public int sentTick;

	// Token: 0x04001298 RID: 4760
	public PhotonMessageInfo punInfo;
}

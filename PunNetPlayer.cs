using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020002D0 RID: 720
[Serializable]
public class PunNetPlayer : NetPlayer
{
	// Token: 0x170001EC RID: 492
	// (get) Token: 0x0600115B RID: 4443 RVA: 0x00053DB3 File Offset: 0x00051FB3
	// (set) Token: 0x0600115C RID: 4444 RVA: 0x00053DBB File Offset: 0x00051FBB
	public Player PlayerRef { get; private set; }

	// Token: 0x0600115E RID: 4446 RVA: 0x00053DCC File Offset: 0x00051FCC
	public void InitPlayer(Player playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x0600115F RID: 4447 RVA: 0x00053DD5 File Offset: 0x00051FD5
	public override bool IsValid
	{
		get
		{
			return !this.PlayerRef.IsInactive;
		}
	}

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001160 RID: 4448 RVA: 0x00053DE5 File Offset: 0x00051FE5
	public override int ActorNumber
	{
		get
		{
			Player playerRef = this.PlayerRef;
			if (playerRef == null)
			{
				return -1;
			}
			return playerRef.ActorNumber;
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001161 RID: 4449 RVA: 0x00053DF8 File Offset: 0x00051FF8
	public override string UserId
	{
		get
		{
			return this.PlayerRef.UserId;
		}
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001162 RID: 4450 RVA: 0x00053E05 File Offset: 0x00052005
	public override bool IsMasterClient
	{
		get
		{
			return this.PlayerRef.IsMasterClient;
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001163 RID: 4451 RVA: 0x00053E12 File Offset: 0x00052012
	public override bool IsLocal
	{
		get
		{
			return this.PlayerRef == PhotonNetwork.LocalPlayer;
		}
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001164 RID: 4452 RVA: 0x00053E21 File Offset: 0x00052021
	public override bool IsNull
	{
		get
		{
			return this.PlayerRef == null;
		}
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x06001165 RID: 4453 RVA: 0x00053E2C File Offset: 0x0005202C
	public override string NickName
	{
		get
		{
			return this.PlayerRef.NickName;
		}
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001166 RID: 4454 RVA: 0x00053E39 File Offset: 0x00052039
	public override string DefaultName
	{
		get
		{
			return this.PlayerRef.DefaultName;
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06001167 RID: 4455 RVA: 0x00053E46 File Offset: 0x00052046
	public override bool InRoom
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && currentRoom.Players.ContainsValue(this.PlayerRef);
		}
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x00053E63 File Offset: 0x00052063
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((PunNetPlayer)myPlayer).PlayerRef.Equals(((PunNetPlayer)other).PlayerRef);
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x00053E88 File Offset: 0x00052088
	public override void OnReturned()
	{
		base.OnReturned();
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x00053E90 File Offset: 0x00052090
	public override void OnTaken()
	{
		base.OnTaken();
		this.PlayerRef = null;
	}
}

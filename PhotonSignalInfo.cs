using System;
using Photon.Pun;

// Token: 0x0200090E RID: 2318
[Serializable]
public struct PhotonSignalInfo
{
	// Token: 0x0600383C RID: 14396 RVA: 0x001100F8 File Offset: 0x0010E2F8
	public PhotonSignalInfo(NetPlayer sender, int timestamp)
	{
		this.sender = sender;
		this.timestamp = timestamp;
	}

	// Token: 0x17000587 RID: 1415
	// (get) Token: 0x0600383D RID: 14397 RVA: 0x00110108 File Offset: 0x0010E308
	public double sentServerTime
	{
		get
		{
			return this.timestamp / 1000.0;
		}
	}

	// Token: 0x0600383E RID: 14398 RVA: 0x0011011C File Offset: 0x0010E31C
	public override string ToString()
	{
		return string.Format("[{0}: Sender = '{1}' sentTime = {2}]", "PhotonSignalInfo", this.sender.ActorNumber, this.sentServerTime);
	}

	// Token: 0x0600383F RID: 14399 RVA: 0x00110148 File Offset: 0x0010E348
	public static implicit operator PhotonMessageInfo(PhotonSignalInfo psi)
	{
		return new PhotonMessageInfo(psi.sender.GetPlayerRef(), psi.timestamp, null);
	}

	// Token: 0x04003DDA RID: 15834
	public readonly int timestamp;

	// Token: 0x04003DDB RID: 15835
	public readonly NetPlayer sender;
}

using System;
using Photon.Pun;

// Token: 0x0200090E RID: 2318
[Serializable]
public struct PhotonSignalInfo
{
	// Token: 0x0600383B RID: 14395 RVA: 0x00110020 File Offset: 0x0010E220
	public PhotonSignalInfo(NetPlayer sender, int timestamp)
	{
		this.sender = sender;
		this.timestamp = timestamp;
	}

	// Token: 0x17000587 RID: 1415
	// (get) Token: 0x0600383C RID: 14396 RVA: 0x00110030 File Offset: 0x0010E230
	public double sentServerTime
	{
		get
		{
			return this.timestamp / 1000.0;
		}
	}

	// Token: 0x0600383D RID: 14397 RVA: 0x00110044 File Offset: 0x0010E244
	public override string ToString()
	{
		return string.Format("[{0}: Sender = '{1}' sentTime = {2}]", "PhotonSignalInfo", this.sender.ActorNumber, this.sentServerTime);
	}

	// Token: 0x0600383E RID: 14398 RVA: 0x00110070 File Offset: 0x0010E270
	public static implicit operator PhotonMessageInfo(PhotonSignalInfo psi)
	{
		return new PhotonMessageInfo(psi.sender.GetPlayerRef(), psi.timestamp, null);
	}

	// Token: 0x04003DD9 RID: 15833
	public readonly int timestamp;

	// Token: 0x04003DDA RID: 15834
	public readonly NetPlayer sender;
}

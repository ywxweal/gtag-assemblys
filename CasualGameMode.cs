using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;

// Token: 0x02000484 RID: 1156
public class CasualGameMode : GorillaGameManager
{
	// Token: 0x06001C4E RID: 7246 RVA: 0x0008AF4B File Offset: 0x0008914B
	public override int MyMatIndex(NetPlayer player)
	{
		if (this.GetMyMaterial == null)
		{
			return 0;
		}
		return this.GetMyMaterial(player);
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x00045F91 File Offset: 0x00044191
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001C53 RID: 7251 RVA: 0x00002076 File Offset: 0x00000276
	public override GameModeType GameType()
	{
		return GameModeType.Casual;
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x0008AF63 File Offset: 0x00089163
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x0008AF6C File Offset: 0x0008916C
	public override string GameModeName()
	{
		return "CASUAL";
	}

	// Token: 0x04001F7F RID: 8063
	public CasualGameMode.MyMatDelegate GetMyMaterial;

	// Token: 0x02000485 RID: 1157
	// (Invoke) Token: 0x06001C58 RID: 7256
	public delegate int MyMatDelegate(NetPlayer player);
}

using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000933 RID: 2355
internal class PaintbrawlRPCs : RPCNetworkBase
{
	// Token: 0x06003932 RID: 14642 RVA: 0x0011340D File Offset: 0x0011160D
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.paintbrawlManager = (GorillaPaintbrawlManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06003933 RID: 14643 RVA: 0x00113428 File Offset: 0x00111628
	[PunRPC]
	public void RPC_ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPC_ReportSlingshotHit");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayer);
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.paintbrawlManager.ReportSlingshotHit(player, hitLocation, projectileCount, photonMessageInfoWrapped);
	}

	// Token: 0x04003E5E RID: 15966
	private GameModeSerializer serializer;

	// Token: 0x04003E5F RID: 15967
	private GorillaPaintbrawlManager paintbrawlManager;
}

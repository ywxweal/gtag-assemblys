using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000693 RID: 1683
public class TappableSystem : GTSystem<Tappable>
{
	// Token: 0x06002A29 RID: 10793 RVA: 0x000D074C File Offset: 0x000CE94C
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnTapRPC");
		if (key < 0 || key >= this._instances.Count || !float.IsFinite(tapStrength))
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		this._instances[key].OnTapLocal(tapStrength, Time.time, new PhotonMessageInfoWrapped(info));
	}
}

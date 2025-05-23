using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200049F RID: 1183
public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	// Token: 0x06001CB2 RID: 7346 RVA: 0x0008B960 File Offset: 0x00089B60
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindObjectOfType<GorillaGameManager>()).RPC(this.functionName, RpcTarget.MasterClient, null);
	}

	// Token: 0x04001FEF RID: 8175
	public string functionName;
}

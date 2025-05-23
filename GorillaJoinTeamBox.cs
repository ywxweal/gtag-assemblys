using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200061E RID: 1566
public class GorillaJoinTeamBox : GorillaTriggerBox
{
	// Token: 0x060026DA RID: 9946 RVA: 0x000C10A5 File Offset: 0x000BF2A5
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("GorillaGameManager").GetComponent<GorillaGameManager>() != null)
		{
			bool inRoom = PhotonNetwork.InRoom;
		}
	}

	// Token: 0x04002B59 RID: 11097
	public bool joinRedTeam;
}

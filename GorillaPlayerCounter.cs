using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200053A RID: 1338
public class GorillaPlayerCounter : MonoBehaviour
{
	// Token: 0x06002082 RID: 8322 RVA: 0x000A319C File Offset: 0x000A139C
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x000A31B0 File Offset: 0x000A13B0
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null)
		{
			int num = 0;
			foreach (KeyValuePair<int, Player> keyValuePair in PhotonNetwork.CurrentRoom.Players)
			{
				if ((bool)keyValuePair.Value.CustomProperties["isRedTeam"] == this.isRedTeam)
				{
					num++;
				}
			}
			this.text.text = num.ToString();
		}
	}

	// Token: 0x0400246F RID: 9327
	public bool isRedTeam;

	// Token: 0x04002470 RID: 9328
	public Text text;

	// Token: 0x04002471 RID: 9329
	public string attribute;
}

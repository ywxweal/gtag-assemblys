using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200053B RID: 1339
public class GorillaScoreCounter : MonoBehaviour
{
	// Token: 0x06002085 RID: 8325 RVA: 0x000A3224 File Offset: 0x000A1424
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
		if (this.isRedTeam)
		{
			this.attribute = "redScore";
			return;
		}
		this.attribute = "blueScore";
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x000A3258 File Offset: 0x000A1458
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties[this.attribute] != null)
		{
			this.text.text = ((int)PhotonNetwork.CurrentRoom.CustomProperties[this.attribute]).ToString();
		}
	}

	// Token: 0x04002472 RID: 9330
	public bool isRedTeam;

	// Token: 0x04002473 RID: 9331
	public Text text;

	// Token: 0x04002474 RID: 9332
	public string attribute;
}

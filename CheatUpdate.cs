using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000522 RID: 1314
public class CheatUpdate : MonoBehaviour
{
	// Token: 0x06001FCE RID: 8142 RVA: 0x000A0D31 File Offset: 0x0009EF31
	private void Start()
	{
		base.StartCoroutine(this.UpdateNumberOfPlayers());
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x000A0D40 File Offset: 0x0009EF40
	public IEnumerator UpdateNumberOfPlayers()
	{
		for (;;)
		{
			base.StartCoroutine(this.UpdatePlayerCount());
			yield return new WaitForSeconds(10f);
		}
		yield break;
	}

	// Token: 0x06001FD0 RID: 8144 RVA: 0x000A0D4F File Offset: 0x0009EF4F
	private IEnumerator UpdatePlayerCount()
	{
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("player_count", PhotonNetwork.CountOfPlayers - 1);
		wwwform.AddField("game_version", "live");
		wwwform.AddField("game_name", Application.productName);
		Debug.Log(PhotonNetwork.CountOfPlayers - 1);
		using (UnityWebRequest www = UnityWebRequest.Post("http://ntsfranz.crabdance.com/update_monke_count", wwwform))
		{
			yield return www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.Log(www.error);
			}
		}
		UnityWebRequest www = null;
		yield break;
		yield break;
	}
}

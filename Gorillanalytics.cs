using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;

// Token: 0x02000624 RID: 1572
public class Gorillanalytics : MonoBehaviour
{
	// Token: 0x060026F1 RID: 9969 RVA: 0x000C14F9 File Offset: 0x000BF6F9
	private IEnumerator Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("GorillanalyticsChance", delegate(string s)
		{
			double num;
			if (double.TryParse(s, out num))
			{
				this.oneOverChance = num;
			}
		}, delegate(PlayFabError e)
		{
		});
		for (;;)
		{
			yield return new WaitForSeconds(this.interval);
			if ((double)Random.Range(0f, 1f) < 1.0 / this.oneOverChance && PlayFabClientAPI.IsClientLoggedIn())
			{
				this.UploadGorillanalytics();
			}
		}
		yield break;
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000C1508 File Offset: 0x000BF708
	private void UploadGorillanalytics()
	{
		try
		{
			string text;
			string text2;
			string text3;
			this.GetMapModeQueue(out text, out text2, out text3);
			Vector3 position = GTPlayer.Instance.headCollider.transform.position;
			Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
			this.uploadData.version = NetworkSystemConfig.AppVersion;
			this.uploadData.upload_chance = this.oneOverChance;
			this.uploadData.map = text;
			this.uploadData.mode = text2;
			this.uploadData.queue = text3;
			this.uploadData.player_count = (int)(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.PlayerCount : 0);
			this.uploadData.pos_x = position.x;
			this.uploadData.pos_y = position.y;
			this.uploadData.pos_z = position.z;
			this.uploadData.vel_x = averagedVelocity.x;
			this.uploadData.vel_y = averagedVelocity.y;
			this.uploadData.vel_z = averagedVelocity.z;
			this.uploadData.cosmetics_owned = string.Join(";", CosmeticsController.instance.unlockedCosmetics.Select((CosmeticsController.CosmeticItem c) => c.itemName));
			this.uploadData.cosmetics_worn = string.Join(";", CosmeticsController.instance.currentWornSet.items.Select((CosmeticsController.CosmeticItem c) => c.itemName));
			GorillaServer.Instance.UploadGorillanalytics(this.uploadData);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000C16D8 File Offset: 0x000BF8D8
	private void GetMapModeQueue(out string map, out string mode, out string queue)
	{
		if (!PhotonNetwork.InRoom)
		{
			map = "none";
			mode = "none";
			queue = "none";
			return;
		}
		object obj = null;
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null)
		{
			currentRoom.CustomProperties.TryGetValue("gameMode", out obj);
		}
		string gameMode = ((obj != null) ? obj.ToString() : null) ?? "";
		map = this.maps.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
		mode = this.modes.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
		queue = this.queues.FirstOrDefault((string s) => gameMode.Contains(s)) ?? "unknown";
	}

	// Token: 0x04002B7A RID: 11130
	public float interval = 60f;

	// Token: 0x04002B7B RID: 11131
	public double oneOverChance = 4320.0;

	// Token: 0x04002B7C RID: 11132
	public PhotonNetworkController photonNetworkController;

	// Token: 0x04002B7D RID: 11133
	public GameModeZoneMapping gameModeData;

	// Token: 0x04002B7E RID: 11134
	public List<string> maps;

	// Token: 0x04002B7F RID: 11135
	public List<string> modes;

	// Token: 0x04002B80 RID: 11136
	public List<string> queues;

	// Token: 0x04002B81 RID: 11137
	private readonly Gorillanalytics.UploadData uploadData = new Gorillanalytics.UploadData();

	// Token: 0x02000625 RID: 1573
	private class UploadData
	{
		// Token: 0x04002B82 RID: 11138
		public string version;

		// Token: 0x04002B83 RID: 11139
		public double upload_chance;

		// Token: 0x04002B84 RID: 11140
		public string map;

		// Token: 0x04002B85 RID: 11141
		public string mode;

		// Token: 0x04002B86 RID: 11142
		public string queue;

		// Token: 0x04002B87 RID: 11143
		public int player_count;

		// Token: 0x04002B88 RID: 11144
		public float pos_x;

		// Token: 0x04002B89 RID: 11145
		public float pos_y;

		// Token: 0x04002B8A RID: 11146
		public float pos_z;

		// Token: 0x04002B8B RID: 11147
		public float vel_x;

		// Token: 0x04002B8C RID: 11148
		public float vel_y;

		// Token: 0x04002B8D RID: 11149
		public float vel_z;

		// Token: 0x04002B8E RID: 11150
		public string cosmetics_owned;

		// Token: 0x04002B8F RID: 11151
		public string cosmetics_worn;
	}
}

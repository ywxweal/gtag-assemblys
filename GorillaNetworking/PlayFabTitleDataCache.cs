using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace GorillaNetworking
{
	// Token: 0x02000C6D RID: 3181
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06004EE5 RID: 20197 RVA: 0x001781F0 File Offset: 0x001763F0
		// (set) Token: 0x06004EE6 RID: 20198 RVA: 0x001781F7 File Offset: 0x001763F7
		public static PlayFabTitleDataCache Instance { get; private set; }

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06004EE7 RID: 20199 RVA: 0x001781FF File Offset: 0x001763FF
		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		// Token: 0x06004EE8 RID: 20200 RVA: 0x00178210 File Offset: 0x00176410
		public void GetTitleData(string name, Action<string> callback, Action<PlayFabError> errorCallback)
		{
			if (this.isDataUpToDate && this.titleData.ContainsKey(name))
			{
				callback.SafeInvoke(this.titleData[name]);
				return;
			}
			PlayFabTitleDataCache.DataRequest dataRequest = new PlayFabTitleDataCache.DataRequest
			{
				Name = name,
				Callback = callback,
				ErrorCallback = errorCallback
			};
			this.requests.Add(dataRequest);
			if (this.isDataUpToDate && this.updateDataCoroutine == null)
			{
				this.UpdateData();
			}
		}

		// Token: 0x06004EE9 RID: 20201 RVA: 0x00178283 File Offset: 0x00176483
		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
		}

		// Token: 0x06004EEA RID: 20202 RVA: 0x0017829F File Offset: 0x0017649F
		private void Start()
		{
			this.UpdateData();
		}

		// Token: 0x06004EEB RID: 20203 RVA: 0x001782A8 File Offset: 0x001764A8
		public void LoadDataFromFile()
		{
			try
			{
				if (!File.Exists(PlayFabTitleDataCache.FilePath))
				{
					Debug.LogWarning("Title data file " + PlayFabTitleDataCache.FilePath + " does not exist!");
				}
				else
				{
					string text = File.ReadAllText(PlayFabTitleDataCache.FilePath);
					this.titleData = JsonMapper.ToObject<Dictionary<string, string>>(text);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error reading PlayFab title data from file: {0}", ex));
			}
		}

		// Token: 0x06004EEC RID: 20204 RVA: 0x00178318 File Offset: 0x00176518
		private void SaveDataToFile(string filepath)
		{
			try
			{
				string text = JsonMapper.ToJson(this.titleData);
				File.WriteAllText(filepath, text);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error writing PlayFab title data to file: {0}", ex));
			}
		}

		// Token: 0x06004EED RID: 20205 RVA: 0x00178360 File Offset: 0x00176560
		public void UpdateData()
		{
			this.updateDataCoroutine = base.StartCoroutine(this.UpdateDataCo());
		}

		// Token: 0x06004EEE RID: 20206 RVA: 0x00178374 File Offset: 0x00176574
		private IEnumerator UpdateDataCo()
		{
			this.LoadDataFromFile();
			this.LoadKey();
			Dictionary<string, string> dictionary = this.titleData;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>((dictionary != null) ? dictionary.Count : 0);
			if (this.titleData != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.titleData)
				{
					string text;
					string text2;
					keyValuePair.Deconstruct(out text, out text2);
					string text3 = text;
					string text4 = text2;
					if (text3 != null)
					{
						dictionary2[text3] = ((text4 != null) ? PlayFabTitleDataCache.MD5(text4) : null);
					}
				}
			}
			string text5 = JsonMapper.ToJson(new Dictionary<string, object>
			{
				{
					"version",
					Application.version
				},
				{ "key", this.titleDataKey },
				{ "data", dictionary2 }
			});
			Stopwatch sw = Stopwatch.StartNew();
			Dictionary<string, JsonData> dictionary3;
			using (UnityWebRequest www = new UnityWebRequest(PlayFabAuthenticatorSettings.TitleDataApiBaseUrl, "POST"))
			{
				byte[] bytes = new UTF8Encoding(true).GetBytes(text5);
				www.uploadHandler = new UploadHandlerRaw(bytes);
				www.downloadHandler = new DownloadHandlerBuffer();
				www.SetRequestHeader("Content-Type", "application/json");
				www.timeout = 15;
				yield return www.SendWebRequest();
				if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
				{
					Debug.LogError("Failed to get TitleData from the server.\n" + www.error);
					this.ClearRequestWithError(null);
					yield break;
				}
				dictionary3 = JsonMapper.ToObject<Dictionary<string, JsonData>>(www.downloadHandler.text);
			}
			UnityWebRequest www = null;
			Debug.Log(string.Format("TitleData fetched: {0:N5}", sw.Elapsed.TotalSeconds));
			foreach (KeyValuePair<string, JsonData> keyValuePair2 in dictionary3)
			{
				PlayFabTitleDataCache.DataUpdate onTitleDataUpdate = this.OnTitleDataUpdate;
				if (onTitleDataUpdate != null)
				{
					onTitleDataUpdate.Invoke(keyValuePair2.Key);
				}
				if (keyValuePair2.Value == null)
				{
					this.titleData.Remove(keyValuePair2.Key);
				}
				else
				{
					this.titleData.AddOrUpdate(keyValuePair2.Key, JsonMapper.ToJson(keyValuePair2.Value));
				}
			}
			if (dictionary3.Keys.Count > 0)
			{
				this.SaveDataToFile(PlayFabTitleDataCache.FilePath);
			}
			this.requests.RemoveAll(delegate(PlayFabTitleDataCache.DataRequest request)
			{
				string text6;
				if (this.titleData.TryGetValue(request.Name, out text6))
				{
					request.Callback.SafeInvoke(text6);
					return true;
				}
				return false;
			});
			this.ClearRequestWithError(null);
			this.isDataUpToDate = true;
			this.updateDataCoroutine = null;
			yield break;
			yield break;
		}

		// Token: 0x06004EEF RID: 20207 RVA: 0x00178384 File Offset: 0x00176584
		private void LoadKey()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("title_data_key");
			this.titleDataKey = textAsset.text;
		}

		// Token: 0x06004EF0 RID: 20208 RVA: 0x001783A8 File Offset: 0x001765A8
		private static string MD5(string value)
		{
			HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(value);
			byte[] array = hashAlgorithm.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004EF1 RID: 20209 RVA: 0x00178400 File Offset: 0x00176600
		private void ClearRequestWithError(PlayFabError e = null)
		{
			if (e == null)
			{
				e = new PlayFabError();
			}
			foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
			{
				dataRequest.ErrorCallback.SafeInvoke(e);
			}
			this.requests.Clear();
		}

		// Token: 0x0400520C RID: 21004
		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		// Token: 0x0400520D RID: 21005
		private const string FileName = "TitleDataCache.json";

		// Token: 0x0400520E RID: 21006
		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		// Token: 0x0400520F RID: 21007
		private Dictionary<string, string> titleData = new Dictionary<string, string>();

		// Token: 0x04005210 RID: 21008
		private string titleDataKey;

		// Token: 0x04005211 RID: 21009
		private bool isDataUpToDate;

		// Token: 0x04005212 RID: 21010
		private Coroutine updateDataCoroutine;

		// Token: 0x02000C6E RID: 3182
		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		// Token: 0x02000C6F RID: 3183
		private class DataRequest
		{
			// Token: 0x170007CF RID: 1999
			// (get) Token: 0x06004EF5 RID: 20213 RVA: 0x001784C5 File Offset: 0x001766C5
			// (set) Token: 0x06004EF6 RID: 20214 RVA: 0x001784CD File Offset: 0x001766CD
			public string Name { get; set; }

			// Token: 0x170007D0 RID: 2000
			// (get) Token: 0x06004EF7 RID: 20215 RVA: 0x001784D6 File Offset: 0x001766D6
			// (set) Token: 0x06004EF8 RID: 20216 RVA: 0x001784DE File Offset: 0x001766DE
			public Action<string> Callback { get; set; }

			// Token: 0x170007D1 RID: 2001
			// (get) Token: 0x06004EF9 RID: 20217 RVA: 0x001784E7 File Offset: 0x001766E7
			// (set) Token: 0x06004EFA RID: 20218 RVA: 0x001784EF File Offset: 0x001766EF
			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}

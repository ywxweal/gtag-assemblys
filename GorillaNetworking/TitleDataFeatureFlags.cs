using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C56 RID: 3158
	public class TitleDataFeatureFlags
	{
		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06004E53 RID: 20051 RVA: 0x00175658 File Offset: 0x00173858
		// (set) Token: 0x06004E54 RID: 20052 RVA: 0x00175660 File Offset: 0x00173860
		public bool ready { get; private set; }

		// Token: 0x06004E55 RID: 20053 RVA: 0x00175669 File Offset: 0x00173869
		public void FetchFeatureFlags()
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.TitleDataKey, delegate(string json)
			{
				FeatureFlagListData featureFlagListData = JsonUtility.FromJson<FeatureFlagListData>(json);
				foreach (FeatureFlagData featureFlagData in featureFlagListData.flags)
				{
					if (featureFlagData.valueType == "percent")
					{
						this.flagValueByName.AddOrUpdate(featureFlagData.name, featureFlagData.value);
					}
					List<string> alwaysOnForUsers = featureFlagData.alwaysOnForUsers;
					if (alwaysOnForUsers != null && alwaysOnForUsers.Count > 0)
					{
						this.flagValueByUser.AddOrUpdate(featureFlagData.name, featureFlagData.alwaysOnForUsers);
					}
				}
				Debug.Log(string.Format("GorillaServer: Fetched flags ({0})", featureFlagListData));
				this.ready = true;
			}, delegate(PlayFabError e)
			{
				Debug.LogError("Error fetching rollout feature flags: " + e.ErrorMessage);
				this.ready = true;
			});
		}

		// Token: 0x06004E56 RID: 20054 RVA: 0x00175694 File Offset: 0x00173894
		public bool IsEnabledForUser(string flagName)
		{
			string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			Debug.Log(string.Concat(new string[]
			{
				"GorillaServer: Checking flag ",
				flagName,
				" for ",
				playFabPlayerId,
				"\nFlag values:\n",
				JsonConvert.SerializeObject(this.flagValueByName),
				"\n\nDefaults:\n",
				JsonConvert.SerializeObject(this.defaults)
			}));
			List<string> list;
			if (this.flagValueByUser.TryGetValue(flagName, out list) && list != null && list.Contains(playFabPlayerId))
			{
				return true;
			}
			int num;
			if (!this.flagValueByName.TryGetValue(flagName, out num))
			{
				Debug.Log("GorillaServer: Returning default");
				bool flag;
				return this.defaults.TryGetValue(flagName, out flag) && flag;
			}
			Debug.Log(string.Format("GorillaServer: Rollout % is {0}", num));
			if (num <= 0)
			{
				Debug.Log("GorillaServer: " + flagName + " is off (<=0%).");
				return false;
			}
			if (num >= 100)
			{
				Debug.Log("GorillaServer: " + flagName + " is on (>=100%).");
				return true;
			}
			uint num2 = XXHash32.Compute(Encoding.UTF8.GetBytes(playFabPlayerId), 0U) % 100U;
			Debug.Log(string.Format("GorillaServer: Partial rollout, seed = {0} flag value = {1}", num2, (ulong)num2 < (ulong)((long)num)));
			return (ulong)num2 < (ulong)((long)num);
		}

		// Token: 0x04005155 RID: 20821
		public string TitleDataKey = "DeployFeatureFlags";

		// Token: 0x04005157 RID: 20823
		public Dictionary<string, bool> defaults = new Dictionary<string, bool>
		{
			{ "2024-05-ReturnCurrentVersionV2", true },
			{ "2024-05-ReturnMyOculusHashV2", true },
			{ "2024-05-TryDistributeCurrencyV2", true },
			{ "2024-05-AddOrRemoveDLCOwnershipV2", true },
			{ "2024-05-BroadcastMyRoomV2", true },
			{ "2024-06-CosmeticsAuthenticationV2", true },
			{ "2024-08-KIDIntegrationV1", true },
			{ "2025-04-CosmeticsAuthenticationV2-SetData", false },
			{ "2025-04-CosmeticsAuthenticationV2-ReadData", false },
			{ "2025-04-CosmeticsAuthenticationV2-Compat", true }
		};

		// Token: 0x04005158 RID: 20824
		private Dictionary<string, int> flagValueByName = new Dictionary<string, int>();

		// Token: 0x04005159 RID: 20825
		private Dictionary<string, List<string>> flagValueByUser = new Dictionary<string, List<string>>();
	}
}

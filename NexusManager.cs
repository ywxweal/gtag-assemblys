using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using NexusSDK;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000433 RID: 1075
public class NexusManager : MonoBehaviour
{
	// Token: 0x06001A8E RID: 6798 RVA: 0x00082743 File Offset: 0x00080943
	private void Awake()
	{
		if (NexusManager.instance == null)
		{
			NexusManager.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x0008275F File Offset: 0x0008095F
	private void Start()
	{
		SDKInitializer.Init(this.publicApiKey, this.environment);
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x00082772 File Offset: 0x00080972
	public static IEnumerator GetMembers(NexusManager.GetMembersRequest RequestParams, Action<AttributionAPI.GetMembers200Response> onSuccess, Action<string> onFailure)
	{
		string text = SDKInitializer.ApiBaseUrl + "/manage/members";
		List<string> list = new List<string>();
		if (RequestParams.page != 0)
		{
			list.Add("page=" + RequestParams.page.ToString());
		}
		if (RequestParams.pageSize != 0)
		{
			list.Add("pageSize=" + RequestParams.pageSize.ToString());
		}
		text += "?";
		text += string.Join("&", list);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
		{
			webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
			yield return webRequest.SendWebRequest();
			if (webRequest.responseCode == 200L)
			{
				AttributionAPI.GetMembers200Response getMembers200Response = JsonConvert.DeserializeObject<AttributionAPI.GetMembers200Response>(webRequest.downloadHandler.text, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
				if (onSuccess != null)
				{
					onSuccess(getMembers200Response);
				}
			}
			else if (onFailure != null)
			{
				onFailure(webRequest.error);
			}
		}
		UnityWebRequest webRequest = null;
		yield break;
		yield break;
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x00082790 File Offset: 0x00080990
	public void VerifyCreatorCode(string code, Action<Member> onSuccess, Action onFailure)
	{
		NexusManager.GetMemberByCodeRequest getMemberByCodeRequest = new NexusManager.GetMemberByCodeRequest
		{
			memberCode = code
		};
		base.StartCoroutine(NexusManager.GetMemberByCode(getMemberByCodeRequest, onSuccess, onFailure));
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x000827BE File Offset: 0x000809BE
	public static IEnumerator GetMemberByCode(NexusManager.GetMemberByCodeRequest RequestParams, Action<Member> onSuccess, Action onFailure)
	{
		string text = SDKInitializer.ApiBaseUrl + "/manage/members/{memberCode}";
		text = text.Replace("{memberCode}", RequestParams.memberCode);
		List<string> list = new List<string>();
		text += "?";
		text += string.Join("&", list);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
		{
			webRequest.SetRequestHeader("x-shared-secret", SDKInitializer.ApiKey);
			yield return webRequest.SendWebRequest();
			if (webRequest.responseCode == 200L)
			{
				Member member = JsonConvert.DeserializeObject<Member>(webRequest.downloadHandler.text, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
				if (onSuccess != null)
				{
					onSuccess(member);
				}
			}
			else if (onFailure != null)
			{
				onFailure();
			}
		}
		UnityWebRequest webRequest = null;
		yield break;
		yield break;
	}

	// Token: 0x04001DAD RID: 7597
	private string publicApiKey = "nexus_pk_4c18dcb1531846c7abad4cb00c5242bb";

	// Token: 0x04001DAE RID: 7598
	private string environment = "production";

	// Token: 0x04001DAF RID: 7599
	public static NexusManager instance;

	// Token: 0x04001DB0 RID: 7600
	private Member[] validatedMembers;

	// Token: 0x02000434 RID: 1076
	[Serializable]
	public struct GetMemberByCodeRequest
	{
		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06001A94 RID: 6804 RVA: 0x000827F9 File Offset: 0x000809F9
		// (set) Token: 0x06001A95 RID: 6805 RVA: 0x00082801 File Offset: 0x00080A01
		public string memberCode { readonly get; set; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06001A96 RID: 6806 RVA: 0x0008280A File Offset: 0x00080A0A
		// (set) Token: 0x06001A97 RID: 6807 RVA: 0x00082812 File Offset: 0x00080A12
		public string groupId { readonly get; set; }
	}

	// Token: 0x02000435 RID: 1077
	[Serializable]
	public struct GetMembersRequest
	{
		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06001A98 RID: 6808 RVA: 0x0008281B File Offset: 0x00080A1B
		// (set) Token: 0x06001A99 RID: 6809 RVA: 0x00082823 File Offset: 0x00080A23
		public int page { readonly get; set; }

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06001A9A RID: 6810 RVA: 0x0008282C File Offset: 0x00080A2C
		// (set) Token: 0x06001A9B RID: 6811 RVA: 0x00082834 File Offset: 0x00080A34
		public int pageSize { readonly get; set; }
	}
}

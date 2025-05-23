using System;
using System.Collections;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200047C RID: 1148
public class DeepLinkHandler : MonoBehaviour
{
	// Token: 0x06001C27 RID: 7207 RVA: 0x0008A44C File Offset: 0x0008864C
	public void Awake()
	{
		if (DeepLinkHandler.instance == null)
		{
			DeepLinkHandler.instance = this;
			return;
		}
		if (DeepLinkHandler.instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x0008A47C File Offset: 0x0008867C
	public static void Initialize(GameObject parent)
	{
		if (DeepLinkHandler.instance == null && parent != null)
		{
			parent.AddComponent<DeepLinkHandler>();
		}
		if (DeepLinkHandler.instance == null)
		{
			return;
		}
		DeepLinkHandler.instance.RefreshLaunchDetails();
		if (DeepLinkHandler.instance.cachedLaunchDetails != null && DeepLinkHandler.instance.cachedLaunchDetails.LaunchType == LaunchType.Deeplink)
		{
			DeepLinkHandler.instance.HandleDeepLink();
			return;
		}
		Object.Destroy(DeepLinkHandler.instance);
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x0008A500 File Offset: 0x00088700
	private void RefreshLaunchDetails()
	{
		if (global::UnityEngine.Application.platform != RuntimePlatform.Android)
		{
			GTDev.Log<string>("[DeepLinkHandler::RefreshLaunchDetails] Not on Android Platform!", null);
			return;
		}
		this.cachedLaunchDetails = ApplicationLifecycle.GetLaunchDetails();
		GTDev.Log<string>(string.Concat(new string[]
		{
			"[DeepLinkHandler::RefreshLaunchDetails] LaunchType: ",
			this.cachedLaunchDetails.LaunchType.ToString(),
			"\n[DeepLinkHandler::RefreshLaunchDetails] LaunchSource: ",
			this.cachedLaunchDetails.LaunchSource,
			"\n[DeepLinkHandler::RefreshLaunchDetails] DeepLinkMessage: ",
			this.cachedLaunchDetails.DeeplinkMessage
		}), null);
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x0008A58B File Offset: 0x0008878B
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x0008A5B0 File Offset: 0x000887B0
	private void HandleDeepLink()
	{
		GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Handling deep link...", null);
		if (this.cachedLaunchDetails.LaunchSource.Contains("7221491444554579"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Witchblood, processing...", null);
			string deeplinkMessage = this.cachedLaunchDetails.DeeplinkMessage;
			string launchSource = this.cachedLaunchDetails.LaunchSource;
			string userID = PlayFabAuthenticator.instance.userID;
			string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			string playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket();
			string text = string.Concat(new string[]
			{
				"{ \"itemGUID\": \"", deeplinkMessage, "\", \"launchSource\": \"", launchSource, "\", \"oculusUserID\": \"", userID, "\", \"playFabID\": \"", playFabPlayerId, "\", \"playFabSessionTicket\": \"", playFabSessionTicket,
				"\" }"
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text, "application/json", new Action<UnityWebRequest>(this.OnWitchbloodCollabResponse)));
			return;
		}
		if (this.cachedLaunchDetails.LaunchSource.Contains("1903584373052985"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Racoon Lagoon, processing...", null);
			string deeplinkMessage2 = this.cachedLaunchDetails.DeeplinkMessage;
			string launchSource2 = this.cachedLaunchDetails.LaunchSource;
			string userID2 = PlayFabAuthenticator.instance.userID;
			string playFabPlayerId2 = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			string playFabSessionTicket2 = PlayFabAuthenticator.instance.GetPlayFabSessionTicket();
			string text2 = string.Concat(new string[]
			{
				"{ \"itemGUID\": \"", deeplinkMessage2, "\", \"launchSource\": \"", launchSource2, "\", \"oculusUserID\": \"", userID2, "\", \"playFabID\": \"", playFabPlayerId2, "\", \"playFabSessionTicket\": \"", playFabSessionTicket2,
				"\" }"
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text2, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text2, "application/json", new Action<UnityWebRequest>(this.OnRaccoonLagoonCollabResponse)));
			return;
		}
		GTDev.LogError<string>("[DeepLinkHandler::HandleDeepLink] App launched via DeepLink, but from an unknown app. App ID: " + this.cachedLaunchDetails.LaunchSource, null);
		Object.Destroy(this);
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x0008A7E8 File Offset: 0x000889E8
	private void OnWitchbloodCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.WitchbloodCollabCosmeticID, true, true, true));
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x0008A878 File Offset: 0x00088A78
	private void OnRaccoonLagoonCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.RaccoonLagoonCosmeticIDs, true, true, true));
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x0008A906 File Offset: 0x00088B06
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Checking if we can process external cosmetic unlock...", null);
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			yield return null;
		}
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Cosmetics initialized, proceeding to process external unlock...", null);
		foreach (string text in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(text, autoEquip, isLeftHand);
		}
		if (destroyOnFinish)
		{
			Object.Destroy(this);
		}
		yield break;
	}

	// Token: 0x04001F55 RID: 8021
	public static volatile DeepLinkHandler instance;

	// Token: 0x04001F56 RID: 8022
	private LaunchDetails cachedLaunchDetails;

	// Token: 0x04001F57 RID: 8023
	private const string WitchbloodAppID = "7221491444554579";

	// Token: 0x04001F58 RID: 8024
	private readonly string[] WitchbloodCollabCosmeticID = new string[] { "LMAKT." };

	// Token: 0x04001F59 RID: 8025
	private const string RaccoonLagoonAppID = "1903584373052985";

	// Token: 0x04001F5A RID: 8026
	private readonly string[] RaccoonLagoonCosmeticIDs = new string[] { "LMALI.", "LHAGS." };

	// Token: 0x04001F5B RID: 8027
	private const string HiddenPathCollabEndpoint = "/api/ConsumeItem";
}

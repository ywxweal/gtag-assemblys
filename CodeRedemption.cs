using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020003F0 RID: 1008
public class CodeRedemption : MonoBehaviour
{
	// Token: 0x06001836 RID: 6198 RVA: 0x000759FA File Offset: 0x00073BFA
	public void Awake()
	{
		if (CodeRedemption.Instance == null)
		{
			CodeRedemption.Instance = this;
			return;
		}
		if (CodeRedemption.Instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x00075A2C File Offset: 0x00073C2C
	public void HandleCodeRedemption(string code)
	{
		string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		string playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket();
		string text = string.Concat(new string[] { "{ \"itemGUID\": \"", code, "\", \"playFabID\": \"", playFabPlayerId, "\", \"playFabSessionTicket\": \"", playFabSessionTicket, "\" }" });
		Debug.Log("[CodeRedemption] Web Request body: \n" + text);
		base.StartCoroutine(CodeRedemption.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeCodeItem", text, "application/json", new Action<UnityWebRequest>(this.OnCodeRedemptionResponse)));
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x00075ACC File Offset: 0x00073CCC
	private void OnCodeRedemptionResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("[CodeRedemption] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text);
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		string text = string.Empty;
		try
		{
			CodeRedemption.CodeRedemptionRequest codeRedemptionRequest = JsonUtility.FromJson<CodeRedemption.CodeRedemptionRequest>(completedRequest.downloadHandler.text);
			if (codeRedemptionRequest.result.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log("[CodeRedemption] Item has already been redeemed!");
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.AlreadyUsed;
				return;
			}
			text = codeRedemptionRequest.playFabItemName;
		}
		catch (Exception ex)
		{
			string text2 = "[CodeRedemption] Error parsing JSON response: ";
			Exception ex2 = ex;
			Debug.LogError(text2 + ((ex2 != null) ? ex2.ToString() : null));
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		Debug.Log("[CodeRedemption] Item successfully granted, processing external unlock...");
		GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Success;
		GorillaComputer.instance.RedemptionCode = "";
		base.StartCoroutine(this.CheckProcessExternalUnlock(new string[] { text }, true, true, true));
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x00075BE0 File Offset: 0x00073DE0
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		Debug.Log("[CodeRedemption] Checking if we can process external cosmetic unlock...");
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			yield return null;
		}
		Debug.Log("[CodeRedemption] Cosmetics initialized, proceeding to process external unlock...");
		foreach (string text in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(text, autoEquip, isLeftHand);
		}
		yield break;
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x00075BFD File Offset: 0x00073DFD
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x04001B1B RID: 6939
	public static volatile CodeRedemption Instance;

	// Token: 0x04001B1C RID: 6940
	private const string HiddenPathCollabEndpoint = "/api/ConsumeCodeItem";

	// Token: 0x020003F1 RID: 1009
	[Serializable]
	public class CodeRedemptionRequest
	{
		// Token: 0x04001B1D RID: 6941
		public string result;

		// Token: 0x04001B1E RID: 6942
		public string itemID;

		// Token: 0x04001B1F RID: 6943
		public string playFabItemName;
	}
}

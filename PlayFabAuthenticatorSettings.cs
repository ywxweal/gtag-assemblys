using System;
using UnityEngine;

// Token: 0x020008EF RID: 2287
public class PlayFabAuthenticatorSettings
{
	// Token: 0x06003788 RID: 14216 RVA: 0x0010BF78 File Offset: 0x0010A178
	static PlayFabAuthenticatorSettings()
	{
		PlayFabAuthenticatorSettings.Load("PlayFabAuthenticatorSettings");
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x0010BF84 File Offset: 0x0010A184
	public static void Load(string path)
	{
		PlayFabAuthenticatorSettingsScriptableObject playFabAuthenticatorSettingsScriptableObject = Resources.Load<PlayFabAuthenticatorSettingsScriptableObject>(path);
		PlayFabAuthenticatorSettings.TitleId = playFabAuthenticatorSettingsScriptableObject.TitleId;
		PlayFabAuthenticatorSettings.AuthApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.AuthApiBaseUrl;
		PlayFabAuthenticatorSettings.FriendApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.FriendApiBaseUrl;
		PlayFabAuthenticatorSettings.HpPromoApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.HpPromoApiBaseUrl;
		PlayFabAuthenticatorSettings.IapApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.IapApiBaseUrl;
		PlayFabAuthenticatorSettings.KidApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.KidApiBaseUrl;
		PlayFabAuthenticatorSettings.ProgressionApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ProgressionApiBaseUrl;
		PlayFabAuthenticatorSettings.TitleDataApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.TitleDataApiBaseUrl;
		PlayFabAuthenticatorSettings.VotingApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.VotingApiBaseUrl;
	}

	// Token: 0x04003D07 RID: 15623
	public static string TitleId;

	// Token: 0x04003D08 RID: 15624
	public static string AuthApiBaseUrl;

	// Token: 0x04003D09 RID: 15625
	public static string FriendApiBaseUrl;

	// Token: 0x04003D0A RID: 15626
	public static string HpPromoApiBaseUrl;

	// Token: 0x04003D0B RID: 15627
	public static string IapApiBaseUrl;

	// Token: 0x04003D0C RID: 15628
	public static string KidApiBaseUrl;

	// Token: 0x04003D0D RID: 15629
	public static string ProgressionApiBaseUrl;

	// Token: 0x04003D0E RID: 15630
	public static string TitleDataApiBaseUrl;

	// Token: 0x04003D0F RID: 15631
	public static string VotingApiBaseUrl;
}

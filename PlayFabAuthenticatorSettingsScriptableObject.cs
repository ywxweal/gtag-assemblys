using System;
using UnityEngine;

// Token: 0x020008F0 RID: 2288
[CreateAssetMenu(fileName = "PlayFabAuthenticatorSettings", menuName = "ScriptableObjects/PlayFabAuthenticatorSettings")]
public class PlayFabAuthenticatorSettingsScriptableObject : ScriptableObject
{
	// Token: 0x04003D10 RID: 15632
	public string TitleId;

	// Token: 0x04003D11 RID: 15633
	public string AuthApiBaseUrl;

	// Token: 0x04003D12 RID: 15634
	public string FriendApiBaseUrl;

	// Token: 0x04003D13 RID: 15635
	public string HpPromoApiBaseUrl;

	// Token: 0x04003D14 RID: 15636
	public string IapApiBaseUrl;

	// Token: 0x04003D15 RID: 15637
	public string KidApiBaseUrl;

	// Token: 0x04003D16 RID: 15638
	public string ProgressionApiBaseUrl;

	// Token: 0x04003D17 RID: 15639
	public string TitleDataApiBaseUrl;

	// Token: 0x04003D18 RID: 15640
	public string VotingApiBaseUrl;
}

using System;
using UnityEngine;

// Token: 0x020008EC RID: 2284
public class PhotonAuthenticatorSettings
{
	// Token: 0x06003776 RID: 14198 RVA: 0x0010BF44 File Offset: 0x0010A144
	static PhotonAuthenticatorSettings()
	{
		PhotonAuthenticatorSettings.Load("PhotonAuthenticatorSettings");
	}

	// Token: 0x06003777 RID: 14199 RVA: 0x0010BF50 File Offset: 0x0010A150
	public static void Load(string path)
	{
		PhotonAuthenticatorSettingsScriptableObject photonAuthenticatorSettingsScriptableObject = Resources.Load<PhotonAuthenticatorSettingsScriptableObject>(path);
		PhotonAuthenticatorSettings.PunAppId = photonAuthenticatorSettingsScriptableObject.PunAppId;
		PhotonAuthenticatorSettings.FusionAppId = photonAuthenticatorSettingsScriptableObject.FusionAppId;
		PhotonAuthenticatorSettings.VoiceAppId = photonAuthenticatorSettingsScriptableObject.VoiceAppId;
	}

	// Token: 0x04003D01 RID: 15617
	public static string PunAppId;

	// Token: 0x04003D02 RID: 15618
	public static string FusionAppId;

	// Token: 0x04003D03 RID: 15619
	public static string VoiceAppId;
}

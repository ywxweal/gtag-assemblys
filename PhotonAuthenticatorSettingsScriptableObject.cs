using System;
using UnityEngine;

// Token: 0x020008ED RID: 2285
[CreateAssetMenu(fileName = "PhotonAuthenticatorSettings", menuName = "ScriptableObjects/PhotonAuthenticatorSettings")]
public class PhotonAuthenticatorSettingsScriptableObject : ScriptableObject
{
	// Token: 0x04003D04 RID: 15620
	public string PunAppId;

	// Token: 0x04003D05 RID: 15621
	public string FusionAppId;

	// Token: 0x04003D06 RID: 15622
	public string VoiceAppId;
}

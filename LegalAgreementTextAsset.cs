using System;
using UnityEngine;

// Token: 0x02000850 RID: 2128
[CreateAssetMenu(fileName = "NewLegalAgreementAsset", menuName = "Gorilla Tag/Legal Agreement Asset")]
public class LegalAgreementTextAsset : ScriptableObject
{
	// Token: 0x04003AD7 RID: 15063
	public string title;

	// Token: 0x04003AD8 RID: 15064
	public string playFabKey;

	// Token: 0x04003AD9 RID: 15065
	public string latestVersionKey;

	// Token: 0x04003ADA RID: 15066
	[TextArea(3, 5)]
	public string errorMessage;

	// Token: 0x04003ADB RID: 15067
	public bool optional;

	// Token: 0x04003ADC RID: 15068
	public LegalAgreementTextAsset.PostAcceptAction optInAction;

	// Token: 0x04003ADD RID: 15069
	public string confirmString;

	// Token: 0x02000851 RID: 2129
	public enum PostAcceptAction
	{
		// Token: 0x04003ADF RID: 15071
		NONE
	}
}

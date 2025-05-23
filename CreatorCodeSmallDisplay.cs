using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000446 RID: 1094
public class CreatorCodeSmallDisplay : MonoBehaviour
{
	// Token: 0x06001AF7 RID: 6903 RVA: 0x000846E0 File Offset: 0x000828E0
	private void Awake()
	{
		this.codeText.text = "Creator Code: <none>";
		ATM_Manager.instance.smallDisplays.Add(this);
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x00084704 File Offset: 0x00082904
	public void SetCode(string code)
	{
		if (code == "")
		{
			this.codeText.text = "Creator Code: <none>";
			return;
		}
		this.codeText.text = "Creator Code: " + code;
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x0008473A File Offset: 0x0008293A
	public void SuccessfulPurchase(string memberName)
	{
		if (!string.IsNullOrWhiteSpace(memberName))
		{
			this.codeText.text = "Supported: " + memberName + "!";
		}
	}

	// Token: 0x04001DFD RID: 7677
	public Text codeText;
}

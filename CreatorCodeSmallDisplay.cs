using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000446 RID: 1094
public class CreatorCodeSmallDisplay : MonoBehaviour
{
	// Token: 0x06001AF7 RID: 6903 RVA: 0x00084700 File Offset: 0x00082900
	private void Awake()
	{
		this.codeText.text = "Creator Code: <none>";
		ATM_Manager.instance.smallDisplays.Add(this);
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x00084724 File Offset: 0x00082924
	public void SetCode(string code)
	{
		if (code == "")
		{
			this.codeText.text = "Creator Code: <none>";
			return;
		}
		this.codeText.text = "Creator Code: " + code;
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x0008475A File Offset: 0x0008295A
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

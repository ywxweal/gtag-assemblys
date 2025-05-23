using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200061F RID: 1567
public class GorillaLevelScreen : MonoBehaviour
{
	// Token: 0x060026DC RID: 9948 RVA: 0x000C10EA File Offset: 0x000BF2EA
	private void Awake()
	{
		if (this.myText != null)
		{
			this.startingText = this.myText.text;
		}
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000C110C File Offset: 0x000BF30C
	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	// Token: 0x04002B5A RID: 11098
	public string startingText;

	// Token: 0x04002B5B RID: 11099
	public Material goodMaterial;

	// Token: 0x04002B5C RID: 11100
	public Material badMaterial;

	// Token: 0x04002B5D RID: 11101
	public Text myText;
}

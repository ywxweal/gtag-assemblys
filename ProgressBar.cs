using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006A1 RID: 1697
public class ProgressBar : MonoBehaviour
{
	// Token: 0x06002A78 RID: 10872 RVA: 0x000D13E0 File Offset: 0x000CF5E0
	public void UpdateProgress(float newFill)
	{
		bool flag = newFill > 1f;
		this._fillAmount = Mathf.Clamp(newFill, 0f, 1f);
		this.fillImage.fillAmount = this._fillAmount;
		if (this.useColors)
		{
			if (flag)
			{
				this.fillImage.color = this.overCapacity;
				return;
			}
			if (Mathf.Approximately(this._fillAmount, 1f))
			{
				this.fillImage.color = this.atCapacity;
				return;
			}
			this.fillImage.color = this.underCapacity;
		}
	}

	// Token: 0x04002F5C RID: 12124
	[SerializeField]
	private Image fillImage;

	// Token: 0x04002F5D RID: 12125
	[SerializeField]
	private bool useColors;

	// Token: 0x04002F5E RID: 12126
	[SerializeField]
	private Color underCapacity = Color.green;

	// Token: 0x04002F5F RID: 12127
	[SerializeField]
	private Color overCapacity = Color.red;

	// Token: 0x04002F60 RID: 12128
	[SerializeField]
	private Color atCapacity = Color.yellow;

	// Token: 0x04002F61 RID: 12129
	private float _fillAmount;
}

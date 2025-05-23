using System;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class GRDebugFtueResetButton : GorillaPressableReleaseButton
{
	// Token: 0x0600231A RID: 8986 RVA: 0x000AF9F0 File Offset: 0x000ADBF0
	private void Awake()
	{
		if (!this.availableOnLive)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000AFA06 File Offset: 0x000ADC06
	public void OnPressedButton()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000AFA1C File Offset: 0x000ADC1C
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.isOn = true;
		this.UpdateColor();
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x000AFA31 File Offset: 0x000ADC31
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002757 RID: 10071
	public bool availableOnLive;
}

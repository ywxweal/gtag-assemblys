using System;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class GRDebugFtueResetButton : GorillaPressableReleaseButton
{
	// Token: 0x0600231A RID: 8986 RVA: 0x000AF9D0 File Offset: 0x000ADBD0
	private void Awake()
	{
		if (!this.availableOnLive)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000AF9E6 File Offset: 0x000ADBE6
	public void OnPressedButton()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000AF9FC File Offset: 0x000ADBFC
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.isOn = true;
		this.UpdateColor();
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x000AFA11 File Offset: 0x000ADC11
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002757 RID: 10071
	public bool availableOnLive;
}

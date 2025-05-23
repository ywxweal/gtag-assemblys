using System;
using TMPro;
using UnityEngine;

// Token: 0x020005D1 RID: 1489
public class GRUIScoreboardEntry : MonoBehaviour
{
	// Token: 0x0600244F RID: 9295 RVA: 0x000B68BC File Offset: 0x000B4ABC
	public void Setup(VRRig vrRig, int playerActorId)
	{
		this.playerActorId = playerActorId;
		this.Refresh(vrRig);
	}

	// Token: 0x06002450 RID: 9296 RVA: 0x000B68CC File Offset: 0x000B4ACC
	private void Refresh(VRRig vrRig)
	{
		GRPlayer grplayer = GRPlayer.Get(vrRig);
		if (vrRig != null && grplayer != null)
		{
			if (!this.playerNameLabel.text.Equals(vrRig.playerNameVisible))
			{
				this.playerNameLabel.text = vrRig.playerNameVisible;
			}
			if (grplayer.currency != this.currencySet)
			{
				this.currencySet = grplayer.currency;
				this.playerCurrencyLabel.text = this.currencySet.ToString();
				return;
			}
		}
		else
		{
			this.playerNameLabel.text = "--";
			this.playerCurrencyLabel.text = "--";
			this.currencySet = -1;
		}
	}

	// Token: 0x04002968 RID: 10600
	[SerializeField]
	private TMP_Text playerNameLabel;

	// Token: 0x04002969 RID: 10601
	[SerializeField]
	private TMP_Text playerCurrencyLabel;

	// Token: 0x0400296A RID: 10602
	private int playerActorId = -1;

	// Token: 0x0400296B RID: 10603
	private int currencySet = -1;
}

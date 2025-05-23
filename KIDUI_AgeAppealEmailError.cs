using System;
using TMPro;
using UnityEngine;

// Token: 0x02000814 RID: 2068
public class KIDUI_AgeAppealEmailError : MonoBehaviour
{
	// Token: 0x060032BD RID: 12989 RVA: 0x000FA3FA File Offset: 0x000F85FA
	public void ShowAgeAppealEmailErrorScreen(bool hasChallenge, int newAge, string email)
	{
		this.hasChallenge = hasChallenge;
		this.newAge = newAge;
		this._emailText.text = email;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060032BE RID: 12990 RVA: 0x000FA422 File Offset: 0x000F8622
	public void onBackPressed()
	{
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAge);
	}

	// Token: 0x04003978 RID: 14712
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04003979 RID: 14713
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x0400397A RID: 14714
	private bool hasChallenge;

	// Token: 0x0400397B RID: 14715
	private int newAge;
}

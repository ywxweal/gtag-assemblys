using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000815 RID: 2069
public class KIDUI_AgeDiscrepancyScreen : MonoBehaviour
{
	// Token: 0x060032C0 RID: 12992 RVA: 0x000FA448 File Offset: 0x000F8648
	public async Task ShowAgeDiscrepancyScreenWithAwait(string description)
	{
		base.gameObject.SetActive(true);
		this._descriptionText.text = description;
		await this.WaitForCompletion();
	}

	// Token: 0x060032C1 RID: 12993 RVA: 0x000FA494 File Offset: 0x000F8694
	private async Task WaitForCompletion()
	{
		do
		{
			await Task.Yield();
		}
		while (!this._hasCompleted);
	}

	// Token: 0x060032C2 RID: 12994 RVA: 0x000FA4D7 File Offset: 0x000F86D7
	public void OnHoldComplete()
	{
		this._hasCompleted = true;
	}

	// Token: 0x060032C3 RID: 12995 RVA: 0x0008B909 File Offset: 0x00089B09
	public void OnQuitPressed()
	{
		Application.Quit();
	}

	// Token: 0x0400397C RID: 14716
	[SerializeField]
	private TMP_Text _descriptionText;

	// Token: 0x0400397D RID: 14717
	private bool _hasCompleted;
}

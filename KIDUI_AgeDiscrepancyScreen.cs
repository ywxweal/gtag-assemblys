using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000815 RID: 2069
public class KIDUI_AgeDiscrepancyScreen : MonoBehaviour
{
	// Token: 0x060032BF RID: 12991 RVA: 0x000FA370 File Offset: 0x000F8570
	public async Task ShowAgeDiscrepancyScreenWithAwait(string description)
	{
		base.gameObject.SetActive(true);
		this._descriptionText.text = description;
		await this.WaitForCompletion();
	}

	// Token: 0x060032C0 RID: 12992 RVA: 0x000FA3BC File Offset: 0x000F85BC
	private async Task WaitForCompletion()
	{
		do
		{
			await Task.Yield();
		}
		while (!this._hasCompleted);
	}

	// Token: 0x060032C1 RID: 12993 RVA: 0x000FA3FF File Offset: 0x000F85FF
	public void OnHoldComplete()
	{
		this._hasCompleted = true;
	}

	// Token: 0x060032C2 RID: 12994 RVA: 0x0008B8E9 File Offset: 0x00089AE9
	public void OnQuitPressed()
	{
		Application.Quit();
	}

	// Token: 0x0400397B RID: 14715
	[SerializeField]
	private TMP_Text _descriptionText;

	// Token: 0x0400397C RID: 14716
	private bool _hasCompleted;
}
